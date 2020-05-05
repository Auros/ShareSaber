using HMUI;
using BeatSaberMarkupLanguage;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareSaber.ViewControllers;
using System.Net.Http;
using System.Threading;
using ShareSaber.Models;
using System.IO;
using System.IO.Compression;
using IPA.Utilities;

namespace ShareSaber
{
    public class ShareSaberFlowCoordinator : FlowCoordinator
    {
        private NoFileView _noFileView;
        private LoadingView _loadingView;
        private FindFileView _findFileView;
        private PasswordView _passwordView;
        private DownloadingView _downloadingView;
        private DownloadQueryView _downloadQueryView;

        private NavigationController _navController;

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            if (firstActivation)
            {
                title = "ShareSaber";
                showBackButton = true;

                _loadingView = BeatSaberUI.CreateViewController<LoadingView>();
                _findFileView = BeatSaberUI.CreateViewController<FindFileView>();
                _navController = BeatSaberUI.CreateViewController<NavigationController>();

                _findFileView.SetAction(LoadFileData);
            }
            SetViewControllerToNavigationController(_navController, _findFileView);
            ProvideInitialViewControllers(_navController);
        }

        private void ShowInitialScreen()
        {
            SetViewControllerToNavigationController(_navController, _findFileView);
        }

        private async void LoadFileData(string key)
        {
            CancellationTokenSource cts = new CancellationTokenSource();

            SetViewControllerToNavigationController(_navController, _loadingView);
            _loadingView.SetToken(cts);
            WebResponse fileResponse = await Plugin.Client.SendAsync(new HttpMethod("GET"), Plugin.ShareSaberAPIURL + "api/files/key/" + key, cts.Token);
            if (fileResponse.IsSuccessStatusCode)
            {
                BSFile file = fileResponse.ContentToJson<BSFile>();
                
                if (!file.HasPassword)
                {
                    PasswordEntered("", file);
                }
                else
                {
                    if (_passwordView == null)
                        _passwordView = BeatSaberUI.CreateViewController<PasswordView>();
                    SetViewControllerToNavigationController(_navController, _passwordView);
                    _passwordView.SetAction(file, PasswordEntered);
                }
            }
            else
            {
                if (_noFileView == null)
                    _noFileView = BeatSaberUI.CreateViewController<NoFileView>();
                SetViewControllerToNavigationController(_navController, _noFileView);
                _noFileView.SetAction(ShowInitialScreen);
            }
        }

        string _lastEnteredPassword = "";

        private void PasswordEntered(string pass, BSFile file)
        {
            _lastEnteredPassword = pass;

            if (_downloadQueryView == null)
                _downloadQueryView = BeatSaberUI.CreateViewController<DownloadQueryView>();
            SetViewControllerToNavigationController(_navController, _downloadQueryView);
            _downloadQueryView.Setup(file, ShowInitialScreen, Download);
        }

        private async void Download(BSFile file)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Progress<double> progress = new Progress<double>();
            if (_downloadingView == null)
                _downloadingView = BeatSaberUI.CreateViewController<DownloadingView>();
            SetViewControllerToNavigationController(_navController, _downloadingView);
            _downloadingView.SetToken(cts);

            progress.ProgressChanged += delegate (object o, double prog)
            {
                if (_downloadingView != null)
                    _downloadingView.Progress = string.Format("0.00", prog * 100) + "%";
            };

            WebResponse downloadResponse = await Plugin.Client.SendAsync(new HttpMethod("GET"), Plugin.ShareSaberAPIURL + file.DownloadURL + $"?password={_lastEnteredPassword}", cts.Token, null, null, progress);
            if (downloadResponse.IsSuccessStatusCode)
            {
                string path = Path.Combine(UnityGame.InstallPath, "Beat Saber_Data", "CustomWIPLevels");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                _downloadingView.Progress = "Extracting...";
                await ExtractZipAsync(file, downloadResponse.ContentToBytes(), path);

                SongCore.Loader.Instance.RefreshSongs();
                SongCore.Loader.Instance.RefreshLevelPacks();

                _downloadingView.Progress = "Map Downloaded! Check your library.";
                _downloadingView.Downloading = false;
                
            }
            else
            {
                if (_noFileView == null)
                    _noFileView = BeatSaberUI.CreateViewController<NoFileView>();
                SetViewControllerToNavigationController(_navController, _noFileView);
                _noFileView.SetAction(ShowInitialScreen);
            }
        }

        private async Task ExtractZipAsync(BSFile songInfo, byte[] zip, string customSongsPath, bool overwrite = false)
        {
            Stream zipStream = new MemoryStream(zip);
            try
            {
                ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Read);
                string basePath = songInfo.Key + " (" + songInfo.Name + " - " + songInfo.Uploader + ")";
                basePath = string.Join("", basePath.Split((Path.GetInvalidFileNameChars().Concat(Path.GetInvalidPathChars()).ToArray())));
                string path = customSongsPath + "/" + basePath;
                if (!overwrite && Directory.Exists(path))
                {
                    int pathNum = 1;
                    while (Directory.Exists(path + $" ({pathNum})")) ++pathNum;
                    path += $" ({pathNum})";
                }
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                await Task.Run(() =>
                {
                    foreach (var entry in archive.Entries)
                    {
                        var entryPath = Path.Combine(path, entry.Name);
                        if (overwrite || !File.Exists(entryPath)) 
                            entry.ExtractToFile(entryPath, overwrite);
                    }
                }).ConfigureAwait(false);
                archive.Dispose();
            }
            catch
            {
                return;
            }
            zipStream.Close();
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(this, null, false);
        }
    }
}

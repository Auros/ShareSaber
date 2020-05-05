using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using ShareSaber.Models;
using System;
using System.Threading;

namespace ShareSaber.ViewControllers
{
    [ViewDefinition("ShareSaber.Views.download-query.bsml")]
    [HotReload("C:\\Users\\Aurora\\Programming\\ShareSaber\\Mod\\ShareSaber\\Views\\download-query.bsml")]
    public class DownloadQueryView : BSMLAutomaticViewController
    {
        private BSFile _file;

        private Action _cancel;
        private Action<BSFile> _download;

        private string _inquiry = "Would you like to download?";
        [UIValue("inquiry")]
        public string Inquiry
        {
            get => _inquiry;
            set
            {
                _inquiry = value;
                NotifyPropertyChanged("Inquiry");
            }
        }

        [UIAction("cancel")]
        public void Cancel() =>
            _cancel?.Invoke();

        [UIAction("download")]
        public void Download() =>
            _download?.Invoke(_file);

        public void Setup(BSFile file, Action cancel, Action<BSFile> download)
        {
            _file = file;
            _cancel = cancel;
            _download = download;

            Inquiry = $"Would you like to download <color=#00ffff>{_file.Name}</color> (<color=green>{_file.Type}</color>).";
        }
    }
}

using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using ShareSaber.Models;
using System;
using System.Threading;

namespace ShareSaber.ViewControllers
{
    [ViewDefinition("ShareSaber.Views.downloading.bsml")]
    [HotReload("C:\\Users\\Aurora\\Programming\\ShareSaber\\Mod\\ShareSaber\\Views\\downloading.bsml")]
    public class DownloadingView : BSMLAutomaticViewController
    {
        private CancellationTokenSource _cts;

        private string _progress = "0%";
        [UIValue("progress")]
        public string Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                NotifyPropertyChanged("Progress");
            }
        }

        private bool _downloading = true;
        [UIValue("downloading")]
        public bool Downloading
        {
            get => _downloading;
            set
            {
                _downloading = value;
                NotifyPropertyChanged();
            }
        }

        [UIAction("cancel")]
        public void Cancel() =>
            _cts.Cancel();

        public void SetToken(CancellationTokenSource cts) =>
            _cts = cts;
        
    }
}

using System;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;

namespace ShareSaber.ViewControllers
{
    [ViewDefinition("ShareSaber.Views.no-file.bsml")]
    [HotReload("C:\\Users\\Aurora\\Programming\\ShareSaber\\Mod\\ShareSaber\\Views\\no-file.bsml")]
    public class NoFileView : BSMLAutomaticViewController
    {
        private Action _goBack;

        [UIAction("go-back")]
        public void GoBack()
            => _goBack?.Invoke();

        public void SetAction(Action goBack)
            => _goBack = goBack;
    }
}

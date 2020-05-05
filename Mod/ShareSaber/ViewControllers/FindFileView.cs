using System;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;

namespace ShareSaber.ViewControllers
{
    [ViewDefinition("ShareSaber.Views.find-file.bsml")]
    [HotReload("C:\\Users\\Aurora\\Programming\\ShareSaber\\Mod\\ShareSaber\\Views\\find-file.bsml")]
    public class FindFileView : BSMLAutomaticViewController
    {
        private Action<string> _searched;

        [UIAction("searched")]
        public void Searched(string str)
            => _searched?.Invoke(str);

        public void SetAction(Action<string> search)
            => _searched = search;
    }
}

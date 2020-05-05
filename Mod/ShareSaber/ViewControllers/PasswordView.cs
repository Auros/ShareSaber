using System;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using ShareSaber.Models;

namespace ShareSaber.ViewControllers
{
    [ViewDefinition("ShareSaber.Views.password.bsml")]
    [HotReload("C:\\Users\\Aurora\\Programming\\ShareSaber\\Mod\\ShareSaber\\Views\\password.bsml")]
    public class PasswordView : BSMLAutomaticViewController
    {
        private BSFile _file;

        private Action<string, BSFile> _entered;

        [UIAction("entered")]
        public void Searched(string str)
            => _entered?.Invoke(str, _file);

        public void SetAction(BSFile file, Action<string, BSFile> entered)
        {
            _file = file;
            _entered = entered;
        }
    }
}

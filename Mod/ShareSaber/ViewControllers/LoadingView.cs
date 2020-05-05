using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using System.Threading;

namespace ShareSaber.ViewControllers
{
    [ViewDefinition("ShareSaber.Views.loading.bsml")]
    [HotReload("C:\\Users\\Aurora\\Programming\\ShareSaber\\Mod\\ShareSaber\\Views\\loading.bsml")]
    public class LoadingView : BSMLAutomaticViewController
    {
        private CancellationTokenSource _cts;

        [UIAction("cancel")]
        public void Cancel() =>
            _cts?.Cancel();

        public void SetToken(CancellationTokenSource source) =>
            _cts = source;
    }
}

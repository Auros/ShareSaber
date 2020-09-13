using IPA;
using BeatSaberMarkupLanguage;
using IPALogger = IPA.Logging.Logger;
using BeatSaberMarkupLanguage.MenuButtons;
using System;

namespace ShareSaber
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static IPALogger Log { get; private set; }
        internal static Plugin Instance { get; private set; }
        internal static WebClient Client { get; private set; }
        internal static Version Version => new Version("1.0.1");
        internal static MenuButton MenuButton { get; private set; }
        internal static string ShareSaberAPIURL { get; } = "https://sharesaber.com/";
        internal static ShareSaberFlowCoordinator ShareSaberFlow { get; private set; }

        [Init]
        public Plugin(IPALogger logger)
        {
            Instance = this;
            Log = logger;
        }

        [OnEnable]
        public void Enabled()
        {
            Client = new WebClient();
            MenuButton = new MenuButton("ShareSaber", StartShareSaberUI);
            MenuButtons.instance.RegisterButton(MenuButton);
        }

        [OnDisable]
        public void Disabled()
        {
            Client.Dispose();
            MenuButtons.instance.UnregisterButton(MenuButton);
        }

        public void StartShareSaberUI()
        {
            if (ShareSaberFlow == null)
                ShareSaberFlow = BeatSaberUI.CreateFlowCoordinator<ShareSaberFlowCoordinator>();
            BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(ShareSaberFlow);
        }
    }
}

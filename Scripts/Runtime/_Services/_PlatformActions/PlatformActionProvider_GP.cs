#if GAMEPUSH
using _Data;
using GamePush;
using Zenject;

namespace _Services._PlatformActions {
    public class PlatformActionProvider_GP : IPlatformActionProvider {
        [Inject] private ProjectSettings _projectSettings;

        private PlatformActionProvider_GP(ProjectSettings settings) {
            _projectSettings = settings;
        }

        public void Initialize() {
            PlatformConfig = _projectSettings.GetAdConfig(GetCurrentPlatform());
        }
        
        public PlatformConfig PlatformConfig { get; private set; }

        public string GetISO() {
            return GP_Language.CurrentISO();
        }

        public void CallGameReadyAPI() {
            GP_Game.GameReady();
        }

        public void CallGameplayStart() {
            GP_Game.GameplayStart();
        }

        public void CallGameplayEnd() {
            GP_Game.GameplayStop();
        }

        public void SetMinimalDelayForInterstitial() {
            //controlled from GP dashboard
        }

        public bool IsRemoteConfigSupported() {
            return true;
        }

        public SupportedPlatform GetCurrentPlatform() {
            // Переводим GamePush.Platform в наш SupportedPlatform
            return GP_Platform.Type() switch {
                Platform.YANDEX => SupportedPlatform.Yandex,
                Platform.VK => SupportedPlatform.VK,
                Platform.CRAZY_GAMES => SupportedPlatform.CrazyGames,
                Platform.POKI => SupportedPlatform.Poki,
                Platform.GAME_DISTRIBUTION => SupportedPlatform.GameDistribution,
                Platform.GAMEPIX => SupportedPlatform.GamePix,
                Platform.CUSTOM => SupportedPlatform.Playgama,
                _ => SupportedPlatform.Unknown
            };
        }
    }
}
#endif
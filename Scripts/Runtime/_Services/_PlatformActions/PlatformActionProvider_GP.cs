using _Data;
using _Services._PlatformActions;
using GamePush;

namespace __CoreGameLib._Scripts {
    public class PlatformActionProvider_GP : IPlatformActionProvider {

        private PlatformActionProvider_GP(ProjectSettings settings) {
            PlatformConfig = settings.GetAdConfig(GetCurrentPlatform());
        }
        
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

        public PlatformConfig PlatformConfig { get; }

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
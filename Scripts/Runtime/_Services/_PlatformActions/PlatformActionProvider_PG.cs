#if PLAYGAMA
// Файл: Core/Services/Lang/PlatformActionProvider_PG.cs

using _Data;
using _Services._PlatformActions;
using Playgama;
using Playgama.Modules.Platform;
using Zenject;

namespace _Infrastructure {
    public class PlatformActionProvider_PG : IPlatformActionProvider {

        [Inject]
        private PlatformActionProvider_PG(ProjectSettings settings) {
            PlatformConfig = settings.GetAdConfig(GetCurrentPlatform());
        }
        
        public PlatformConfig PlatformConfig { get; }
        
        public SupportedPlatform GetCurrentPlatform() {
            // Playgama возвращает string. Переводим его в наш SupportedPlatform.
            // Строки нужно сверять с официальной документацией Playgama
            string platformId = Bridge.platform.id?.ToLower() ?? "";
            
            return platformId switch {
                "yandex" => SupportedPlatform.Yandex,
                "vk" => SupportedPlatform.VK,
                "crazy_games" => SupportedPlatform.CrazyGames,
                "poki" => SupportedPlatform.Poki,
                "game_distribution" => SupportedPlatform.GameDistribution,
                "gamepix" => SupportedPlatform.GamePix,
                "playgama" => SupportedPlatform.Playgama,
                _ => SupportedPlatform.Unknown
            };
        }

        public string GetISO() {
            return Bridge.platform.language;
        }

        public void CallGameReadyAPI() {
            Bridge.platform.SendMessage(PlatformMessage.GameReady);
        }

        public void CallGameplayStart() {
#pragma warning disable 0612, 0618
            Bridge.platform.SendMessage(PlatformMessage.GameplayStarted);
#pragma warning restore 0612, 0618
        }

        public void CallGameplayEnd() {
#pragma warning disable 0612, 0618
            Bridge.platform.SendMessage(PlatformMessage.GameplayStopped);
#pragma warning restore 0612, 0618
        }

        public void SetMinimalDelayForInterstitial() {
            // Delay is controlled via ProjectSettings or platform dashboard
        }

        public bool IsRemoteConfigSupported() {
            return Bridge.remoteConfig.isSupported;
        }

    }
}
#endif

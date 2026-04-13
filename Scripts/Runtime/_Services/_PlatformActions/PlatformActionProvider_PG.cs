// Файл: Core/Services/Lang/PlatformActionProvider_PG.cs

using System;
using _Services._PlatformActions;
using _Services._ScriptableObjects;
using Playgama;

namespace _Infrastructure {
    public class PlatformActionProvider_PG : IPlatformActionProvider {
        
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
                _ => SupportedPlatform.Unknown
            };
        }

        public string GetISO() {
            throw new NotImplementedException();
        }

        public void CallGameReadyAPI() {
            throw new NotImplementedException();
        }

        public void SetMinimalDelayForInterstitial() {
            throw new NotImplementedException();
        }

        public bool IsRemoteConfigSupported() {
            throw new NotImplementedException();
        }
    }
}
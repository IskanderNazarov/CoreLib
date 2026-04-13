// Файл: Core/Services/ScriptableObjects/ProjectSettings.cs

using System;
using System.Collections.Generic;
using core.ads;
using UnityEngine;

namespace _Data {
    
    // 1. Твой единый язык платформ
    public enum SupportedPlatform {
        Unknown,
        Yandex,
        VK,
        CrazyGames,
        Poki,
        GameDistribution,
        GamePix
    }

    [Serializable]
    public class PlatformAdConfig {
        public SupportedPlatform platform;
        public AdPlacementType allowedPlacements = AdPlacementType.All;
    }

    [CreateAssetMenu(fileName = "ProjectSettings", menuName = "ScriptableObjects/ProjectSettings", order = 10)]
    public class ProjectSettings : ScriptableObject {
        
        [Header("SDK Selection")]
        public SDK_Type SDKType = SDK_Type.Playgama;
        //public string[] PublicKeysFor_GP;
        
        [Header("Ads Global Settings (Playgama Only)")]
        public int FirstInterstitialTime = 60;
        public int minimumDelayBetweenInterstitial = 60;

        [Header("Ads Placements Config")]
        public PlatformAdConfig defaultAdConfig; // Если платформа не найдена
        public List<PlatformAdConfig> platformAdConfigs = new List<PlatformAdConfig>();

        // Метод для быстрого получения конфига
        public PlatformAdConfig GetAdConfig(SupportedPlatform currentPlatform) {
            var config = platformAdConfigs.Find(c => c.platform == currentPlatform);
            return config ?? defaultAdConfig;
        }
    }
    
    public enum SDK_Type {
        Playgama, 
        GamePush
    }
}
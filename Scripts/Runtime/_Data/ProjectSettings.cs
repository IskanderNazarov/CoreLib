using System;
using System.Collections.Generic;
using _Services._AdsService;
using _Services._Localization;
#if GAMEPUSH
using GamePush;
#endif
using UnityEngine;
using UnityEngine.Serialization;

namespace _Data {
    
    // 1. Твой единый язык платформ
    public enum SupportedPlatform {
        Unknown,
        Yandex,
        VK,
        CrazyGames,
        Poki,
        GameDistribution,
        GamePix,
        Playgama,
        GameMonetize
    }

    [Serializable]
    public class PlatformConfig {
        public SupportedPlatform platform;
        public AdPlacementType allowedPlacements = AdPlacementType.All;
        public string leaderboardID;
    }

    [CreateAssetMenu(fileName = "ProjectSettings", menuName = "Data/ProjectSettings", order = 10)]
    public class ProjectSettings : ScriptableObject {
        
        [Header("SDK Selection")]
        public SDK_Type SDKType = SDK_Type.Playgama;
        //public string[] PublicKeysFor_GP;
        
        [Header("Ads Global Settings (Playgama Only)")]
        public int FirstInterstitialTime = 60;
        public int minimumDelayBetweenInterstitial = 60;

        [FormerlySerializedAs("defaultAdConfig")] [Header("Ads Placements Config")]
        public PlatformConfig defaultConfig; // Если платформа не найдена
        public List<PlatformConfig> platformAdConfigs = new List<PlatformConfig>();
        
        public LocalesSettings LocalesSettings;

        [Header("Analytics Settings")]
        [Tooltip("If enabled, events will be sent directly to Yandex Metrika via jslib bridge with full parameters.")]
        public bool UseDirectYandexMetricaBridge = false;
        [Tooltip("Optional custom Yandex Metrika Counter ID (leave 0 to use window.yandexMetricaCounterId or auto-detected).")]
        public long YandexMetricaCounterId = 0;

        [Header("Editor Live Events Testing")]
        [Tooltip("Controls how live events behave when running inside Unity Editor.")]
        public EditorEventMode EditorEventMode = EditorEventMode.Disabled;
        [Tooltip("Event ID used in Unity Editor when EditorEventMode is MockActiveEvent (e.g., halloween_2026, new_year_2027).")]
        public string EditorTestEventId = "";

        // Метод для быстрого получения конфига
        public PlatformConfig GetAdConfig(SupportedPlatform currentPlatform) {
            var config = platformAdConfigs.Find(c => c.platform == currentPlatform);
            return config ?? defaultConfig;
        }
    }
    
    public enum SDK_Type {
        Playgama, 
        GamePush
    }

    public enum EditorEventMode {
        Disabled,
        MockActiveEvent
    }
}

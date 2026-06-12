using System;

namespace core.ads {
    [Flags]
    public enum AdPlacementType {
        None = 0,
        AfterGameAction = 1 << 0,
        AfterLogicPause = 1 << 1,
        All = ~0
    }

    public interface IAdsService {
        event Action OnAdStart;
        event Action OnResumeToGameAfterAd;
        
        // Добавлен placementType
        void Initialize();
        void ShowInterstitial(AdPlacementType placementType, Action onAdClosed);
        void ShowRewarded(Action onRewardGranted, Action onAdClosed);
    }
}
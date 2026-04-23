using System;

namespace core.ads {
    public class AdsService_Editor:IAdsService {
        public event Action OnAdStart;
        public event Action OnResumeToGameAfterAd;
        public void ShowInterstitial(AdPlacementType placementType, Action onAdClosed) {
            OnAdStart?.Invoke();
        }

        public void ShowRewarded(Action onRewardGranted, Action onAdClosed) {
            onRewardGranted?.Invoke();
            onAdClosed?.Invoke();
        }
    }
}
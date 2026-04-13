// Файл: Core/Ads/AdsService_GP.cs
using System;
using _Data;
using _Services._PlatformActions;
using GamePush;
using UnityEngine;
using Zenject;

namespace core.ads {
    public class AdsService_GP : IAdsService {
        private bool _isAdShowing;
        
        private readonly ProjectSettings _projectSettings;
        private readonly IPlatformActionProvider _platformProvider;
        private PlatformAdConfig _platformConfig;

        [Inject]
        public AdsService_GP(ProjectSettings projectSettings, IPlatformActionProvider platformProvider) {
            _projectSettings = projectSettings;
            _platformProvider = platformProvider;
        }

        public void Initialize() {
            SupportedPlatform currentPlatform = _platformProvider.GetCurrentPlatform(); 
            _platformConfig = _projectSettings.GetAdConfig(currentPlatform);
        }

        public event Action OnAdStart;
        public event Action OnResumeToGameAfterAd;

        public void ShowInterstitial(AdPlacementType placementType, Action onAdClosed) {
            if (_isAdShowing) {
                onAdClosed?.Invoke();
                return;
            }

#if UNITY_EDITOR
            onAdClosed?.Invoke();
            return;
#endif

            // 1. Проверяем разрешено ли показывать рекламу в этот момент для этой площадки
            if (!_platformConfig.allowedPlacements.HasFlag(placementType)) {
                onAdClosed?.Invoke();
                return;
            }

            // 2. Делегируем проверку таймеров самому GamePush
            if (!GP_Ads.IsFullscreenAvailable()) {
                onAdClosed?.Invoke();
                return;
            }

            _isAdShowing = true;

            GP_Ads.ShowFullscreen(
                onFullscreenStart: () => { PauseGame(); },
                onFullscreenClose: (success) => {
                    _isAdShowing = false;
                    ResumeGame();
                    onAdClosed?.Invoke();
                }
            );
        }

        public void ShowRewarded(Action onRewardGranted, Action onAdClosed) {
            if (_isAdShowing) return;

#if UNITY_EDITOR
            onRewardGranted?.Invoke();
            onAdClosed?.Invoke();
            return;
#endif

            if (!GP_Ads.IsRewardedAvailable()) {
                onAdClosed?.Invoke();
                return;
            }

            _isAdShowing = true;

            GP_Ads.ShowRewarded(
                idOrTag: "REWARD",
                onRewardedReward: (tag) => { onRewardGranted?.Invoke(); },
                onRewardedStart: () => { PauseGame(); },
                onRewardedClose: (success) => {
                    _isAdShowing = false;
                    ResumeGame();
                    onAdClosed?.Invoke();
                }
            );
        }

        private void PauseGame() {
            OnAdStart?.Invoke();
            Time.timeScale = 0;
            AudioListener.pause = true; // Глушим весь звук на время рекламы
        }

        private void ResumeGame() {
            Time.timeScale = 1;
            AudioListener.pause = false;
            OnResumeToGameAfterAd?.Invoke();
        }
    }
}
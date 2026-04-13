// Файл: Core/Ads/AdsService_PG.cs

using System;
using _Services._PlatformActions;
using _Services._ScriptableObjects;
using Playgama;
using Playgama.Modules.Advertisement;
using UnityEngine;
using Zenject;

namespace core.ads {
    public class AdsService_PG : IInitializable, IAdsService, IDisposable {
        private bool _isAdShowing = false;
        private DateTime _sessionStartTime;
        private DateTime _lastAdTime;

        private Action _onInterstitialClosed;
        private Action _onRewardGranted;
        private Action _onRewardedAdClosed;

        private readonly ProjectSettings _projectSettings;
        private readonly IPlatformActionProvider _platformProvider;
        private PlatformAdConfig _platformConfig;

        [Inject]
        public AdsService_PG(ProjectSettings projectSettings, IPlatformActionProvider platformProvider) {
            _projectSettings = projectSettings;
            _platformProvider = platformProvider;
        }

        public void Initialize() {
            _sessionStartTime = DateTime.Now;
            _lastAdTime = DateTime.MinValue; 

            SupportedPlatform currentPlatform = _platformProvider.GetCurrentPlatform();
            _platformConfig = _projectSettings.GetAdConfig(currentPlatform);

            Bridge.advertisement.interstitialStateChanged += OnInterstitialStateChanged;
            Bridge.advertisement.rewardedStateChanged += OnRewardedStateChanged;
        }

        public void Dispose() {
            Bridge.advertisement.interstitialStateChanged -= OnInterstitialStateChanged;
            Bridge.advertisement.rewardedStateChanged -= OnRewardedStateChanged;
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

            // 1. Проверяем разрешенные плейсменты из ProjectSettings
            if (!_platformConfig.allowedPlacements.HasFlag(placementType)) {
                onAdClosed?.Invoke();
                return;
            }

            // 2. Локальная проверка таймеров (т.к. Playgama берет настройки из ProjectSettings)
            var timeSinceStart = (float)DateTime.Now.Subtract(_sessionStartTime).TotalSeconds;
            
            // Защита от первого вызова: если рекламы еще не было, проверяем только timeSinceStart
            bool isFirstAd = _lastAdTime == DateTime.MinValue;
            var timeSinceLastAd = isFirstAd ? float.MaxValue : (float)DateTime.Now.Subtract(_lastAdTime).TotalSeconds;

            if (timeSinceStart < _projectSettings.FirstInterstitialTime || 
                timeSinceLastAd < _projectSettings.minimumDelayBetweenInterstitial) {
                onAdClosed?.Invoke();
                return;
            }

            _onInterstitialClosed = onAdClosed;
            Bridge.advertisement.ShowInterstitial();
        }

        private void OnInterstitialStateChanged(InterstitialState state) {
            switch (state) {
                case InterstitialState.Opened:
                    _isAdShowing = true;
                    PauseGame();
                    break;
                case InterstitialState.Closed:
                case InterstitialState.Failed:
                    if (!_isAdShowing) return;

                    // Обновляем таймер только если показ действительно состоялся или провалился после открытия
                    _lastAdTime = DateTime.Now; 
                    ResumeGame();
                    
                    var callback = _onInterstitialClosed;
                    _onInterstitialClosed = null; // Очищаем ДО вызова, защита от двойного клика
                    _isAdShowing = false;
                    
                    callback?.Invoke();
                    break;
            }
        }

        public void ShowRewarded(Action onRewardGranted, Action onAdClosed) {
            if (_isAdShowing) {
                onAdClosed?.Invoke();
                return;
            }

#if UNITY_EDITOR
            onRewardGranted?.Invoke();
            onAdClosed?.Invoke();
            return;
#endif

            _onRewardGranted = onRewardGranted;
            _onRewardedAdClosed = onAdClosed;

            Bridge.advertisement.ShowRewarded();
        }

        private void OnRewardedStateChanged(RewardedState state) {
            switch (state) {
                case RewardedState.Opened:
                    _isAdShowing = true;
                    PauseGame();
                    break;
                case RewardedState.Rewarded:
                    _onRewardGranted?.Invoke();
                    break;
                case RewardedState.Closed:
                case RewardedState.Failed:
                    if (!_isAdShowing) return;

                    ResumeGame();
                    
                    var callback = _onRewardedAdClosed;
                    _onRewardGranted = null;
                    _onRewardedAdClosed = null;
                    _isAdShowing = false;
                    
                    callback?.Invoke();
                    break;
            }
        }

        private void PauseGame() {
            OnAdStart?.Invoke();
            Time.timeScale = 0;
            AudioListener.pause = true;
        }

        private void ResumeGame() {
            Time.timeScale = 1;
            AudioListener.pause = false;
            OnResumeToGameAfterAd?.Invoke();
        }
    }
}
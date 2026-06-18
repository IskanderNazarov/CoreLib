using System.Collections;
using core.ads;
using UnityEngine;
using Zenject;

namespace Core._Services {
    public class AdsCallbacksHelper : MonoBehaviour {
        [Inject] private IAdsService _adsService;
        [Inject] private SoundManager _soundManager;

        private void Start() {
            _adsService.OnAdStart -= OnAdStart;
            _adsService.OnAdStart += OnAdStart;
            _adsService.OnResumeToGameAfterAd -= OnResumeToGameAfterAd;
            _adsService.OnResumeToGameAfterAd += OnResumeToGameAfterAd;
        }

        private void OnDestroy() {
            if (_adsService != null) {
                _adsService.OnAdStart -= OnAdStart;
                _adsService.OnResumeToGameAfterAd -= OnResumeToGameAfterAd;
            }
        }

        private void OnAdStart() {
            OnGameShouldPause(true);
        }

        private void OnResumeToGameAfterAd() {
            OnGameShouldPause(false);
            
            // Force refresh music state to ensure bg music restarts if it was stopped by the ad or browser
            if (_soundManager.IsMusicOn) {
                _soundManager.SetMusicOn(true);
            }
        }

        private void OnGameShouldPause(bool pause) {
            _soundManager.MuteSounds(pause);
        }
    }
}

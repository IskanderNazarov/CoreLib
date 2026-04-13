using System;
using Core._Services.SoundManagement;
using Hellmade.Sound;
using Zenject;

namespace Core._Services {
    public class SoundManager {
        public Action<bool> OnMusicStateChanged;
        public Action<bool> OnSoundStateChanged;

        private readonly ISoundStateProvider _stateProvider;

        [Inject]
        public SoundManager(ISoundStateProvider stateProvider) {
            // Если игра ничего не передала, ставим заглушку по умолчанию
            _stateProvider = stateProvider;
        }

        public void MuteSounds(bool mute) {
            EazySoundManager.GlobalVolume = mute ? 0 : 1.0f;
        }

        // Обновленный метод. Принимает overridePitch для безопасного изменения питча ракет
        private Audio PlayGameAudio(bool canPlay, SoundInfo info, float? overridePitch = null) {
            if (!canPlay || info == null || info.clip == null) return null;

            var id = EazySoundManager.PlaySound(info.clip, info.volume);
            var audio = EazySoundManager.GetAudio(id);

            if (audio != null) {
                audio.Loop = info.loop;
                audio.Pitch = overridePitch ?? info.pitch; // Если передали кастомный питч - используем его
            }

            return audio;
        }

        public Audio PlayMusic(SoundInfo info) => PlayGameAudio(_stateProvider.IsMusicOn, info);

        public Audio PlaySound(SoundInfo info, float? overridePitch = null) => PlayGameAudio(_stateProvider.IsSoundOn, info, overridePitch);

        public void StopAllSounds() => EazySoundManager.StopAllSounds();

        // Удобные геттеры
        public bool IsSoundOn => _stateProvider.IsSoundOn;
        public bool IsMusicOn => _stateProvider.IsMusicOn;

        public void SetSFXOn(bool isOn) {
            _stateProvider.IsSoundOn = isOn;
            OnSoundStateChanged?.Invoke(isOn);
        }

        public void SetMusicOn(bool isOn) {
            _stateProvider.IsMusicOn = isOn;
            OnMusicStateChanged?.Invoke(isOn);
        }
    }
}
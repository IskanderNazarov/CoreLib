using System;
using UnityEngine;

namespace Core._Services.SoundManagement {
    [Serializable]
    public class SoundInfo {
        public AudioClip clip;
        [Range(0, 1f)] public float volume = 1f;
        [Range(-3f, 3f)] public float pitch = 1f; // Добавили питч
        public bool loop = false;
        public float delay = 0f;
    }
}
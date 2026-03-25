using System;
using UnityEngine;

namespace Scriptable {
    namespace SoundManaging {
        [Serializable]
        public class SoundInfo {
            public AudioClip clip;
            [Range(0, 1)] public float volume;
            public bool loop;
            public float delay;

            public SoundInfo() {
                volume = 1;
            }
        }
    }
}
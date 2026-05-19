// File: Scripts/Runtime/_Services/_Saving/DataSaver_GP.cs

using System;
using System.Collections;
using Core._Services._Saving;
using GamePush;
using UnityEngine;
using UnityEngine.Events;

namespace __CoreGameLib._Scripts._Services._Saving {
    public class DataSaver_GP : IDataSaver {
        private const float LOAD_TIMEOUT = 5.0f; // max wait time

        public IEnumerator Load(string key, Action<string> onLoaded) {
            var isDone = false;

            UnityAction onComplete = () => { isDone = true; };
            UnityAction onError = () => { 
                Debug.LogWarning("// DataSaver_GP: Load error callback"); 
                isDone = true; 
            };

            GP_Player.OnLoadComplete += onComplete;
            GP_Player.OnLoadError += onError;

            GP_Player.Load();

            // wait until done or timeout reached
            float elapsedTime = 0f;
    while (!isDone && elapsedTime < LOAD_TIMEOUT) {
        // Используем unscaledDeltaTime на случай, если игра в момент загрузки стоит на паузе (Time.timeScale == 0)
        elapsedTime += Time.unscaledDeltaTime;
        yield return null; // Ждем следующий кадр
    }

            GP_Player.OnLoadComplete -= onComplete;
            GP_Player.OnLoadError -= onError;

            if (!isDone) {
                Debug.LogWarning($"// DataSaver_GP: Load timed out after {LOAD_TIMEOUT}s");
            }

            var loadedString = GP_Player.GetString(key);
            onLoaded?.Invoke(loadedString);
        }

        public void Save(string key, string json) {
            GP_Player.Set(key, json);
            GP_Player.Sync();
        }

        public void Delete(string key) {
            GP_Player.Set(key, "");
            GP_Player.Sync();
        }
    }
}

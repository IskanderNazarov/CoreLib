using System;
using System.Collections;
using Core._Services._Saving;
using Playgama;
using Playgama.Modules.Storage;
using UnityEngine;

namespace _Services._Saving {
    public class DataSaver_PG : IDataSaver {
        
        public IEnumerator Load(string key, Action<string> onLoaded) {
            bool isDone = false;
            string loadedString = null;

            // Запрашиваем 1 ключ (например, "main_save")
            Bridge.storage.Get(key, (success, value) => {
                if (success && !string.IsNullOrEmpty(value)) {
                    loadedString = value; // Получили наш JSON
                }
                isDone = true;
            }, GetStorageType());

            yield return new WaitUntil(() => isDone);
            onLoaded?.Invoke(loadedString);
        }

        public void Save(string key, string json) {
            // Просто отдаем строку платформе
            Bridge.storage.Set(key, json, (success) => {
                if (!success) Debug.LogWarning($"[DataSaver_PG] Failed to save {key}");
            }, GetStorageType());
        }

        public void Delete(string key) {
            Bridge.storage.Delete(key, null, GetStorageType());
        }

        private StorageType GetStorageType() {
#if UNITY_EDITOR
            return StorageType.LocalStorage;
#else
            return Bridge.player.isAuthorized ? StorageType.PlatformInternal : StorageType.LocalStorage;
#endif
        }
    }
}
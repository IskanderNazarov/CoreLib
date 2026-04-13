using System;
using System.Collections;
using Core._Services._Saving;
using GamePush;
using UnityEngine;
using UnityEngine.Events;

namespace __CoreGameLib._Scripts._Services._Saving {
    public class DataSaver_GP : IDataSaver {
        
        public IEnumerator Load(string key, Action<string> onLoaded) {
            bool isDone = false;

            UnityAction onComplete = () => { isDone = true; };
            UnityAction onError = () => { 
                Debug.LogWarning("[DataSaver_GP] Load error"); 
                isDone = true; 
            };

            GP_Player.OnLoadComplete += onComplete;
            GP_Player.OnLoadError += onError;

            GP_Player.Load(); // Загружаем профиль игрока из сети

            yield return new WaitUntil(() => isDone);

            GP_Player.OnLoadComplete -= onComplete;
            GP_Player.OnLoadError -= onError;

            // Достаем JSON из поля GamePush
            string loadedString = GP_Player.GetString(key);
            onLoaded?.Invoke(loadedString);
        }

        public void Save(string key, string json) {
            GP_Player.Set(key, json);
            GP_Player.Sync(); // Синхронизируем. Защита от спама будет в SaveManager!
        }

        public void Delete(string key) {
            GP_Player.Set(key, "");
            GP_Player.Sync();
        }
    }
}
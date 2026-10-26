#if GAMEPUSH
// File: Scripts/Runtime/_Services/_Saving/DataSaver_GP.cs

using Cysharp.Threading.Tasks;
using GamePush;
using GamePush.Data;
using UnityEngine;

namespace _Services._Saving {
    public class DataSaver_GP : IDataSaver {
        private const float LOAD_TIMEOUT = 5.0f; // max wait time

        public async UniTask<string> Load(string key) {
            // К моменту GP_Init.isReady SDK GamePush уже загрузил и синхронизировал данные
            // из выбранного хранилища (например, Облако площадки + Локальное хранилище).
            // Принудительный вызов GP_Player.Load() не делаем, чтобы:
            // 1) не тратить платные запросы к серверу GamePush;
            // 2) не перезатирать непубличные поля пустыми значениями с сервера GamePush;
            // 3) избежать 5-секундного таймаута при сетевых задержках.
            if (!GP_Init.isReady) {
                float elapsedTime = 0f;
                while (!GP_Init.isReady && elapsedTime < LOAD_TIMEOUT) {
                    elapsedTime += Time.unscaledDeltaTime;
                    await UniTask.Yield();
                }

                if (!GP_Init.isReady) {
                    Debug.LogWarning($"// DataSaver_GP: GP_Init.isReady timed out after {LOAD_TIMEOUT}s");
                }
            }

            return GP_Player.GetString(key);
        }

        private void PrintPlayerField(PlayerField pf) {

            Debug.Log($"pf.key: {pf.key}");
            Debug.Log($"pf.name: {pf.name}");
            Debug.Log($"pf.@default: {pf.@default}");
            Debug.Log($"Variants");
            foreach (var pfVariant in pf.variants) {
                Debug.Log($"pfVariant.name: {pfVariant.name} -->> pfVariant.key: {pfVariant.value}");
            }
        }

        private void PrintPlayerFieldData(PlayerFieldData pf) {

            Debug.Log($"pfd.key: {pf.key}");
            Debug.Log($"pfd.name: {pf.name}");
            Debug.Log($"Variants");
            foreach (var pfVariant in pf.variants) {
                Debug.Log($"pfdVariant.name: {pfVariant.name} -->> pfdVariant.key: {pfVariant.value}");
            }
            Debug.Log("-----------");
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

#endif

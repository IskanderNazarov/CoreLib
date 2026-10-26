#if GAMEPUSH
// File: Scripts/Runtime/_Services/_Saving/DataSaver_GP.cs

using Cysharp.Threading.Tasks;
using GamePush;
using GamePush.Data;
using UnityEngine;

namespace _Services._Saving {
    public class DataSaver_GP : IDataSaver {
        private const float LOAD_TIMEOUT = 5.0f; // max wait time
        private static bool _eventsSubscribed;

        public DataSaver_GP() {
            EnsureEventsSubscribed();
        }

        private static void EnsureEventsSubscribed() {
            if (_eventsSubscribed) return;
            _eventsSubscribed = true;

            GP_Player.OnSyncComplete += () => {
                Debug.Log($"[SAVE_DIAG] [GP_Event] OnSyncComplete | PlayerID={GP_Player.GetID()} | Credentials='{GP_Player.GetString("credentials")}' | IsLoggedIn={GP_Player.IsLoggedIn()} | UpdatedAt='{GP_Player.GetString("updatedAt")}'");
            };
            GP_Player.OnSyncError += () => {
                Debug.LogError($"[SAVE_DIAG] [GP_Event] OnSyncError | PlayerID={GP_Player.GetID()} | Credentials='{GP_Player.GetString("credentials")}' | IsLoggedIn={GP_Player.IsLoggedIn()}");
            };
            GP_Player.OnLoadComplete += () => {
                Debug.Log($"[SAVE_DIAG] [GP_Event] OnLoadComplete | PlayerID={GP_Player.GetID()} | Credentials='{GP_Player.GetString("credentials")}' | UpdatedAt='{GP_Player.GetString("updatedAt")}'");
            };
            GP_Player.OnLoadError += () => {
                Debug.LogError($"[SAVE_DIAG] [GP_Event] OnLoadError | PlayerID={GP_Player.GetID()} | Credentials='{GP_Player.GetString("credentials")}'");
            };
        }

        public async UniTask<string> Load(string key) {
            EnsureEventsSubscribed();

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
                    Debug.LogWarning($"[SAVE_DIAG] [DataSaver_GP.Load] GP_Init.isReady timed out after {LOAD_TIMEOUT}s for key='{key}'");
                }
            }

            string rawValue = GP_Player.GetString(key);

            Debug.Log(
                $"[SAVE_DIAG] [DataSaver_GP.Load] key='{key}' | GP_Init.isReady={GP_Init.isReady} | " +
                $"Platform='{GP_Platform.TypeAsString()}' (CloudSaves={GP_Platform.IsSupportsCloudSaves()}) | " +
                $"PlayerID={GP_Player.GetID()} | Name='{GP_Player.GetName()}' | " +
                $"IsLoggedIn={GP_Player.IsLoggedIn()} | HasCredentials={GP_Player.HasAnyCredentials()} | IsStub={GP_Player.IsStub()} | " +
                $"Credentials='{GP_Player.GetString("credentials")}' | UpdatedAt='{GP_Player.GetString("updatedAt")}' | " +
                $"URL='{Application.absoluteURL}' | RawLen={(rawValue != null ? rawValue.Length : 0)} | Raw='{rawValue}'"
            );

            return rawValue;
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
            EnsureEventsSubscribed();
            Debug.Log(
                $"[SAVE_DIAG] [DataSaver_GP.Save] key='{key}' | PlayerID={GP_Player.GetID()} | " +
                $"Credentials='{GP_Player.GetString("credentials")}' | IsLoggedIn={GP_Player.IsLoggedIn()} | " +
                $"JsonLen={(json != null ? json.Length : 0)} | Json='{json}'"
            );
            GP_Player.Set(key, json);
            GP_Player.Sync();
        }

        public void Delete(string key) {
            EnsureEventsSubscribed();
            Debug.Log($"[SAVE_DIAG] [DataSaver_GP.Delete] key='{key}' | PlayerID={GP_Player.GetID()}");
            GP_Player.Set(key, "");
            GP_Player.Sync();
        }
    }
}

#endif

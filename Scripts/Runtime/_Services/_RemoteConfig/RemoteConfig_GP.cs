// Файл: RemoteConfig_GP.cs
using System.Collections;
using System.Collections.Generic;
using _Services._Saving;
using GamePush;
using UnityEngine;
// Для RCKeysStorage
// Для IKeysStorage

namespace __CoreGameLib._Scripts._Services._RemoteConfig {
    public class RemoteConfig_GP : IRemoteConfig {

        private Dictionary<string, string> _configCache;
        private bool _isFetchCompleted;
        private IKeysStorage _keysStorage;

        public IEnumerator LoadConfigs(IKeysStorage keysStorage, bool loadPlatformVariables = false) {
            _keysStorage = keysStorage;
            InitializeDefaults();

            _isFetchCompleted = false;

            // 1. Если мы хотим загрузить переменные с платформы И платформа это поддерживает
            if (loadPlatformVariables && GP_Variables.IsPlatformVariablesAvailable()) {
                Debug.Log("RemoteConfig_GP: Запрашиваем переменные с ПЛАТФОРМЫ (Яндекс/VK и т.д.)...");
                GP_Variables.FetchPlatformVariables(OnPlatformFetchSuccess, OnPlatformFetchError);
            }
            // 2. Иначе используем переменные из самого GamePush
            else {
                Debug.Log("RemoteConfig_GP: Запрашиваем переменные из GamePush (Платформенные пропущены или не поддерживаются).");
                // Так как галочка "Загружать при старте" включена, данные уже в памяти.
                // Но мы вызываем Fetch для гарантии и чтобы получить их удобным списком в коллбэке.
                GP_Variables.Fetch(OnGamePushFetchSuccess, OnGamePushFetchError);
            }

            // 3. Ждем ответа (с защитой от вечного зависания)
            float timeout = 2.0f; // 5 секунд на ожидание
#if UNITY_EDITOR
            timeout = 0.1f;
  #endif
            while (!_isFetchCompleted && timeout > 0) {
                timeout -= Time.unscaledDeltaTime;
                yield return null;
            }

            if (timeout <= 0) {
                Debug.LogWarning("RemoteConfig_GP: Превышено время ожидания загрузки конфигов. Используем дефолтные значения.");
            }
        }

        public string GetValue(string key) {
            // Ищем ключ в кэше (в котором лежат либо загруженные переменные, либо дефолтные)
            if (_configCache != null && _configCache.TryGetValue(key, out var value)) {
                return value;
            }

            // Fallback
            Debug.LogWarning($"RemoteConfig_GP: Ключ '{key}' не найден в кэше. Пробуем получить из defaults.");
            return _keysStorage.GetDefaultValue<object>(key)?.ToString();
        }

        // --- Внутренние методы ---

        private void InitializeDefaults() {
            _configCache = new Dictionary<string, string>();
            foreach (var kv in _keysStorage.GetDefaultValues()) {
                _configCache[kv.Key] = kv.Value.ToString();
            }
        }

        // --- Коллбэки Платформенных Переменных (Яндекс, VK) ---
        private void OnPlatformFetchSuccess(Dictionary<string, string> variables) {
            Debug.Log($"RemoteConfig_GP: Переменные платформы успешно получены. Количество: {variables.Count}");
            foreach (var kvp in variables) {
                _configCache[kvp.Key] = kvp.Value;
            }
            _isFetchCompleted = true;
        }

        private void OnPlatformFetchError(string error) {
            Debug.LogWarning($"RemoteConfig_GP: Ошибка загрузки переменных платформы. Остаются дефолтные значения., error:{error}");
            _isFetchCompleted = true;
        }

        // --- Коллбэки Переменных GamePush ---
        private void OnGamePushFetchSuccess(List<VariablesData> variables) {
            Debug.Log($"RemoteConfig_GP: Переменные GamePush успешно получены. Количество: {variables.Count}");
            foreach (var variable in variables) {
                _configCache[variable.key] = variable.value;
            }
            _isFetchCompleted = true;
        }

        private void OnGamePushFetchError() {
            Debug.LogWarning("RemoteConfig_GP: Ошибка загрузки переменных GamePush. Остаются дефолтные значения.");
            _isFetchCompleted = true;
        }
    }
}

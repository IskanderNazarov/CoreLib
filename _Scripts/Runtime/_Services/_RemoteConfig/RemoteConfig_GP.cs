using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePush;
using _Infrastructure; // Для RCKeysStorage
using _Services._Saving; // Для IKeysStorage

namespace __CoreGameLib._Scripts._Services._RemoteConfig {
    public class RemoteConfig_GP : IRemoteConfig {
        
        // Локальный кэш конфигурации (Ключ -> Значение)
        private Dictionary<string, string> _configCache;
        
        // Флаг завершения сетевого запроса
        private bool _isFetchCompleted;
        private IKeysStorage _keysStorage;

        public IEnumerator LoadConfigs(IKeysStorage  keysStorage) {
            Debug.Log($"RemoteConfig_GP, keysStorage: == null {keysStorage == null}");
            // 1. Инициализируем кэш дефолтными значениями
            _keysStorage = keysStorage;
            InitializeDefaults();
            
            _isFetchCompleted = false;

/*#if UNITY_EDITOR
            // В редакторе GP_Variables.Fetch (судя по вашему коду) не возвращает колбэки, 
            // поэтому просто используем дефолтные значения и выходим, чтобы не зависнуть.
            Debug.Log("RemoteConfig_GP: Editor mode, using default values.");
            yield break;
#endif*/

            // 2. Запрашиваем актуальные данные у сервера
            // GamePush возвращает список переменных (VariablesData)
            GP_Variables.Fetch(OnFetchSuccess, OnFetchError);

            // 3. Ждем ответа или тайм-аута
            float timeout = 5.0f; 
            while (!_isFetchCompleted && timeout > 0) {
                timeout -= Time.unscaledDeltaTime;
                yield return null;
            }

            if (timeout <= 0) {
                Debug.LogWarning("RemoteConfig_GP: Fetch timed out. Using default values.");
            }
        }

        public string GetValue(string key) {
            // Если кэш есть и ключ найден — возвращаем значение
            if (_configCache != null && _configCache.TryGetValue(key, out var value)) {
                return value;
            }

            // Fallback: если что-то пошло не так, пробуем достать дефолтное значение напрямую
            Debug.LogWarning($"RemoteConfig_GP: Key '{key}' not found in cache. Trying defaults.");
            return _keysStorage.GetDefaultValue<object>(key)?.ToString();
        }

        // --- Внутренние методы ---

        private void InitializeDefaults() {
            _configCache = new Dictionary<string, string>();
            
            foreach (var kv in _keysStorage.GetDefaultValues()) {
                // Конвертируем все в строки, так как GamePush и IRemoteConfig работают со строками
                _configCache[kv.Key] = kv.Value.ToString();
            }
        }

        private void OnFetchSuccess(List<VariablesData> variables) {
            Debug.Log($"RemoteConfig_GP: Fetch success. Received {variables.Count} variables.");
            
            foreach (var variable in variables) {
                // Обновляем значение в кэше. 
                // variable.key - имя переменной в админке GP
                // variable.value - значение (строка)
                
                if (_configCache.ContainsKey(variable.key)) {
                    _configCache[variable.key] = variable.value;
                } else {
                    // Если в админке добавили новый ключ, которого нет в коде, тоже добавляем (на всякий случай)
                    _configCache.Add(variable.key, variable.value);
                }
            }
            
            _isFetchCompleted = true;
        }

        private void OnFetchError() {
            Debug.LogWarning("RemoteConfig_GP: Fetch failed. Keeping default values.");
            _isFetchCompleted = true;
        }
    }
}
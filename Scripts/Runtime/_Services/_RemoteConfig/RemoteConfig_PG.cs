using System.Collections;
using System.Collections.Generic;
using __CoreGameLib._Scripts._Services._RemoteConfig;
using _Services._Saving;
using Playgama;
using UnityEngine;

namespace _Infrastructure {
    public class RemoteConfig_PG : IRemoteConfig {
        private bool _isLoaded;
        private IKeysStorage _keysStorage;
        private DataParserTool _dataParserTool;

        public IEnumerator LoadConfigs(IKeysStorage keysStorage, bool loadPlatformVariables = false) {
            _keysStorage = keysStorage;
            
            // 1. Сразу инициализируем парсер дефолтными значениями.
            // Согласно документации: "always provide hardcoded defaults" до старта.
            var defValues = _keysStorage.GetDefaultValues();
            _dataParserTool = new DataParserTool(defValues, _keysStorage);

            // 2. Проверяем поддержку на текущей площадке
            if (!Bridge.remoteConfig.isSupported) {
                Debug.Log("[RemoteConfig_PG] Remote Config не поддерживается на данной площадке. Используем локальные дефолты.");
                yield break;
            }

            _isLoaded = false;

            // 3. Устанавливаем контекст для сегментации (как в твоем старом коде)
            // В v2 параметры накапливаются и отправляются при вызове Get()
            var clientFeatures = new object[] { defValues };
            var context = new Dictionary<string, object> {
                { "clientFeatures", clientFeatures }
            };
            Bridge.remoteConfig.SetContext(context);

            // 4. Запрашиваем конфиг. В v2 метод Get() вызывается без параметров.
            Bridge.remoteConfig.Get(OnLoadComplete);

            // 5. Ждем ответа с защитой от вечного зависания (Таймаут 5 секунд)
            float timeout = 5.0f;
            while (!_isLoaded && timeout > 0) {
                timeout -= Time.unscaledDeltaTime;
                yield return null;
            }

            if (timeout <= 0) {
                Debug.LogWarning("[RemoteConfig_PG] Превышено время ожидания загрузки конфигов. Используем дефолтные значения.");
            }
        }

        public string GetValue(string key) {
            return _dataParserTool.GetDataString(key);
        }

        // В v2 возвращается строго Dictionary<string, string>
        private void OnLoadComplete(bool success, Dictionary<string, string> map) {
            _isLoaded = true;

            if (!success || map == null) {
                Debug.LogWarning("[RemoteConfig_PG] Ошибка загрузки Remote Config или конфиг пуст.");
                return;
            }

            Debug.Log($"[RemoteConfig_PG] Успешная загрузка. Получено ключей: {map.Count}");

            var defMap = _keysStorage.GetDefaultValues();
            var mergedData = new Dictionary<string, object>();

            // Мержим полученные данные поверх дефолтных
            foreach (var kv in defMap) {
                if (map.ContainsKey(kv.Key)) {
                    mergedData[kv.Key] = map[kv.Key];
                } else {
                    mergedData[kv.Key] = kv.Value;
                }
            }

            // Пересобираем парсер с уже объединенными (актуальными) данными
            _dataParserTool = new DataParserTool(mergedData, _keysStorage);
        }
    }
}
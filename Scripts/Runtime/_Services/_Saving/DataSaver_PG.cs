#if PLAYGAMA
using System;
using System.Collections;
using System.Collections.Generic;
using Playgama;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace _Services._Saving {
    public class DataSaver_PG : IDataSaver {
        private readonly HashSet<string> _savingKeys = new HashSet<string>();
        private readonly Dictionary<string, string> _pendingSaves = new Dictionary<string, string>();

        public async UniTask<string> Load(string key) {
            bool isDone = false;
            string loadedString = null;

            // В Bridge SDK v2 передаем ключ в виде списка
            var keys = new List<string> {
                key
            };

            Bridge.storage.Get(keys, (success, data) => {
                // Если успех и список данных не пуст, берем первый элемент (индекс 0)
                if (success && data != null && data.Count > 0 && !string.IsNullOrEmpty(data[0])) {
                    loadedString = data[0];
                }
                isDone = true;
            });

            await UniTask.WaitUntil(() => isDone);
            return loadedString;
        }

        public void Save(string key, string json) {
            if (_savingKeys.Contains(key)) {
                // Предыдущее сохранение еще выполняется. Запоминаем самое актуальное состояние.
                _pendingSaves[key] = json;
                return;
            }

            _savingKeys.Add(key);
            SendSave(key, json);
        }

        private void SendSave(string key, string json) {
            // Упаковываем ключ и данные в списки
            var keys = new List<string> {
                key
            };
            var data = new List<object> {
                json
            }; // JSON (строка) передается как object

            try {
                Bridge.storage.Set(keys, data, (success) => {
                    if (!success) {
                        Debug.LogWarning($"[DataSaver_PG] Не удалось сохранить данные по ключу: {key}");
                    }

                    _savingKeys.Remove(key);

                    if (_pendingSaves.TryGetValue(key, out var pendingJson)) {
                        _pendingSaves.Remove(key);
                        Save(key, pendingJson);
                    }
                });
            } catch (Exception ex) {
                Debug.LogError($"[DataSaver_PG] Ошибка при вызове Bridge.storage.Set для ключа {key}: {ex.Message}");
                _savingKeys.Remove(key);
                if (_pendingSaves.TryGetValue(key, out var pendingJson)) {
                    _pendingSaves.Remove(key);
                    Save(key, pendingJson);
                }
            }
        }

        public void Delete(string key) {
            _pendingSaves.Remove(key);

            // Упаковываем ключ в список
            var keys = new List<string> {
                key
            };

            Bridge.storage.Delete(keys, (success) => {
                if (!success) {
                    Debug.LogWarning($"[DataSaver_PG] Не удалось удалить данные по ключу: {key}");
                }
            });
        }
    }
}

#endif

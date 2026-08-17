using System;
using System.Collections;
using System.Collections.Generic;
using Core._Services._Saving;
using Playgama;
using UnityEngine;

namespace _Services._Saving {
    public class DataSaver_PG : IDataSaver {

        public IEnumerator Load(string key, Action<string> onLoaded) {
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

            yield return new WaitUntil(() => isDone);
            onLoaded?.Invoke(loadedString);
        }

        public void Save(string key, string json) {
            // Упаковываем ключ и данные в списки
            var keys = new List<string> {
                key
            };
            var data = new List<object> {
                json
            }; // JSON (строка) передается как object

            Bridge.storage.Set(keys, data, (success) => {
                if (!success) {
                    Debug.LogWarning($"[DataSaver_PG] Не удалось сохранить данные по ключу: {key}");
                }
            });
        }

        public void Delete(string key) {
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

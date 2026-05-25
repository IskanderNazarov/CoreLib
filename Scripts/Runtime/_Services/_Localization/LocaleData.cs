using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Services._Localization {
    [CreateAssetMenu(fileName = "LocaleData_", menuName = "Data/Localization/LocaleData")]
    public class LocaleData : ScriptableObject {
        public LangCode language;

        // Список для удобного редактирования в Инспекторе
        public List<TranslationEntry> entries = new List<TranslationEntry>();

        // Словарь для быстрого O(1) поиска в рантайме
        private Dictionary<string, string> _dict;

        public void Initialize() {
            _dict = new Dictionary<string, string>();
            foreach (var entry in entries) {
                if (!_dict.ContainsKey(entry.key))
                    _dict.Add(entry.key, entry.value);
            }
        }

        public string GetText(string key) {
            if (_dict == null) Initialize();
            return _dict.TryGetValue(key, out string val) ? val : $"[{key}]";
        }
    }

    [Serializable]
    public class TranslationEntry {
        public string key;
        [TextArea(1, 3)] public string value;
    }

    public enum LangCode { en, ru, tr, es }
}

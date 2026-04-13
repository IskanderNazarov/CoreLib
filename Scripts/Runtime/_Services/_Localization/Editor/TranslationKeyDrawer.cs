using System.Linq;
using UnityEditor;
using UnityEngine;

namespace _Services._Localization.Editor {
    [CustomPropertyDrawer(typeof(TranslationKeyAttribute))]
    public class TranslationKeyDrawer : PropertyDrawer {
        private string[] _keys;

        private void LoadKeys() {
            // Кешируем ключи, чтобы не нагружать редактор при каждой отрисовке кадра
            if (_keys != null) return;

            // Ищем твой LocalesSettings (базу данных)
            string[] guids = AssetDatabase.FindAssets("t:LocalesSettings");
            if (guids.Length == 0) {
                _keys = new[] { "NO_SETTINGS_FOUND" };
                return;
            }

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            var settings = AssetDatabase.LoadAssetAtPath<LocalesSettings>(path);

            // Берем первый язык из массива как "мастер-список" ключей
            if (settings != null && settings.Locales != null && settings.Locales.Length > 0) {
                var masterLocale = settings.Locales[0];
                if (masterLocale != null && masterLocale.entries != null) {
                    // Собираем уникальные непустые ключи
                    _keys = masterLocale.entries
                        .Select(e => e.key)
                        .Where(k => !string.IsNullOrEmpty(k))
                        .Distinct()
                        .ToArray();
                }
            }

            if (_keys == null || _keys.Length == 0) {
                _keys = new[] { "NO_KEYS_FOUND" };
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (property.propertyType != SerializedPropertyType.String) {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            LoadKeys();

            // Находим индекс текущего ключа в массиве
            int selectedIndex = Mathf.Max(0, System.Array.IndexOf(_keys, property.stringValue));

            // Отрисовываем сам Popup
            selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, _keys);

            // Сохраняем строку по выбранному индексу
            if (_keys.Length > 0 && selectedIndex < _keys.Length) {
                property.stringValue = _keys[selectedIndex];
            }
        }
    }
}
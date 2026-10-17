using System;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace _Services._Localization.Editor {
    [CustomPropertyDrawer(typeof(TranslationKeyAttribute))]
    public class TranslationKeyDrawer : PropertyDrawer {
        private string[] _keys;

        private void LoadKeys(bool forceReload = false) {
            // Кешируем ключи, чтобы не нагружать редактор при каждой отрисовке кадра
            if (_keys != null && !forceReload) return;

            // Ищем LocalesSettings (базу данных)
            string[] guids = AssetDatabase.FindAssets("t:LocalesSettings");
            if (guids.Length == 0) {
                _keys = new[] { "NO_SETTINGS_FOUND" };
                return;
            }

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            var settings = AssetDatabase.LoadAssetAtPath<LocalesSettings>(path);

            if (settings != null && settings.Locales != null && settings.Locales.Length > 0) {
                // Собираем уникальные непустые ключи со всех локалей и сортируем их
                _keys = settings.Locales
                    .Where(l => l != null && l.entries != null)
                    .SelectMany(l => l.entries)
                    .Select(e => e.key)
                    .Where(k => !string.IsNullOrEmpty(k))
                    .Distinct()
                    .OrderBy(k => k)
                    .ToArray();
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

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            string currentKey = property.stringValue;
            string displayTitle = string.IsNullOrEmpty(currentKey) ? "<None>" : currentKey;

            bool hasKey = !string.IsNullOrEmpty(currentKey);
            bool keyExists = !hasKey || (_keys != null && _keys.Contains(currentKey));

            GUIContent buttonContent = new GUIContent(
                displayTitle,
                keyExists ? displayTitle : $"Warning: Key '{currentKey}' not found in LocalesSettings!"
            );

            Color prevColor = GUI.color;
            if (!keyExists) {
                GUI.color = new Color(1f, 0.65f, 0.65f);
            }

            if (EditorGUI.DropdownButton(position, buttonContent, FocusType.Keyboard, EditorStyles.popup)) {
                LoadKeys(forceReload: true);

                var serializedObject = property.serializedObject;
                var propertyPath = property.propertyPath;

                var dropdown = new TranslationKeyDropdown(
                    new AdvancedDropdownState(),
                    _keys,
                    selectedKey => {
                        serializedObject.Update();
                        var targetProp = serializedObject.FindProperty(propertyPath);
                        if (targetProp != null) {
                            targetProp.stringValue = selectedKey;
                            serializedObject.ApplyModifiedProperties();
                        }
                    }
                );

                dropdown.Show(position);
            }

            GUI.color = prevColor;
        }
    }

    public class TranslationKeyDropdown : AdvancedDropdown {
        private readonly string[] _keys;
        private readonly Action<string> _onSelected;

        public TranslationKeyDropdown(AdvancedDropdownState state, string[] keys, Action<string> onSelected) : base(state) {
            _keys = keys;
            _onSelected = onSelected;
            minimumSize = new Vector2(280, 320);
        }

        protected override AdvancedDropdownItem BuildRoot() {
            var root = new AdvancedDropdownItem("Localization Keys");

            root.AddChild(new KeyDropdownItem("<None>", string.Empty));

            if (_keys != null) {
                foreach (var key in _keys) {
                    if (key == "NO_SETTINGS_FOUND" || key == "NO_KEYS_FOUND") {
                        root.AddChild(new AdvancedDropdownItem(key) { enabled = false });
                        continue;
                    }

                    if (key.Contains('/')) {
                        string[] parts = key.Split('/');
                        AdvancedDropdownItem parent = root;
                        for (int i = 0; i < parts.Length - 1; i++) {
                            var existing = parent.children.FirstOrDefault(c => c.name == parts[i]);
                            if (existing == null) {
                                existing = new AdvancedDropdownItem(parts[i]);
                                parent.AddChild(existing);
                            }
                            parent = existing;
                        }
                        parent.AddChild(new KeyDropdownItem(parts[parts.Length - 1], key));
                    } else {
                        root.AddChild(new KeyDropdownItem(key, key));
                    }
                }
            }

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item) {
            base.ItemSelected(item);
            if (item is KeyDropdownItem keyItem) {
                _onSelected?.Invoke(keyItem.Key);
            }
        }

        private class KeyDropdownItem : AdvancedDropdownItem {
            public string Key { get; }

            public KeyDropdownItem(string displayName, string key) : base(displayName) {
                Key = key;
            }
        }
    }
}
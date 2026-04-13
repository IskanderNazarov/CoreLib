using System.Linq;
using UnityEditor;
using UnityEngine;

namespace _Services._Localization.Editor {
    public class LocalizationValidatorWindow : EditorWindow
    {
        private LocalesSettings _settings;
        private Vector2 _scrollPos;

        // Добавляем пункт в верхнее меню Unity
        [MenuItem("Tools/Localization/Validator")]
        public static void ShowWindow()
        {
            GetWindow<LocalizationValidatorWindow>("Locales Validator");
        }

        private void OnEnable()
        {
            // При открытии окна пытаемся автоматически найти LocalesSettings
            string[] guids = AssetDatabase.FindAssets("t:LocalesSettings");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                _settings = AssetDatabase.LoadAssetAtPath<LocalesSettings>(path);
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("Анализ локализаций", EditorStyles.boldLabel);

            // Поле для ручного выбора базы (на случай, если автоматика не сработает)
            _settings = (LocalesSettings)EditorGUILayout.ObjectField("База настроек", _settings, typeof(LocalesSettings), false);

            if (_settings == null || _settings.Locales == null || _settings.Locales.Length == 0)
            {
                EditorGUILayout.HelpBox("Выберите LocalesSettings, в котором есть хотя бы один язык.", MessageType.Warning);
                return;
            }

            // Берем язык под индексом 0 как эталон (Мастер-язык)
            var masterLocale = _settings.Locales[0];
            if (masterLocale == null)
            {
                EditorGUILayout.HelpBox("Базовый язык (индекс 0) не назначен.", MessageType.Error);
                return;
            }

            // Собираем все ключи из Мастер-языка
            var masterKeys = masterLocale.entries
                .Select(e => e.key)
                .Where(k => !string.IsNullOrEmpty(k))
                .ToList();

            GUILayout.Space(10);
            GUILayout.Label($"Эталонный язык: {masterLocale.language} ({masterKeys.Count} ключей)", EditorStyles.boldLabel);
            GUILayout.Space(10);

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            bool allGood = true;

            // Проверяем все остальные языки, начиная с индекса 1
            for (int i = 1; i < _settings.Locales.Length; i++)
            {
                var targetLocale = _settings.Locales[i];
                if (targetLocale == null) continue;

                var targetKeys = targetLocale.entries.Select(e => e.key).ToList();
            
                // Ищем ключи, которые есть в мастере, но нет в проверяемом языке
                var missingKeys = masterKeys.Except(targetKeys).ToList();
            
                // Ищем ключи, которые есть, но значение пустое
                var emptyValues = targetLocale.entries.Where(e => string.IsNullOrEmpty(e.value)).Select(e => e.key).ToList();

                if (missingKeys.Count > 0 || emptyValues.Count > 0)
                {
                    allGood = false;
                
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    GUILayout.Label($"Язык: {targetLocale.language}", EditorStyles.boldLabel);

                    // Отрисовка пропущенных ключей (Красным)
                    if (missingKeys.Count > 0)
                    {
                        GUI.color = new Color(1f, 0.4f, 0.4f);
                        GUILayout.Label($"Отсутствуют ключи ({missingKeys.Count}):", EditorStyles.boldLabel);
                        GUI.color = Color.white;

                        foreach (var key in missingKeys)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label($"- {key}");
                            if (GUILayout.Button("Добавить пустой", GUILayout.Width(120)))
                            {
                                AddKeyToLocale(targetLocale, key);
                            }
                            GUILayout.EndHorizontal();
                        }

                        if (GUILayout.Button($"Добавить все недостающие в {targetLocale.language}"))
                        {
                            foreach (var key in missingKeys) AddKeyToLocale(targetLocale, key);
                        }
                    }

                    // Отрисовка пустых значений (Желтым)
                    if (emptyValues.Count > 0)
                    {
                        GUILayout.Space(5);
                        GUI.color = Color.yellow;
                        GUILayout.Label($"Пустые переводы ({emptyValues.Count}):", EditorStyles.boldLabel);
                        GUI.color = Color.white;

                        foreach (var key in emptyValues)
                        {
                            GUILayout.Label($"- {key}");
                        }
                    }

                    EditorGUILayout.EndVertical();
                    GUILayout.Space(10);
                }
            }

            if (allGood)
            {
                GUI.color = Color.green;
                GUILayout.Label("Все локализации идеально синхронизированы с эталоном!", EditorStyles.boldLabel);
                GUI.color = Color.white;
            }

            EditorGUILayout.EndScrollView();
        }

        // Метод добавления ключа с пометкой файла как измененного
        private void AddKeyToLocale(LocaleData locale, string key)
        {
            locale.entries.Add(new TranslationEntry { key = key, value = "" });
        
            // Обязательно помечаем ассет как "грязный", чтобы Unity сохранила изменения
            EditorUtility.SetDirty(locale);
            AssetDatabase.SaveAssets();
        }
    }
}
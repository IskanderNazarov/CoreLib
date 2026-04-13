using System;
using System.Collections.Generic;
using _Services._PlatformActions;
using UnityEngine;

namespace _Services._Localization {
    public class Localizer {
        public event Action OnLanguageChanged;

        private readonly Dictionary<LangCode, LocaleData> _locales = new();
        private LocaleData _currentLocale;

        public LangCode CurrentLanguage { get; private set; }

        // Делаем public для корректной работы DI-контейнера
        public Localizer(LocalesSettings localesSettings, IPlatformActionProvider platformActionProvider) {
            foreach (var loc in localesSettings.Locales) {
                loc.Initialize();
                _locales[loc.language] = loc;
            }

            var currentLang = localesSettings.StartLang;
            var systemLang = platformActionProvider.GetISO();

            // Оптимизированный поиск языка без LINQ
            if (Enum.TryParse<LangCode>(systemLang, true, out var parsedLang) && _locales.ContainsKey(parsedLang)) {
                currentLang = parsedLang;
            }

            SetLanguage(currentLang);
        }

        public void SetLanguage(LangCode langCode) {
            if (_locales.TryGetValue(langCode, out var locale)) {
                _currentLocale = locale;
                CurrentLanguage = langCode;
                OnLanguageChanged?.Invoke();
            } else {
                Debug.LogError($"Locale for {langCode} not found!");
            }
        }

        public string Get(string key) {
            if (_currentLocale == null) return $"[{key}]";
            return _currentLocale.GetText(key);
        }
    }
}
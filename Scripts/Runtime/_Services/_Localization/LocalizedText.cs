using TMPro;
using UnityEngine;
using Zenject;

namespace _Services._Localization {
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedText : MonoBehaviour {
        [TranslationKey] public string key;

        [Tooltip("Применить перевод автоматически при старте и смене языка?")]
        public bool autoApply = true;

        private TMP_Text _text;
        private Localizer _localizer;
        private object[] _formatArgs;

        // Внедряем зависимость и сразу подписываемся на событие
        [Inject]
        private void Construct(Localizer localizer) {
            _localizer = localizer;
            _localizer.OnLanguageChanged += OnLanguageChanged;
        }

        private void Awake() {
            _text = GetComponent<TMP_Text>();
        }

        private void Start() {
            if (autoApply) Apply();
        }

        // Отписываемся при уничтожении объекта, чтобы избежать утечек памяти
        private void OnDestroy() {
            if (_localizer != null) {
                _localizer.OnLanguageChanged -= OnLanguageChanged;
            }
        }

        private void OnLanguageChanged() {
            if (autoApply) Apply();
        }

        public void Apply() {
            if (string.IsNullOrEmpty(key) || _localizer == null) return;

            string translated = _localizer.Get(key);
            _text.text = _formatArgs != null && _formatArgs.Length > 0
                ? string.Format(translated, _formatArgs)
                : translated;
        }

        public void FormatAndApply(params object[] args) {
            _formatArgs = args;
            Apply();
        }

        public void SetKeyAndApply(string newKey, params object[] args) {
            key = newKey;
            FormatAndApply(args);
        }
    }
}
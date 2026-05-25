using UnityEngine;

namespace _Services._Localization {
    [CreateAssetMenu(fileName = "Database", menuName = "Data/Localization/LocaleDatabase")]
    public class LocalesSettings : ScriptableObject {
        public LangCode StartLang = LangCode.en;
        public LocaleData[] Locales;
    }
}
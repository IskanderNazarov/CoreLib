// Файл: IRemoteConfig.cs
using System.Collections;
using _Services._Saving;

namespace __CoreGameLib._Scripts._Services._RemoteConfig {
    public interface IRemoteConfig {
        // Добавлен параметр loadPlatformVariables (по умолчанию false)
        IEnumerator LoadConfigs(IKeysStorage keysStorage, bool loadPlatformVariables = false);
        string GetValue(string key);
    }
}

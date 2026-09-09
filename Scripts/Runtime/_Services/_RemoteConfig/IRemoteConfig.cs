// Файл: IRemoteConfig.cs
using System.Collections;
using _Services._Saving;
using Cysharp.Threading.Tasks;

namespace __CoreGameLib._Scripts._Services._RemoteConfig {
    public interface IRemoteConfig {
        // Добавлен параметр loadPlatformVariables (по умолчанию false)
        UniTask LoadConfigs(IKeysStorage keysStorage, bool loadPlatformVariables = false);
        string GetValue(string key);
    }
}

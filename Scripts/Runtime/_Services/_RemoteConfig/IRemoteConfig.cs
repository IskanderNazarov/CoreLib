// Файл: IRemoteConfig.cs

using Cysharp.Threading.Tasks;

namespace _Services._RemoteConfig {
    public interface IRemoteConfig {
        // Добавлен параметр loadPlatformVariables (по умолчанию false)
        UniTask LoadConfigs(IKeysStorage keysStorage, bool loadPlatformVariables = false);
        string GetValue(string key);
    }
}

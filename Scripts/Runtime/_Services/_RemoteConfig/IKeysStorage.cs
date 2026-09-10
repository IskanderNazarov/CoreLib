using System.Collections.Generic;

namespace _Services._RemoteConfig {
    public interface IKeysStorage {
        List<string> GetAllKeys();
        Dictionary<string, object> GetDefaultValues();
        T GetDefaultValue<T>(string key);

        void TryToAddDefaultValue(string key, object value);
    }
}
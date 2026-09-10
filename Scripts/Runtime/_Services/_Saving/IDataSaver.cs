// CoreLib.asmdef (Слой Core)

using Cysharp.Threading.Tasks;

namespace _Services._Saving {
    public interface IDataSaver {
        // Загружает строку (JSON) по одному главному ключу
        UniTask<string> Load(string key);
        
        // Передает готовую строку на сервер
        void Save(string key, string json);
        
        // Полезно для кнопки "Сбросить прогресс"
        void Delete(string key);
    }
}
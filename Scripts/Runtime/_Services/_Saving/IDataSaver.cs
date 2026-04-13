// CoreLib.asmdef (Слой Core)
using System;
using System.Collections;

namespace Core._Services._Saving {
    public interface IDataSaver {
        // Загружает строку (JSON) по одному главному ключу
        IEnumerator Load(string key, Action<string> onLoaded);
        
        // Передает готовую строку на сервер
        void Save(string key, string json);
        
        // Полезно для кнопки "Сбросить прогресс"
        void Delete(string key);
    }
}
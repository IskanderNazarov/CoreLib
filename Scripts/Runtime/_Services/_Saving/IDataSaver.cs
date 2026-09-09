// CoreLib.asmdef (Слой Core)
using System;
using System.Collections;
using Cysharp.Threading.Tasks;

namespace Core._Services._Saving {
    public interface IDataSaver {
        // Загружает строку (JSON) по одному главному ключу
        UniTask<string> Load(string key);
        
        // Передает готовую строку на сервер
        void Save(string key, string json);
        
        // Полезно для кнопки "Сбросить прогресс"
        void Delete(string key);
    }
}
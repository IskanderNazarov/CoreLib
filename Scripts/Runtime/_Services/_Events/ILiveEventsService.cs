using System;
using Cysharp.Threading.Tasks;

namespace _Services._Events 
{
    /// <summary>
    /// Универсальная модель внутриигрового события, 
    /// не зависящая от конкретного SDK.
    /// </summary>
    public class LiveEventData 
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public bool IsAutoJoin { get; set; }
        
        // Можно добавить ссылки на иконки (локальные или URL)
        public string IconUrl { get; set; }
    }

    /// <summary>
    /// Контракт сервиса управления событиями
    /// </summary>
    public interface ILiveEventsService 
    {
        // 1. События для подписки UI
        event Action<string> OnEventStarted;    // передаем ID ивента
        event Action<string> OnEventEnded;
        event Action<string> OnEventJoined;

        // 2. Проверки состояний (синхронные, обычно берутся из кэша адаптера)
        bool IsEventActive(string eventId);
        bool IsJoined(string eventId);
        TimeSpan GetTimeLeft(string eventId);

        // 3. Получение данных об ивенте
        LiveEventData GetEventData(string eventId);
        LiveEventData[] GetAllActiveEvents();

        // 4. Действия
        /// <summary>
        /// Попытка присоединиться к ивенту (ручная регистрация)
        /// </summary>
        UniTask<bool> JoinEventAsync(string eventId);
    }
}
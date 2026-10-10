using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Services._Events 
{
    public class LiveEventsService_Editor : ILiveEventsService 
    {
        public event Action<string> OnEventStarted;
        public event Action<string> OnEventEnded;
        public event Action<string> OnEventJoined;

        private List<LiveEventData> _mockEvents = new List<LiveEventData>();
        private HashSet<string> _joinedEvents = new HashSet<string>();

        public LiveEventsService_Editor() 
        {
            // Добавляем тестовое событие для редактора
            _mockEvents.Add(new LiveEventData {
                Id = "TEST_EVENT",
                Name = "Тестовое событие Editor",
                Description = "Это событие доступно только в Unity Editor для тестов",
                DateStart = DateTime.Now.AddDays(-1),
                DateEnd = DateTime.Now.AddDays(2),
                IsAutoJoin = false,
                IconUrl = ""
            });
        }

        public bool IsEventActive(string eventId) 
        {
            var ev = GetEventData(eventId);
            if (ev == null) return false;
            
            var now = DateTime.Now;
            return now >= ev.DateStart && now <= ev.DateEnd;
        }

        public bool IsJoined(string eventId) 
        {
            return _joinedEvents.Contains(eventId);
        }

        public TimeSpan GetTimeLeft(string eventId) 
        {
            var ev = GetEventData(eventId);
            if (ev == null) return TimeSpan.Zero;
            
            var now = DateTime.Now;
            if (now > ev.DateEnd) return TimeSpan.Zero;
            return ev.DateEnd - now;
        }

        public LiveEventData GetEventData(string eventId) 
        {
            return _mockEvents.FirstOrDefault(e => e.Id == eventId);
        }

        public LiveEventData[] GetAllActiveEvents() 
        {
            var now = DateTime.Now;
            return _mockEvents.Where(e => now >= e.DateStart && now <= e.DateEnd).ToArray();
        }

        public async UniTask<bool> JoinEventAsync(string eventId) 
        {
            // Имитация задержки сети
            await UniTask.Delay(500); 
            
            var ev = GetEventData(eventId);
            if (ev != null && IsEventActive(eventId)) 
            {
                if (_joinedEvents.Add(eventId)) 
                {
                    OnEventJoined?.Invoke(eventId);
                }
                return true;
            }
            
            Debug.LogWarning($"[LiveEventsService_Editor] Не удалось присоединиться к событию {eventId}");
            return false;
        }
        
        // Метод для отладки: симуляция старта нового события
        public void Debug_StartEvent(LiveEventData newEvent) 
        {
            _mockEvents.Add(newEvent);
            OnEventStarted?.Invoke(newEvent.Id);
        }
        
        // Метод для отладки: симуляция окончания события
        public void Debug_EndEvent(string eventId) 
        {
            var ev = GetEventData(eventId);
            if (ev != null) 
            {
                ev.DateEnd = DateTime.Now.AddSeconds(-1);
                OnEventEnded?.Invoke(eventId);
            }
        }
    }
}

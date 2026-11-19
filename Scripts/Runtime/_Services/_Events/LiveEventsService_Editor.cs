using System;
using System.Collections.Generic;
using System.Linq;
using _Data;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Services._Events 
{
    public class LiveEventsService_Editor : ILiveEventsService 
    {
        public event Action<string> OnEventStarted;
        public event Action<string> OnEventEnded;
        public event Action<string> OnEventJoined;

        private readonly ProjectSettings _projectSettings;
        private readonly List<LiveEventData> _mockEvents = new List<LiveEventData>();
        private readonly HashSet<string> _joinedEvents = new HashSet<string>();

        private LiveEventData _settingsMockEvent;
        private EditorEventMode _lastSyncedMode = EditorEventMode.Disabled;
        private string _lastSyncedEventId = string.Empty;

        public LiveEventsService_Editor([InjectOptional] ProjectSettings projectSettings = null) 
        {
            _projectSettings = projectSettings;
            SyncWithSettings();
        }

        private void SyncWithSettings()
        {
            if (_projectSettings == null)
            {
                RemoveSettingsMockEvent();
                return;
            }

            var mode = _projectSettings.EditorEventMode;
            string targetId = _projectSettings.EditorTestEventId != null
                ? _projectSettings.EditorTestEventId.Trim()
                : string.Empty;

            if (mode == _lastSyncedMode && string.Equals(_lastSyncedEventId, targetId, StringComparison.Ordinal))
            {
                return;
            }

            _lastSyncedMode = mode;
            _lastSyncedEventId = targetId;

            RemoveSettingsMockEvent();

            if (mode == EditorEventMode.MockActiveEvent && !string.IsNullOrEmpty(targetId))
            {
                _settingsMockEvent = new LiveEventData
                {
                    Id = targetId,
                    Name = $"Editor Mock ({targetId})",
                    Description = "Mock event configured via ProjectSettings",
                    DateStart = DateTime.Now.AddDays(-1),
                    DateEnd = DateTime.Now.AddDays(2),
                    IsAutoJoin = false,
                    IconUrl = ""
                };
                _mockEvents.Add(_settingsMockEvent);
            }
        }

        private void RemoveSettingsMockEvent()
        {
            if (_settingsMockEvent != null)
            {
                _mockEvents.Remove(_settingsMockEvent);
                _settingsMockEvent = null;
            }
        }

        public bool IsEventActive(string eventId) 
        {
            SyncWithSettings();
            var ev = GetEventDataInternal(eventId);
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
            SyncWithSettings();
            var ev = GetEventDataInternal(eventId);
            if (ev == null) return TimeSpan.Zero;
            
            var now = DateTime.Now;
            if (now > ev.DateEnd) return TimeSpan.Zero;
            return ev.DateEnd - now;
        }

        public LiveEventData GetEventData(string eventId) 
        {
            SyncWithSettings();
            return GetEventDataInternal(eventId);
        }

        private LiveEventData GetEventDataInternal(string eventId)
        {
            if (string.IsNullOrEmpty(eventId)) return null;
            return _mockEvents.FirstOrDefault(e => string.Equals(e.Id, eventId, StringComparison.OrdinalIgnoreCase));
        }

        public LiveEventData[] GetAllActiveEvents() 
        {
            SyncWithSettings();
            var now = DateTime.Now;
            return _mockEvents.Where(e => now >= e.DateStart && now <= e.DateEnd).ToArray();
        }

        public async UniTask<bool> JoinEventAsync(string eventId) 
        {
            // Имитация задержки сети
            await UniTask.Delay(500); 
            
            SyncWithSettings();
            var ev = GetEventDataInternal(eventId);
            if (ev != null && IsEventActive(eventId)) 
            {
                if (_joinedEvents.Add(ev.Id)) 
                {
                    OnEventJoined?.Invoke(ev.Id);
                }
                return true;
            }
            
            Debug.LogWarning($"[LiveEventsService_Editor] Не удалось присоединиться к событию {eventId}");
            return false;
        }
        
        // Метод для отладки: симуляция старта нового события
        public void Debug_StartEvent(LiveEventData newEvent) 
        {
            if (newEvent == null) return;
            _mockEvents.Add(newEvent);
            OnEventStarted?.Invoke(newEvent.Id);
        }
        
        // Метод для отладки: симуляция окончания события
        public void Debug_EndEvent(string eventId) 
        {
            var ev = GetEventDataInternal(eventId);
            if (ev != null) 
            {
                ev.DateEnd = DateTime.Now.AddSeconds(-1);
                OnEventEnded?.Invoke(ev.Id);
            }
        }
    }
}

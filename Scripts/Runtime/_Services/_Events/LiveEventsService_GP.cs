#if GAMEPUSH
using System;
using System.Linq;
using GamePush;
using Cysharp.Threading.Tasks;

namespace _Services._Events 
{
    public class LiveEventsService_GP : ILiveEventsService 
    {
        public event Action<string> OnEventStarted;
        public event Action<string> OnEventEnded;
        public event Action<string> OnEventJoined;

        public LiveEventsService_GP() 
        {
            GP_Events.OnEventJoin += OnGPEventJoin;
        }

        ~LiveEventsService_GP() 
        {
            GP_Events.OnEventJoin -= OnGPEventJoin;
        }

        private void OnGPEventJoin(PlayerEvents playerEvent) 
        {
            var allEvents = GP_Events.List();
            var gpEv = allEvents?.FirstOrDefault(e => e.id == playerEvent.eventId);
            if (gpEv != null) 
            {
                string tagId = string.IsNullOrEmpty(gpEv.tag) ? gpEv.id.ToString() : gpEv.tag;
                OnEventJoined?.Invoke(tagId);
            }
        }

        public bool IsEventActive(string eventId) 
        {
            if (!GP_Init.isReady) return false;
            return GP_Events.IsActive(eventId);
        }

        public bool IsJoined(string eventId) 
        {
            if (!GP_Init.isReady) return false;
            return GP_Events.IsJoined(eventId);
        }

        public TimeSpan GetTimeLeft(string eventId) 
        {
            if (!GP_Init.isReady) return TimeSpan.Zero;
            var ev = GP_Events.GetEvent(eventId);
            if (ev == null) return TimeSpan.Zero;
            return TimeSpan.FromSeconds(ev.timeLeft);
        }

        public LiveEventData GetEventData(string eventId) 
        {
            if (!GP_Init.isReady) return null;
            var ev = GP_Events.GetEvent(eventId);
            return Map(ev);
        }

        public LiveEventData[] GetAllActiveEvents() 
        {
            if (!GP_Init.isReady) return Array.Empty<LiveEventData>();
            var list = GP_Events.List();
            if (list == null) return Array.Empty<LiveEventData>();
            
            return list.Where(e => e.isActive).Select(Map).ToArray();
        }

        public async UniTask<bool> JoinEventAsync(string eventId) 
        {
            if (!GP_Init.isReady) return false;
            
            var ev = GP_Events.GetEvent(eventId);
            if (ev == null) return false;
            
            int targetId = ev.id;
            var tcs = new UniTaskCompletionSource<bool>();
            
            UnityEngine.Events.UnityAction<PlayerEvents> onJoin = null;
            UnityEngine.Events.UnityAction<string> onError = null;
            
            onJoin = (playerEvent) => {
                if (playerEvent.eventId == targetId) {
                    GP_Events.OnEventJoin -= onJoin;
                    GP_Events.OnEventJoinError -= onError;
                    tcs.TrySetResult(true);
                }
            };
            
            onError = (err) => {
                GP_Events.OnEventJoin -= onJoin;
                GP_Events.OnEventJoinError -= onError;
                tcs.TrySetResult(false);
            };
            
            GP_Events.OnEventJoin += onJoin;
            GP_Events.OnEventJoinError += onError;
            
            GP_Events.Join(eventId);
            
            return await tcs.Task;
        }

        private LiveEventData Map(EventData gpEvent) 
        {
            if (gpEvent == null) return null;
            
            DateTime.TryParse(gpEvent.dateStart, out DateTime start);
            DateTime.TryParse(gpEvent.dateEnd, out DateTime end);
            
            return new LiveEventData 
            {
                Id = string.IsNullOrEmpty(gpEvent.tag) ? gpEvent.id.ToString() : gpEvent.tag,
                Name = gpEvent.name,
                Description = gpEvent.description,
                DateStart = start,
                DateEnd = end,
                IsAutoJoin = gpEvent.isAutoJoin,
                IconUrl = gpEvent.icon
            };
        }
    }
}
#endif

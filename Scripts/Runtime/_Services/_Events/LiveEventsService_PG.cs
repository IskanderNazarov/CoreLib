#if PLAYGAMA
using System;
using Cysharp.Threading.Tasks;

namespace _Services._Events 
{
    public class LiveEventsService_PG : ILiveEventsService 
    {
        public event Action<string> OnEventStarted;
        public event Action<string> OnEventEnded;
        public event Action<string> OnEventJoined;

        public bool IsEventActive(string eventId) 
        {
            return false;
        }

        public bool IsJoined(string eventId) 
        {
            return false;
        }

        public TimeSpan GetTimeLeft(string eventId) 
        {
            return TimeSpan.Zero;
        }

        public LiveEventData GetEventData(string eventId) 
        {
            return null;
        }

        public LiveEventData[] GetAllActiveEvents() 
        {
            return Array.Empty<LiveEventData>();
        }

        public UniTask<bool> JoinEventAsync(string eventId) 
        {
            return UniTask.FromResult(false);
        }
    }
}
#endif

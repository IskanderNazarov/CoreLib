// Файл: Core/Services/Rating/RatingService_PG.cs
using System;
using Playgama;
using UnityEngine;

namespace core.rating {
    public class RatingService_PG : IRatingService {
        
        public bool IsReviewSupported() {
            // В Playgama проверка идет через свойство
            return Bridge.social.isRateSupported;
        }

        public void RequestNativeReview(Action<bool> onComplete = null) {
            if (!IsReviewSupported()) {
                Debug.LogWarning("[RatingService_PG] Native review is not supported on this platform.");
                onComplete?.Invoke(false);
                return;
            }

            // У Playgama Rate() - это void метод или Promise, в Unity обертке обычно используются события
            // или коллбэки. Если стандартный вызов не принимает Action, оборачиваем его.
            try {
                Bridge.social.Rate();
                // Playgama не всегда возвращает результат закрытия окна, 
                // поэтому просто сообщаем, что запрос был отправлен успешно.
                onComplete?.Invoke(true);
            } catch (Exception e) {
                Debug.LogError($"[RatingService_PG] Failed to show rate dialog: {e.Message}");
                onComplete?.Invoke(false);
            }
        }
    }
}

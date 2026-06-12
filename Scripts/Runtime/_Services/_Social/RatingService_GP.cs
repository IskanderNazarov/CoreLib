// Файл: Core/Services/Rating/RatingService_GP.cs
using System;
using GamePush;
using UnityEngine;

namespace core.rating {
    public class RatingService_GP : IRatingService {

        public bool IsReviewSupported() {
            // Проверяем, поддерживает ли текущая площадка отзывы и не оценивал ли игрок игру ранее
            return GP_App.CanReview();
        }

        public void RequestNativeReview(Action<bool> onComplete = null) {
            if (!IsReviewSupported()) {
                Debug.LogWarning("[RatingService_GP] Native review is not supported or already rated.");
                onComplete?.Invoke(false);
                return;
            }

            // Вызываем нативное окно GamePush
            GP_App.ReviewRequest(onReviewResult: OnReviewResult);

            void OnReviewResult(int obj) {
                throw new NotImplementedException();
            }

            GP_App.ReviewRequest(
                onReviewResult: (rating) => {
                    // Игрок успешно взаимодействовал с окном
                    Debug.Log($"[RatingService_GP] Review success! Rating: {rating}");
                    onComplete?.Invoke(true);
                },
                onReviewClose: (error) => {
                    // Окно закрыто без оценки или произошла ошибка
                    Debug.Log($"[RatingService_GP] Review closed or failed. Reason: {error}");
                    onComplete?.Invoke(false);
                }
            );
        }
    }
}

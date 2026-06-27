using System;

namespace core.rating {
    public class RatingService_Editor : IRatingService {

        public bool IsReviewSupported() {
            return true;
        }
        public void RequestNativeReview(Action<bool> onComplete = null) {
            onComplete?.Invoke(true);
        }
    }
}

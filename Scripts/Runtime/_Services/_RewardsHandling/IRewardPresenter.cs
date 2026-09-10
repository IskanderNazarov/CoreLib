using System;

namespace _Services._RewardsHandling {
    public interface IRewardPresenter {
        public event Action OnSequenceComplete;
        bool IsRewardingInProgress { get; }
        void ShowReward(Reward reward);
    }
}

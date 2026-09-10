namespace _Services._RewardsHandling {
    public interface IRewardApplier {
        void ApplyReward(Reward reward, string placement);
    }
}

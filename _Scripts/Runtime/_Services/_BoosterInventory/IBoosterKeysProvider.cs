namespace core.boosters {
    public interface IBoosterKeysProvider<TBoosterId> {
        string GetSaveKey(TBoosterId id);
    }
}
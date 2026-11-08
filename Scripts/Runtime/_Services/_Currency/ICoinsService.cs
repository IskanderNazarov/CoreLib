namespace _Services._Currency {
    /// <summary>
    /// Independent currency contract for Coins / soft currency.
    /// Used by BoosterService, Shop, and meta-features across any game project.
    /// </summary>
    public interface ICoinsService {
        bool IsAvailable { get; }
        int Coins { get; }
        bool HasCoins(int amount);
        void AddCoins(int amount, string source = "");
        bool TryConsumeCoins(int amount, string source = "");
    }
}

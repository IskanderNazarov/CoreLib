namespace _Services._SoundManagement {
    public interface ISoundStateProvider {
        bool IsSoundOn { get; set; }
        bool IsMusicOn { get; set; }
    }
}
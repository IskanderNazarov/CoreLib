namespace Core._Services.SoundManagement {
    public interface ISoundStateProvider {
        bool IsSoundOn { get; set; }
        bool IsMusicOn { get; set; }
    }
}
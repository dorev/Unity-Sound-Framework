
namespace Sound
{
    public delegate void SoundEffectDelegate(ISoundEffect effect, string message = null);

    public enum SoundEffectState
    {
        Starting,
        Processing,
        Finished
    }

    public interface ISoundEffect
    {
        int id { get; }
        string name { get; }
        SoundEffectState state { get; }
        void Update(SoundInstance soundInstance);
        bool Replace(ISoundEffect effect);
        SoundEffectDelegate onFinished { get; set; }
        SoundEffectDelegate onFailed { get; set; }
    }
}
using SmallEngine.Components;

namespace SmallEngine.Audio
{
    public class AudioComponent : Component
    {
        private int _id;
        private AudioResource _sound;

        public float Volume { get; set; }

        public AudioComponent() { }

        public AudioComponent(string pAlias)
        {
            _sound = ResourceManager.Request<AudioResource>(pAlias);
            Volume = AudioPlayer.MaxVolume;
        }

        public void SetAudio(string pAlias)
        {
            _sound = ResourceManager.Request<AudioResource>(pAlias);
        }

        public void Play()
        {
            _id = AudioPlayer.Play(_sound, Volume);
        }

        public void Loop()
        {
            _id = AudioPlayer.Loop(_sound, Volume);
        }

        public void Stop()
        {
            AudioPlayer.Stop(_id);
        }

        public void Pause()
        {
            AudioPlayer.Pause(_id);
        }

        public void Resume()
        {
            AudioPlayer.Resume(_id);
        }

        public override void Dispose()
        {
            base.Dispose();
            _sound.Dispose();
        }
    }
}

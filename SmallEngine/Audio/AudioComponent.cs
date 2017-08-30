namespace SmallEngine.Audio
{
    public class AudioComponent : Component
    {
        private AudioResource _sound;

        public AudioComponent()
        {
        }

        public AudioComponent(string pAlias)
        {
            _sound = ResourceManager.Request<AudioResource>(pAlias);
        }

        public void SetAudio(string pAlias)
        {
            _sound = ResourceManager.Request<AudioResource>(pAlias);
        }

        public void Play()
        {
            if(Game.ActiveCamera.IsVisible(GameObject))
            {
                _sound.Play();
            }
        }

        public void PlayImmediate()
        {
            if(Game.ActiveCamera.IsVisible(GameObject))
            {
                _sound.PlayImmediate();
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            _sound.Dispose();
        }
    }
}

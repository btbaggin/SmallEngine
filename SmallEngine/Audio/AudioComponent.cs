using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Audio
{
    class AudioComponent : Component
    {
        private AudioResource _sound;
        public AudioResource Sound
        {
            get { return _sound; }
        }

        public AudioComponent()
        {
        }

        public AudioComponent(string pAlias)
        {
            _sound = ResourceManager.Request<AudioResource>(pAlias);
        }

        public override void Dispose()
        {
            base.Dispose();
            _sound.Dispose();
        }
    }
}

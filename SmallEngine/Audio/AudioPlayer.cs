using System;
using System.Collections.Generic;
using SmallEngine.Messages;

using SharpDX.XAudio2;

namespace SmallEngine.Audio
{
    public class AudioPlayer : IMessageReceiver
    {
        #region Properties
        /// <summary>
        /// Returns the XAudio2 device.
        /// </summary>
        public static XAudio2 Device { get; private set; } = new XAudio2();

        /// <summary>
        /// Gets or sets master volume.
        /// </summary>
        public static float MasterVolume
        {
            get { return _mv.Volume; }
            set { _mv.SetVolume(value); }
        }

        /// <summary>
        /// Minimum volume of any clip
        /// </summary>
        public static float MinVolume
        {
            get { return 0f; }
        }

        /// <summary>
        /// Maximum volume of any clip
        /// </summary>
        public static float MaxVolume
        {
            get { return 1f; }
        }
        #endregion

        static DisposingMessageBus _messages = new DisposingMessageBus(64);
        static List<SourceVoice> _freeVoices = new List<SourceVoice>();
        static MasteringVoice _mv = new MasteringVoice(Device);

        static int _soundid;
        static Dictionary<int, SourceVoice> _playingSounds = new Dictionary<int, SourceVoice>(32);

        public AudioPlayer()
        {
            _mv.SetVolume(MaxVolume);
            _messages.Register(this);
            _messages.Start();
        }

        public static int Play(AudioResource pResource)
        {
            var id = GetSoundId();
            _messages.SendMessage(new AudioMessage("Play", pResource, 1f, id));
            return id;
        }

        public static void Stop(int pId)
        {
            _messages.SendMessage(new AudioMessage("Stop", pId));
        }

        public static void Pause(int pId)
        {
            _messages.SendMessage(new AudioMessage("Pause", pId));
        }

        public static void Resume(int pId)
        {
            _messages.SendMessage(new AudioMessage("Resume", pId));
        }

        #region Message Handling
        public void ReceiveMessage(IMessage pMessage)
        {
            var m = (AudioMessage)pMessage;
            AudioResource resource = m.Resource;
            SourceVoice voice;
            switch(pMessage.Type)
            {
                case "Play":
                    float volume = m.Volume;
                    System.Diagnostics.Debug.Assert(volume >= MinVolume);
                    System.Diagnostics.Debug.Assert(volume <= MaxVolume);

                    GetVoice(resource, m.Id, out voice);
                    if (voice.Volume != volume)
                    {
                        voice.SetVolume(volume);
                        Device.CommitChanges();
                    }
                    resource.Play(voice);
                    _playingSounds.Add(m.Id, voice);
                    break;

                case "Stop":
                    if(_playingSounds.ContainsKey(m.Id))
                    {
                        voice = _playingSounds[m.Id];
                        voice.Stop();
                        voice.FlushSourceBuffers();
                        _playingSounds.Remove(m.Id);
                    }
                    break;

                case "Loop":
                    //TODO

                case "Pause":
                    if(_playingSounds.ContainsKey(m.Id))
                    {
                        voice = _playingSounds[m.Id];
                        voice.Stop();
                    }
                    break;

                case "Resume":
                    if(_playingSounds.ContainsKey(m.Id))
                    {
                        voice = _playingSounds[m.Id];
                        voice.Start();
                    }
                    break;
            }
        }
        #endregion

        private static int GetSoundId()
        {
            unchecked
            {
                return System.Threading.Interlocked.Increment(ref _soundid);
            }
        }

        /// <summary>
        /// Gets a free voice from the voice pool.  If none are available a new one is created
        /// </summary>
        /// <param name="pSound">Sound to play with the voice</param>
        private static void GetVoice(AudioResource pSound, int pId, out SourceVoice pVoice)
        {
            lock (_freeVoices)
            {
                if (_freeVoices.Count == 0)
                {
                    pVoice = new SourceVoice(Device, pSound.Stream, true);
                    pVoice.BufferEnd += new SoundCompleteCallback(pVoice, pId).OnSoundFinished;
                }
                else
                {
                    pVoice = _freeVoices[_freeVoices.Count - 1];
                    _freeVoices.RemoveAt(_freeVoices.Count - 1);
                }
            }
        }

        /// <summary>
        /// Puts the voice back into the pool of eligible free voices
        /// </summary>
        /// <param name="pVoice">Voice to mark for reuse</param>
        private static void ReuseVoice(int pId, ref SourceVoice pVoice)
        {
            lock (_freeVoices)
            {
                _freeVoices.Add(pVoice);
            }
            _playingSounds.Remove(pId);
        }

        #region Disposable support
        public static void DisposePlayer()
        {
            _messages.Stop();
            lock (_freeVoices)
            {
                for (int i = 0; i < _freeVoices.Count; i++)
                {
                    _freeVoices[i].Dispose();
                }
            }

            Device.Dispose();
        }
        #endregion

        #region Callback
        protected struct SoundCompleteCallback
        {
            SourceVoice _voice;
            readonly int _id;
            public SoundCompleteCallback(SourceVoice pVoice, int pId)
            {
                _voice = pVoice;
                _id = pId;
            }
            public void OnSoundFinished(IntPtr arg)
            {
                _voice.BufferEnd -= OnSoundFinished;
                ReuseVoice(_id, ref _voice);
            }
        }
        #endregion
    }
}

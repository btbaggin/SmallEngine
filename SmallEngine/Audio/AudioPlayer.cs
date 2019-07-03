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

        static DisposingMessageBus _messages = new DisposingMessageBus(64, 1);
        static List<SourceVoice> _freeVoices = new List<SourceVoice>();
        static MasteringVoice _mv = new MasteringVoice(Device);

        readonly static AudioPlayer _instance = new AudioPlayer();
        static int _soundid;
        static Dictionary<int, SourceVoice> _playingSounds = new Dictionary<int, SourceVoice>(32);

        static AudioPlayer()
        {
            _mv.SetVolume(MaxVolume);

            _messages.Register(_instance);
            _messages.Start();
        }

        /// <summary>
        /// Will play the given AudioResource
        /// </summary>
        /// <param name="pResource">The resource to play</param>
        /// <returns>ID for the instance of the sound play</returns>
        public static int Play(AudioResource pResource)
        {
            return Play(pResource, MaxVolume);
        }

        /// <summary>
        /// Will play the given AudioResource at the specified volume
        /// </summary>
        /// <param name="pResource">The resource to play</param>
        /// <param name="pVolume">The volume at which to play the sound</param>
        /// <returns>ID for the instance of the sound play</returns>
        public static int Play(AudioResource pResource, float pVolume)
        {
            System.Diagnostics.Debug.Assert(pVolume >= MinVolume);
            System.Diagnostics.Debug.Assert(pVolume <= MaxVolume);

            var id = GetSoundId();
            _messages.SendMessage(new AudioMessage("Play", pResource, pVolume, id));
            return id;
        }

        /// <summary>
        /// Will play the given sound only if it is not currently playing
        /// </summary>
        /// <param name="pResource">The resource to play</param>
        /// <param name="pId">The ID of last time the resource was played</param>
        /// <returns>ID of the instance of the sound play</returns>
        public static int PlayOnce(AudioResource pResource, int pId)
        {
            return PlayOnce(pResource, MaxVolume, pId);
        }

        /// <summary>
        /// Will play the given sound only if it is not currently playing at the specified volume
        /// </summary>
        /// <param name="pResource">The resource to play</param>
        /// <param name="pVolume">The volume at which to play the sound</param>
        /// <param name="pId">The ID of last time the resource was played</param>
        /// <returns>ID of the instance of the sound play</returns>
        public static int PlayOnce(AudioResource pResource, float pVolume, int pId)
        {
            System.Diagnostics.Debug.Assert(pVolume >= MinVolume);
            System.Diagnostics.Debug.Assert(pVolume <= MaxVolume);

            if (_playingSounds.ContainsKey(pId)) return pId;

            return Play(pResource, pVolume);
        }

        /// <summary>
        /// Will loop the sound indefinitely until <see cref="Stop(int)"/> is called
        /// </summary>
        /// <param name="pResource">The resource to loop</param>
        /// <returns>ID of the instance of the sound play</returns>
        public static int Loop(AudioResource pResource)
        {
            return Loop(pResource, MaxVolume);
        }

        /// <summary>
        /// Will loop the sound indefinitely at the specified volume until <see cref="Stop(int)"/> is called
        /// </summary>
        /// <param name="pResource">The resource to loop</param>
        /// <param name="pVolume">The volume at whiche to play the sound</param>
        /// <returns>ID of the instance of the sound play</returns>
        public static int Loop(AudioResource pResource, float pVolume)
        {
            System.Diagnostics.Debug.Assert(pVolume >= MinVolume);
            System.Diagnostics.Debug.Assert(pVolume <= MaxVolume);

            var id = GetSoundId();
            _messages.SendMessage(new AudioMessage("Loop", pResource, pVolume, id));
            return id;
        }

        /// <summary>
        /// Stops the sound specified in the sound ID
        /// </summary>
        /// <param name="pId">The ID of the sound to stop</param>
        public static void Stop(int pId)
        {
            _messages.SendMessage(new AudioMessage("Stop", pId));
        }

        /// <summary>
        /// Pauses the sound specified in the sound ID
        /// </summary>
        /// <param name="pId">The ID of the sound to pause</param>
        public static void Pause(int pId)
        {
            _messages.SendMessage(new AudioMessage("Pause", pId));
        }

        /// <summary>
        /// Resumes playing of the sound specified in the sound ID
        /// </summary>
        /// <param name="pId">The ID of the sound to resume</param>
        public static void Resume(int pId)
        {
            _messages.SendMessage(new AudioMessage("Resume", pId));
        }

        #region Message Handling
        public void ReceiveMessage(IMessage pMessage)
        {
            var m = (AudioMessage)pMessage;
            AudioResource resource = m.Resource;
            float volume = m.Volume;
            SourceVoice voice;
            switch (pMessage.Type)
            {
                case "Play":
                case "Loop":
                    System.Diagnostics.Debug.Assert(!resource.Disposed, "Audio resource has been disposed");

                    GetVoice(resource, m.ID, pMessage.Type == "Loop", out voice);
                    if (voice.Volume != volume)
                    {
                        voice.SetVolume(volume);
                        Device.CommitChanges();
                    }
                    resource.Play(voice);
                    _playingSounds.Add(m.ID, voice);
                    break;

                case "Stop":
                    if(_playingSounds.ContainsKey(m.ID))
                    {
                        voice = _playingSounds[m.ID];
                        voice.Stop();
                        voice.FlushSourceBuffers();
                        _playingSounds.Remove(m.ID);
                    }
                    break;

                case "Pause":
                    if(_playingSounds.ContainsKey(m.ID))
                    {
                        voice = _playingSounds[m.ID];
                        voice.Stop();
                    }
                    break;

                case "Resume":
                    if(_playingSounds.ContainsKey(m.ID))
                    {
                        voice = _playingSounds[m.ID];
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
        private static void GetVoice(AudioResource pSound, int pId, bool pLoop, out SourceVoice pVoice)
        {
            lock (_freeVoices)
            {
                if (_freeVoices.Count == 0)
                {
                    pVoice = new SourceVoice(Device, pSound.Stream, true);
                }
                else
                {
                    pVoice = _freeVoices[_freeVoices.Count - 1];
                    _freeVoices.RemoveAt(_freeVoices.Count - 1);
                }

                var cb = new SoundCompleteCallback(pVoice, pId, pSound);
                cb.AddCallback(pLoop);
            }
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
        private class SoundCompleteCallback
        {
            SourceVoice _voice;
            readonly AudioResource _resource;
            readonly int _id;
            public SoundCompleteCallback(SourceVoice pVoice, int pId, AudioResource pResources)
            {
                _voice = pVoice;
                _resource = pResources;
                _id = pId;
            }

            public void AddCallback(bool pLoop)
            {
                if (pLoop) _voice.BufferEnd += OnLoopRestart;
                else _voice.BufferEnd += OnSoundFinished;
            }

            private void OnLoopRestart(IntPtr arg)
            {
                if(_playingSounds.ContainsKey(_id)) _resource.Play(_voice);
                else
                {
                    _voice.BufferEnd -= OnLoopRestart;
                    ReuseVoice(_id, ref _voice);
                }
            }

            private void OnSoundFinished(IntPtr arg)
            {
                _voice.BufferEnd -= OnSoundFinished;
                ReuseVoice(_id, ref _voice);
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
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX.XAudio2;
using SharpDX.Multimedia;

namespace SmallEngine.Audio
{
    public class AudioResource : Resource
    {
        public delegate void SoundEndEventHandler(object pSender, SoundEventArgs pInt);
        public event SoundEndEventHandler SoundEnd;

        private bool _currentlyLooping;
        private int _loopCount;
        private int _loopCounter;

        private AudioBuffer _buffer;
        private SourceVoice _voice;
        internal WaveFormat Stream;
        static List<SourceVoice> _freeVoices;
        static MasteringVoice _mv;

        #region Resource Functions
        internal override void Create()
        {
            LoadSound(Path);
        }

        internal override Task CreateAsync()
        {
            return Task.Run(() => LoadSound(Path));
        }

        private void LoadSound(string pPath)
        {
            var wasPlaying = IsPlaying;
            Stop();
            Initialize(pPath);
            if (wasPlaying) Play();
        }

        public override void Dispose()
        {
            if (_voice != null)
            {
                _voice.Stop();
                _voice.FlushSourceBuffers();
                _voice.Dispose();
            }
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Current state of the sound.
        /// </summary>
        public AudioState State { get; protected set; } = AudioState.Stopped;

        /// <summary>
        /// Encoding for the stream
        /// </summary>
        public WaveFormatEncoding Encoding
        {
            get { return Stream.Encoding; }
        }

        /// <summary>
        /// Returns if the sound is currently playing.
        /// </summary>
        public bool IsPlaying
        {
            get
            {
                return State == AudioState.Playing || State == AudioState.Repeating;
            }
        }

        /// <summary>
        /// Retruns if the sound is currently looping.
        /// </summary>
        public bool IsRepeating
        {
            get { return _currentlyLooping; }
        }

        /// <summary>
        /// Returns the number of channels of the audio clip.
        /// </summary>
        public int Channels
        {
            get { return _voice.VoiceDetails.InputChannelCount; }
        }

        /// <summary>
        /// Returns the sample rate of the audio clip.
        /// </summary>
        public int SampleRate
        {
            get { return Stream.SampleRate; }
        }

        /// <summary>
        /// Packets info of the sound
        /// </summary>
        public uint[] DecodedPacketsInfo { get; set; }

        private float _volume;
        /// <summary>
        /// Gets or sets the audio volume.
        /// </summary>
        public float Volume
        {
            get { return _volume; }
            set
            {
                _volume = value;
                if (_voice != null)
                {
                    _voice.SetVolume(_volume);
                    Device.CommitChanges();
                }
            }
        }

        /// <summary>
        /// Minumum volume of any clip
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

        /// <summary>
        /// Returns the XAudio2 device.
        /// </summary>
        public static XAudio2 Device { get; private set; }

        /// <summary>
        /// Gets or sets master volume.
        /// </summary>
        public static float MasterVolume
        {
            get { return _mv.Volume; }
            set { _mv.SetVolume(value); }
        }
        #endregion

        private void Initialize(string pFileName)
        {
            SoundStream soundStream;
            System.IO.FileStream s;
            System.Diagnostics.Debug.Assert(System.IO.File.Exists(pFileName));
            s = System.IO.File.OpenRead(pFileName);
            soundStream = new SoundStream(s);

            //Create a audio buffer from the file data
            _buffer = new AudioBuffer()
            {
                Stream = soundStream,
                AudioBytes = (int)soundStream.Length,
                Flags = BufferFlags.EndOfStream
            };

            DecodedPacketsInfo = soundStream.DecodedPacketsInfo;
            Stream = soundStream.Format;

            _currentlyLooping = false;

            Volume = MaxVolume;

            State = AudioState.Stopped;
        }

        #region Public functions
        public bool Play()
        {
            if (IsPlaying) return false;

            //Get a new voice
            GetVoice(this, out _voice);
            _voice.BufferEnd += OnSoundEnd;

            //Play sound
            _voice.SubmitSourceBuffer(_buffer, DecodedPacketsInfo);
            _voice.Start();
            State = AudioState.Playing;

            return true;
        }

        /// <summary>
        /// Plays the audio clip asynchronously.
        /// </summary>
        public void PlayImmediate()
        {
            //Get a local voice
            SourceVoice voice;
            GetVoice(this, out voice);
            voice.BufferEnd += OnSoundEnd;

            //Clear anything from its buffers (shouldnt be anything)
            voice.FlushSourceBuffers();

            //Play
            voice.SubmitSourceBuffer(_buffer, DecodedPacketsInfo);
            voice.Start();
            State = AudioState.Playing;
        }

        /// <summary>
        /// Immediately stops the playing sound.
        /// </summary>
        public bool Stop()
        {
            if (!IsPlaying) return false;

            _voice.Stop();
            _voice.FlushSourceBuffers();
            State = AudioState.Stopped;

            return true;
        }

        /// <summary>
        /// Pauses the playing sound.
        /// </summary>
        public void Pause()
        {
            if (!IsPlaying) return;

            _voice.Stop();
            State = AudioState.Paused;
        }

        /// <summary>
        /// Resumes the paused sound.
        /// </summary>
        public void Resume()
        {
            if (State != AudioState.Paused) return;

            _voice.Start();
            State = AudioState.Playing;
        }

        /// <summary>
        /// Loops the sound until <see cref="StopLoop"/> is called.
        /// </summary>
        public void Loop()
        {
            if (_currentlyLooping) return;

            _currentlyLooping = true;
            Play();
            State = AudioState.Repeating;
        }

        /// <summary>
        /// Loops the sound the specified number of times.
        /// </summary>
        /// <param name="pCount">Number of times to loop the clip</param>
        public void Loop(int pCount)
        {
            if (_currentlyLooping) return;

            _currentlyLooping = true;
            _loopCount = pCount;
            _loopCounter = 1;
            Play();
            State = AudioState.Repeating;
        }

        /// <summary>
        /// Stops the loop from playing after it has finished playing.
        /// </summary>
        public void StopLoop()
        {
            _currentlyLooping = false;
            _loopCount = 0;
            _loopCounter = 0;
        }

        /// <summary>
        /// Immediately stops the loop and sound from playing.
        /// </summary>
        public void StopLoopImmediate()
        {
            _currentlyLooping = false;
            _loopCount = 0;
            _loopCounter = 0;

            if (IsPlaying)
            {
                Stop();
                State = AudioState.Stopped;
            }
        }

        private void OnSoundEnd(IntPtr pInt)
        {
            var voice = new SourceVoice(pInt);
            voice.BufferEnd -= OnSoundEnd;

            //if we are looping
            if (_currentlyLooping)
            {
                //Check if we have a loop counter and we should continue looping
                if (_loopCounter++ <= _loopCount)
                {
                    Play();
                }
                else //We are done looping
                {
                    _currentlyLooping = false;
                    _loopCount = 0;
                    _loopCounter = 0;

                    //Rethrow event for other classes to handle
                    SoundEnd?.Invoke(this, new SoundEventArgs(pInt));

                    State = AudioState.Stopped;
                }
            }
            else
            {
                //Rethrow event
                SoundEnd?.Invoke(this, new SoundEventArgs(pInt));
                State = AudioState.Stopped;
            }
        }
        #endregion

        #region "Static functions"
        static AudioResource()
        {
            Device = new XAudio2();
            _mv = new MasteringVoice(Device);
            _mv.SetVolume(MaxVolume);
            _freeVoices = new List<SourceVoice>();
        }

        /// <summary>
        /// Gets a free voice from the voice pool.  If none are available a new one is created
        /// </summary>
        /// <param name="pSound">Sound to play with the voice</param>
        public static void GetVoice(AudioResource pSound, out SourceVoice pVoice)
        {
            lock (_freeVoices)
            {
                if (_freeVoices.Count == 0)
                {
                    pVoice = new SourceVoice(Device, pSound.Stream, true);
                    pVoice.BufferEnd += (new SoundCompleteCallback(pVoice)).OnSoundFinished;
                }
                else
                {
                    pVoice = _freeVoices[_freeVoices.Count - 1];
                    _freeVoices.RemoveAt(_freeVoices.Count - 1);
                }
            }

            if (pVoice.Volume != pSound.Volume)
            {
                pVoice.SetVolume(pSound.Volume);
                Device.CommitChanges();
            }
        }

        /// <summary>
        /// Puts the voice back into the pool of eligible free voices
        /// </summary>
        /// <param name="pVoice">Voice to mark for reuse</param>
        public static void ReuseVoice(ref SourceVoice pVoice)
        {
            lock (_freeVoices)
            {
                _freeVoices.Add(pVoice);
            }
        }
        #endregion

        #region "Disposable support"
        public static void DisposeVoices()
        {
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

        #region "Callback"
        protected struct SoundCompleteCallback
        {
            private SourceVoice _voice;
            public SoundCompleteCallback(SourceVoice pVoice)
            {
                _voice = pVoice;
            }
            public void OnSoundFinished(IntPtr arg)
            {
                _voice.BufferEnd -= OnSoundFinished;
                ReuseVoice(ref _voice);
            }
        }
        #endregion
    }
}
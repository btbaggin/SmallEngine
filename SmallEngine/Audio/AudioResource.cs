using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX.XAudio2;
using SharpDX.Multimedia;

namespace SmallEngine.Audio
{
    public class SoundEventArgs : EventArgs
    {
        public IntPtr Pointer { get; private set; }
        public SoundEventArgs(IntPtr pPointer)
        {
            Pointer = pPointer;
        }
    }

    public class AudioResource : Resource
    {
        //TODO look at sound playing properly-
        public delegate void SoundEndEventHandler(object pSender, SoundEventArgs pInt);
        public event SoundEndEventHandler SoundEnd;

        protected AudioBuffer Buffer;
        protected SourceVoice Voice;
        internal WaveFormat Stream;
        internal static List<SourceVoice> FreeVoices;
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
            if (IsPlaying)
            {
                Stop();
            }
        }
        #endregion

        #region Protected properties
        protected bool CurrentlyLooping { get; set; }

        protected int LoopCount { get; set; }

        protected int LoopCounter { get; set; }

        protected float FadeTimer { get; set; }

        protected string FileName { get; set; }
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
            get { return CurrentlyLooping; }
        }

        /// <summary>
        /// Returns the number of channels of the audio clip.
        /// </summary>
        public int Channels
        {
            get { return Voice.VoiceDetails.InputChannelCount; }
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
                if (Voice != null)
                {
                    Voice.SetVolume(_volume);
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
            //Read the file into our stream
            FileName = pFileName;

            SoundStream soundStream;
            System.IO.FileStream s;
            System.Diagnostics.Debug.Assert(System.IO.File.Exists(pFileName));
            s = System.IO.File.OpenRead(pFileName);
            soundStream = new SoundStream(s);

            //Create a audio buffer from the file data
            Buffer = new AudioBuffer()
            {
                Stream = soundStream,
                AudioBytes = (int)soundStream.Length,
                Flags = BufferFlags.EndOfStream
            };

            DecodedPacketsInfo = soundStream.DecodedPacketsInfo;
            Stream = soundStream.Format;

            CurrentlyLooping = false;
            LoopCount = -1;
            LoopCounter = -1;

            Volume = MaxVolume;

            State = AudioState.Stopped;
        }

        #region Abstract functions
        public bool Play()
        {
            if (IsPlaying) return false;

            //Get a new voice
            GetVoice(this);
            Voice.BufferEnd += OnSoundEnd;

            //Play sound
            Voice.SubmitSourceBuffer(Buffer, DecodedPacketsInfo);
            Voice.Start();
            State = AudioState.Playing;

            return true;
        }
        /// <summary>
        /// Plays the audio clip.  Will not again if it is currently playing.
        /// </summary>
        public bool Play(float pVolume)
        {
            Volume = pVolume;
            return Play();
        }

        /// <summary>
        /// Plays the audio clip asynchronously.
        /// </summary>
        public void PlayASync()
        {
            //Get a local voice
            GetVoice(this);//new SourceVoice(SoundManager.Device, Stream);
            Voice.BufferEnd += OnSoundEnd;

            //Clear anything from its buffers (shouldnt be anything)
            Voice.FlushSourceBuffers();

            //Play
            Voice.SubmitSourceBuffer(Buffer, DecodedPacketsInfo);
            Voice.Start();

            //Set the call back so that the voice can be recycled
            Voice.BufferEnd += (new SoundCompleteCallback(Voice, this)).OnSoundFinished;
        }

        /// <summary>
        /// Immediately stops the playing sound.
        /// </summary>
        public bool Stop()
        {
            if (!IsPlaying) return false;

            Voice.Stop();
            Voice.FlushSourceBuffers();
            State = AudioState.Stopped;

            return true;
        }

        /// <summary>
        /// Pauses the playing sound.
        /// </summary>
        public void Pause()
        {
            if (!IsPlaying) return;

            Voice.Stop();
            State = AudioState.Paused;
        }

        /// <summary>
        /// Resumes the paused sound.
        /// </summary>
        public void Resume()
        {
            if (State != AudioState.Paused) return;

            Voice.Start();
            State = AudioState.Playing;
        }

        /// <summary>
        /// Loops the sound until <see cref="StopLoop"/> is called.
        /// </summary>
        public void Loop()
        {
            if (CurrentlyLooping) return;

            CurrentlyLooping = true;
            Play();
            State = AudioState.Repeating;
        }

        /// <summary>
        /// Loops the sound the specified number of times.
        /// </summary>
        /// <param name="pCount">Number of times to loop the clip</param>
        public void Loop(int pCount)
        {
            if (CurrentlyLooping) return;

            CurrentlyLooping = true;
            LoopCount = pCount;
            LoopCounter = 1;
            Play();
            State = AudioState.Repeating;
        }

        /// <summary>
        /// Stops the loop from playing after it has finished playing.
        /// </summary>
        public void StopLoop()
        {
            CurrentlyLooping = false;
            LoopCount = -1;
            LoopCounter = -1;
        }

        /// <summary>
        /// Immediately stops the loop and sound from playing.
        /// </summary>
        public void StopLoopImmediate()
        {
            CurrentlyLooping = false;
            LoopCount = -1;
            LoopCounter = -1;

            if (IsPlaying)
            {
                Stop();
                State = AudioState.Stopped;
            }
        }

        /// <summary>
        /// Fades the volume in to <see cref=" pEndingVolume"/> over the specified number of steps and duration.
        /// </summary>
        /// <param name="pDeltaTime">Delta time</param>
        /// <param name="pStep">Number of steps to fade</param>
        /// <param name="pDuration">Duration to fade in milliseconds</param>
        /// <param name="pEndingVolume">Volume to fade to</param>
        public void FadeIn(float pDeltaTime, float pStep, float pDuration, float pEndingVolume)
        {
            if (FadeTimer > pDuration * pStep && Volume < pEndingVolume)
            {
                Volume += pStep;
                FadeTimer = 0;
            }
            else
                FadeTimer += pDeltaTime;
        }

        /// <summary>
        /// Fades the volume out to <see cref="MinVolume"/> over the specified number of steps and duration.
        /// </summary>
        /// <param name="pDeltaTime">Delta time</param>
        /// <param name="pStep">Number of steps to fade</param>
        /// <param name="pDuration">Duration to fade in milliseconds</param>
        /// <param name="pEndingVolume">Volume to fade to</param>
        public void FadeOut(float pDeltaTime, float pStep, float pDuration, float pEndingVolume)
        {
            if (FadeTimer > pDuration * pStep && Volume > pEndingVolume)
            {
                Volume -= pStep;
                FadeTimer = 0;
            }
            else
                FadeTimer += pDeltaTime;
        }

        private void OnSoundEnd(IntPtr pInt)
        {
            Voice.BufferEnd -= OnSoundEnd;
            ReuseVoice(ref Voice, this);
            //if we are looping
            if (CurrentlyLooping)
            {
                //Check if we have a loop counter and we should continue looping
                if (LoopCount == -1)
                {
                    Play();
                }
                else if (LoopCounter + 1 <= LoopCount)
                {
                    LoopCounter++;
                    Play();
                }
                else //We are done looping
                {
                    CurrentlyLooping = false;
                    LoopCount = -1;
                    LoopCounter = -1;

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
            FreeVoices = new List<SourceVoice>();
        }

        /// <summary>
        /// Gets a free voice from the voice pool.  If none are available a new one is created
        /// </summary>
        /// <param name="pSound">Sound to play with the voice</param>
        public static void GetVoice(AudioResource pSound)
        {
            SourceVoice voice;
            lock (FreeVoices)
            {
                if (FreeVoices.Count == 0)
                {
                    voice = new SourceVoice(Device, pSound.Stream, true);
                }
                else
                {
                    voice = FreeVoices[FreeVoices.Count - 1];
                    FreeVoices.RemoveAt(FreeVoices.Count - 1);
                }
            }

            if(voice.Volume != pSound.Volume)
            {
                voice.SetVolume(pSound.Volume);
                Device.CommitChanges();
            }
            pSound.Voice = voice;
        }

        /// <summary>
        /// Puts the voice back into the pool of eligible free voices
        /// </summary>
        /// <param name="pVoice">Voice to mark for reuse</param>
        public static void ReuseVoice(ref SourceVoice pVoice, AudioResource pSound)
        {
            lock (FreeVoices)
            {
                FreeVoices.Add(pVoice);
            }
        }
        #endregion

        #region "Disposable support"
        public static void DisposeVoices()
        {
            lock (FreeVoices)
            {
                for (int i = 0; i < FreeVoices.Count; i++)
                {
                    FreeVoices[i].Dispose();
                }
            }

            Device.Dispose();
        }
        #endregion

        public override string ToString()
        {
            return FileName;
        }

        #region "Callback"
        protected struct SoundCompleteCallback
        {
            private SourceVoice _voice;
            private readonly AudioResource _sound;
            public SoundCompleteCallback(SourceVoice pVoice, AudioResource pSound)
            {
                _voice = pVoice;
                _sound = pSound;
            }
            public void OnSoundFinished(IntPtr arg)
            {
                ReuseVoice(ref _voice, _sound);
            }
        }
        #endregion
    }
}

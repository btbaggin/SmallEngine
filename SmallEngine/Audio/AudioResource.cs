using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using SharpDX.XAudio2;
using SharpDX.Multimedia;
using System.Runtime.Serialization;

namespace SmallEngine.Audio
{
    /// <summary>
    /// Provides a way to play audio files
    /// </summary>
    [Serializable]
    public sealed class AudioResource : Resource
    {
        [NonSerialized] AudioBuffer _buffer;
        [NonSerialized] internal WaveFormat Stream;

        #region Public properties
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
        public uint[] DecodedPacketsInfo { get; private set; }
        #endregion

        #region Resource Functions
        /// <inheritdoc/>
        public override void Create()
        {
            LoadSound(Path);
        }

        /// <inheritdoc/>
        public override Task CreateAsync()
        {
            return Task.Run(() => LoadSound(Path));
        }

        private void LoadSound(string pPath)
        {
            Initialize(pPath);
        }

        protected override void DisposeResource()
        {
            _buffer.Stream.Dispose();
            Stream = null;
        }
        #endregion

        public AudioResource() { }

        private AudioResource(SerializationInfo pInfo, StreamingContext pContext) : base(pInfo, pContext)
        {
            if (!ResourceManager.ResourceLoaded(Alias))
                throw new Serialization.ResourceNotLoadedException(Alias);

            var a = ResourceManager.Request<AudioResource>(Alias);
            _buffer = a._buffer;
            Stream = a.Stream;
        }

        private void Initialize(string pFileName)
        {
            System.Diagnostics.Debug.Assert(System.IO.File.Exists(pFileName));

            System.IO.FileStream s = System.IO.File.OpenRead(pFileName);

            switch(Game.RenderMethod)
            {
                case Graphics.RenderMethods.DirectX:
                    SoundStream soundStream = new SoundStream(s);

                    //Create a audio buffer from the file data
                    _buffer = new AudioBuffer()
                    {
                        Stream = soundStream,
                        AudioBytes = (int)soundStream.Length,
                        Flags = BufferFlags.EndOfStream
                    };

                    DecodedPacketsInfo = soundStream.DecodedPacketsInfo;
                    Stream = soundStream.Format;
                    break;

                case Graphics.RenderMethods.OpenGL:
                    throw new NotImplementedException();

                default:
                    throw new UnknownEnumException(typeof(Graphics.RenderMethods), Game.RenderMethod);
            }
        }

        internal bool Play(SourceVoice pVoice)
        {
            //Clear anything from its buffers (shouldn't be anything)
            pVoice.FlushSourceBuffers();

            //Play
            pVoice.SubmitSourceBuffer(_buffer, DecodedPacketsInfo);
            pVoice.Start();

            return true;
        }
    }
}
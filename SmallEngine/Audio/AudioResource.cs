using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using SharpDX.XAudio2;
using SharpDX.Multimedia;

namespace SmallEngine.Audio
{
    public class AudioResource : Resource
    {
        private AudioBuffer _buffer;
        internal WaveFormat Stream;

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
            Initialize(pPath);
        }

        public override void Dispose()
        {
            _buffer.Stream.Dispose();
        }
        #endregion

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

        private void Initialize(string pFileName)
        {
            pFileName = System.IO.Path.GetFullPath(pFileName);
            System.Diagnostics.Debug.Assert(System.IO.File.Exists(pFileName));

            System.IO.FileStream s= System.IO.File.OpenRead(pFileName);
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
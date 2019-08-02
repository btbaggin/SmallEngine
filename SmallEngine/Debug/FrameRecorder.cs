using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Serialization;

namespace SmallEngine.Debug
{
    public static class FrameRecorder
    {
        struct FrameInfo
        {
            public Vector2 MousePosition;
            public byte[] Input;
            public float DeltaTime;
            public float TimeScale;
        }

        public static bool IsRecording { get; private set; }

        public static bool IsPlaying { get; private set; }

        static int _currentFrameIndex;
        static List<FrameInfo> _frames;
        internal static void SaveFrame(Vector2 pMousePosition, byte[] pInput, float pDeltaTime, float pTimescale)
        {
            if (!IsRecording) return;
            _frames.Add(new FrameInfo() { MousePosition = pMousePosition, Input = pInput, DeltaTime = pDeltaTime, TimeScale = pTimescale });
        }

        internal static void GetFrameInfo(out Vector2 pMousePosition, out byte[] pInput, out float pDeltaTime, out float pTimescale)
        {
            var fi = _frames[_currentFrameIndex++];
            if (_currentFrameIndex >= _frames.Count) IsPlaying = false;

            pMousePosition = fi.MousePosition;
            pInput = fi.Input;
            pDeltaTime = fi.DeltaTime;
            pTimescale = fi.TimeScale;
        }

        public static void StartRecording()
        {
            System.Diagnostics.Debug.Assert(!IsRecording, "Already recording");
            IsRecording = true;
            _frames = new List<FrameInfo>();
        }

        public static void EndRecording()
        {
            System.Diagnostics.Debug.Assert(IsRecording, "Not recording");
            //TODO write out to file
            IsRecording = false;
        }

        public static void Save(string pPath)
        {
            using (var sr = new System.IO.FileStream(pPath, System.IO.FileMode.Create))
            {
                for(int i = 0; i < _frames.Count; i++)
                {
                    var frame = _frames[i];
                    sr.WriteFloat(frame.MousePosition.X);
                    sr.WriteFloat(frame.MousePosition.Y);
                    sr.WriteBytes(frame.Input);
                    sr.WriteFloat(frame.DeltaTime);
                    sr.WriteFloat(frame.TimeScale);
                }
            }
        }

        public static void Play(string pPath)
        {
            _frames = new List<FrameInfo>();
            using (System.IO.Stream sr = new System.IO.FileStream(pPath, System.IO.FileMode.Open))
            {
                while(sr.Position < sr.Length)
                {
                    var x = sr.ReadFloat();
                    var y = sr.ReadFloat();
                    var input = sr.ReadBytes();
                    var dt = sr.ReadFloat();
                    var ts = sr.ReadFloat();

                    _frames.Add(new FrameInfo() { MousePosition = new Vector2(x, y), Input = input, DeltaTime = dt, TimeScale = ts });
                }
            }

            IsPlaying = true;
            _currentFrameIndex = 0;
        }
    }
}

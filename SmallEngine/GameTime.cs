using System;
using System.Diagnostics;

namespace SmallEngine
{
    public static class GameTime
    {
        static readonly double _secondsPerCount = 1d / Stopwatch.Frequency;
        static double _deltaTime = 0;
        static double _unscaleDeltaTime;
        static float _timeScale = 1f;
        static float _savedTimeScale;

        static long _startTime = 0;
        static long _lastTime = 0;
        static long _currentTime = 0;

        #region "Properties"
        /// <summary>
        /// Ticks elapsed since the last <see cref="Tick"/> call
        /// </summary>
        public static long ElapsedSinceTick
        {
            get { return Stopwatch.GetTimestamp() - _currentTime; }
        }

        /// <summary>
        /// Seconds that the game has been running
        /// </summary>
        public static float TotalTime
        {
            get
            {
                return (float)(_startTime * _secondsPerCount);
            }
        }

        /// <summary>
        /// Milliseconds between <see cref="Tick"/> calls
        /// </summary>
        public static float DeltaTime
        {
            get { return (float)_deltaTime; }
        }

        /// <summary>
        /// The amount of time leftover before a Draw call to be interpolated
        /// </summary>
        internal static float RenderTime { get; set; }

        /// <summary>
        /// The amount of time between each physics step
        /// </summary>
        internal static float PhysicsTime { get; set; }

        /// <summary>
        /// Milliseconds elapsed since <see cref="Tick"/> not affected by time scaling
        /// </summary>
        public static float UnscaledDeltaTime
        {
            get { return (float)_unscaleDeltaTime; }
        }

        /// <summary>
        /// Ticks that represents the current time
        /// </summary>
        public static long CurrentTime
        {
            get { return _currentTime; }
        }

        /// <summary>
        /// Gets if the game time is currently running
        /// </summary>
        public static bool Stopped { get; private set; }

        /// <summary>
        /// Gets or sets how fast time runs in relation to real time
        /// </summary>
        public static float TimeScale
        {
            get { return _timeScale; }
            set
            {
                if (value < 0) throw new ArgumentException("TimeScale");
                _timeScale = value;
            }
        }
        #endregion

        #region Public functions
        /// <summary>
        /// Ticks the clock to set delta time
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void Tick()
        {
            _currentTime = Stopwatch.GetTimestamp();
            _unscaleDeltaTime = (_currentTime - _lastTime) * _secondsPerCount;
            _deltaTime = _unscaleDeltaTime * TimeScale;
            _lastTime = _currentTime;
#if DEBUG
            _unscaleDeltaTime = Math.Min(_unscaleDeltaTime, .02);
            _deltaTime = Math.Min(_deltaTime, .02);
#endif
        }

        /// <summary>
        /// Rests all time counters
        /// </summary>
        public static void Reset()
        {
            var resetTime = Stopwatch.GetTimestamp();

            _startTime = resetTime;
            _lastTime = resetTime;
            _currentTime = resetTime;
            Stopped = false;
        }

        /// <summary>
        /// Stops the GameTime from doing normal ticks
        /// </summary>
        public static void Stop()
        {
            if (!Stopped)
            {
                _savedTimeScale = TimeScale;
                TimeScale = 0;
                Stopped = true;
            }
        }

        /// <summary>
        /// Starts the GameTime to do normal ticks
        /// </summary>
        public static void Start()
        {
            if (Stopped)
            {
                var lastTime = Stopwatch.GetTimestamp();
                TimeScale = _savedTimeScale;
                _lastTime = lastTime;
                Stopped = false;
            }
        }
        #endregion

        #region "Static functions"
        /// <summary>
        /// Converts clock ticks to minutes
        /// </summary>
        /// <param name="pTime">Clock ticks to convert</param>
        /// <returns>How many minutes the ticks represent</returns>
        public static float TickToMinutes(long pTime)
        {
            return (float)(pTime * _secondsPerCount * 60);
        }

        /// <summary>
        /// Converts clock ticks to seconds
        /// </summary>
        /// <param name="pTime">Clock ticks to convert</param>
        /// <returns>How many seconds the ticks represent</returns>
        public static float TickToSeconds(long pTime)
        {
            return (float)(pTime * _secondsPerCount);
        }

        /// <summary>
        /// Converts clock ticks to milliseconds
        /// </summary>
        /// <param name="pTime">Clock ticks to convert</param>
        /// <returns>How many milliseconds the ticks represent</returns>
        public static float TickToMillis(long pTime)
        {
            return (float)(pTime * _secondsPerCount * 1000);
        }

        /// <summary>
        /// Converts milliseconds to clock ticks
        /// </summary>
        /// <param name="pMillis">Milliseconds to convert</param>
        /// <returns>How many clock ticks the milliseconds represent</returns>
        public static long MillisToTick(float pMillis)
        {
            return (long)(pMillis * Stopwatch.Frequency / 1000);
        }

        /// <summary>
        /// Converts seconds to clock ticks
        /// </summary>
        /// <param name="pSeconds">Seconds to convert</param>
        /// <returns>How many clock ticks the seconds represent</returns>
        public static long SecondsToTick(float pSeconds)
        {
            return (long)(pSeconds * Stopwatch.Frequency);
        }

        /// <summary>
        /// Converts minutes to clock ticks
        /// </summary>
        /// <param name="pMinutes">Minutes to convert</param>
        /// <returns>How many clock ticks the minutes represent</returns>
        public static long MinutesToTick(float pMinutes)
        {
            return (long)(pMinutes * (Stopwatch.Frequency * 60));
        }
        #endregion
    }
}

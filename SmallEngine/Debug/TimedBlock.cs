using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace SmallEngine.Debug
{
    struct TimedBlock : IDisposable //TODO change to ref struct with C# 8.0
    {
        static DebugLog _log; //TODO something else
        short _headerIndex;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TimedBlock(uint pHits, string pFile, string pMethod, int pLine)
        {
            _headerIndex = 0;
            StartLog(pFile, pMethod, pLine);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimedBlock Start([CallerFilePath] string pFile = "", [CallerMemberName] string pMethod = "", [CallerLineNumber] int pLine = 0)
        {
            return new TimedBlock(1, pFile, pMethod, pLine);
        }

        [Conditional("TRACE")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void StartLog(string pFile, string pMethod,  int pLine)
        {
            _headerIndex = _log.GetOrAddHeader(pFile, pMethod, pLine);
            _log.AddEvent(DebugLogTypes.Start, _headerIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void IDisposable.Dispose()
        {
            EndLog();
        }

        [Conditional("TRACE")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EndLog()
        {
            _log.AddEvent(DebugLogTypes.End, _headerIndex);
        }
    }
}

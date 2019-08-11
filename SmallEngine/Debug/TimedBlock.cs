using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace SmallEngine.Debug
{
    public struct TimedBlock : IDisposable //TODO change to ref struct with C# 8.0
    {
        short _headerIndex;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TimedBlock(uint pHits, string pFile, string pMethod, int pLine, string pAlias)
        {
            _headerIndex = 0;
            StartLog(pFile, pMethod, pLine, pAlias);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimedBlock Start(string pAlias = "", [CallerFilePath] string pFile = "", [CallerMemberName] string pMethod = "", [CallerLineNumber] int pLine = 0)
        {
            return new TimedBlock(1, pFile, pMethod, pLine, pAlias);
        }

        [Conditional("TRACE")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void StartLog(string pFile, string pMethod,  int pLine, string pAlias)
        {
            _headerIndex = DebugLog.GetOrAddHeader(pFile, pMethod, pLine, pAlias);
            DebugLog.LogEvent(_headerIndex, DebugLogTypes.Start);
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
            DebugLog.LogEvent(_headerIndex, DebugLogTypes.End);
        }
    }
}

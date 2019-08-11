using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace SmallEngine.Debug
{
    struct DebugLogHeader
    {
        public int Line;
        public string FileName;
        public string Method;
        public string Alias;
        public DebugLogHeader(string pFile, string pMethod, int pLine, string pAlias)
        {
            FileName = pFile;
            Method = pMethod;
            Line = pLine;
            Alias = pAlias;
        }
    }

    struct DebugLogEvent
    {
        public long Clock;
        public DebugLogTypes Type;
        public short HeaderIndex;
        public int ThreadIndex;

        public DebugLogEvent(long pClock, DebugLogTypes pType, short pHeaderIndex, int pThreadIndex)
        {
            Clock = pClock;
            Type = pType;
            HeaderIndex = pHeaderIndex;
            ThreadIndex = pThreadIndex;
        }
    }

    public enum DebugLogTypes : byte
    {
        Start,
        End,
        Message
    }

    static class DebugLog
    {
        public readonly static DebugLogHeader[] Headers = new DebugLogHeader[16384];

        static int _headerCount;
        public static int HeaderCount => _headerCount;

        static readonly Dictionary<string, int> _headerMap = new Dictionary<string, int>();
        const int MAX_DEBUG_EVENTS = ushort.MaxValue;
        static readonly DebugLogEvent[][] _events = new DebugLogEvent[2][]
        {
            new DebugLogEvent[MAX_DEBUG_EVENTS],
            new DebugLogEvent[MAX_DEBUG_EVENTS]
        };
        static long _arrayIndex_EventIndex = 0;

        public static short GetOrAddHeader(string pFile, string pMethod, int pLine, string pAlias)
        {
            string methodLine = string.Intern(pMethod + pLine);
            lock(_headerMap)
            {
                if (!_headerMap.ContainsKey(methodLine))
                {
                    var header = Interlocked.Increment(ref _headerCount);
                    header--;
                    _headerMap.Add(methodLine, header);
                    Headers[header] = new DebugLogHeader(pFile, pMethod, pLine, pAlias);
                    return (short)header;
                }
                else
                {
                    return (short)_headerMap[methodLine];
                }
            }
        }

        internal static uint LogEvent(short pHeaderIndex, DebugLogTypes pType)
        {
            var array_Event = Interlocked.Increment(ref _arrayIndex_EventIndex);
            array_Event--;
            uint array = (uint)(array_Event >> 32);
            uint index = (uint)array_Event;
            System.Diagnostics.Debug.Assert(index < MAX_DEBUG_EVENTS);

            _events[array][index] = new DebugLogEvent(Stopwatch.GetTimestamp(), pType, pHeaderIndex, Thread.CurrentThread.ManagedThreadId);
            return index;
        }

        public static DebugLogEvent[] GetEvents(out uint pCount)
        {
            //Get current array index
            //Toggle between 0 and 1
            //Bit shift... event index would be 0

            //We don't need to worry about the array index being thread-safe since we are the only one who modifies it
            uint array = (uint)(_arrayIndex_EventIndex >> 32);
            long newArray_Event = array ^ 1;
            newArray_Event = newArray_Event << 32;

            //Toggle to new array, get count from old array
            long array_event = Interlocked.Exchange(ref _arrayIndex_EventIndex, newArray_Event);
            pCount = (uint)array_event;

            return _events[array];
        }
    }
}

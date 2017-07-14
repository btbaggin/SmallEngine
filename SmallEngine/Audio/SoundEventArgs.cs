using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
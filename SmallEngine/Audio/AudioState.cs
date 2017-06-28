using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Audio
{
    public enum AudioState
    {
        /// <summary>
        /// Sound is currently playing
        /// </summary>
        Playing,

        /// <summary>
        /// Sound is not playing
        /// </summary>
        Stopped,

        /// <summary>
        /// Sound is paused
        /// </summary>
        Paused,

        /// <summary>
        /// Sound is playing in a loop
        /// </summary>
        Repeating
    }
}

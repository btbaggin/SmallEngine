using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Audio
{
    public class FadeEffects
    {
        float _fadeTimer;
        float _duration;
        AudioResource _sound;
        public FadeEffects(AudioResource pSound, float pDuration)
        {
            _sound = pSound;
            _duration = pDuration;
        }

        /// <summary>
        /// Fades the volume in to <see cref=" pEndingVolume"/> over the specified number of steps and duration.
        /// </summary>
        /// <param name="pDeltaTime">Delta time</param>
        /// <param name="pStep">Number of steps to fade</param>
        /// <param name="pDuration">Duration to fade in milliseconds</param>
        /// <param name="pEndingVolume">Volume to fade to</param>
        public void FadeIn(float pDeltaTime, float pStep, float pEndingVolume)
        {
            if ((_fadeTimer += pDeltaTime) > _duration * pStep && _sound.Volume < pEndingVolume)
            {
                MathF.Lerp(_sound.Volume, pEndingVolume, pStep);
                _fadeTimer = 0;
            }
        }

        /// <summary>
        /// Fades the volume out to <see cref="MinVolume"/> over the specified number of steps and duration.
        /// </summary>
        /// <param name="pDeltaTime">Delta time</param>
        /// <param name="pStep">Number of steps to fade</param>
        /// <param name="pDuration">Duration to fade in milliseconds</param>
        /// <param name="pEndingVolume">Volume to fade to</param>
        public void FadeOut(float pDeltaTime, float pStep, float pEndingVolume)
        {
            if ((_fadeTimer += pDeltaTime) > _duration * pStep && _sound.Volume > pEndingVolume)
            {
                MathF.Lerp(_sound.Volume, pEndingVolume, pStep);
                _fadeTimer = 0;
            }
        }
    }
}
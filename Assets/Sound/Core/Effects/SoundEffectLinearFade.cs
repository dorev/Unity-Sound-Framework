using System;
using System.Collections;
using UnityEngine;

namespace Sound
{
    public class SoundEffectLinearFade : ISoundEffect
    {
        public int id => typeof(SoundEffectLinearFade).GetHashCode();
        public string name => typeof(SoundEffectLinearFade).Name;
        public SoundEffectState state { get; private set; } = SoundEffectState.Starting;
        public SoundEffectDelegate onFailed { get; set; }
        public SoundEffectDelegate onFinished { get; set; }

        public float startVolume;
        public float endVolume;
        public float fadeDuration;

        private float _startTime = 0f;
        private float _currentVolume = 0f;
        private bool _needsProcessing = true;

        public void Update(SoundInstance soundInstance)
        {
            if (!_needsProcessing)
            {
                return;
            }

            if (!soundInstance.IsValid)
            {
                onFailed(this, $"Invalid {soundInstance.GetType().Name} processing requested to {name}.");
                _needsProcessing = false;
                return;
            }

            if (state == SoundEffectState.Starting)
            {
                _startTime = Time.time;
                state = SoundEffectState.Processing;
            }

            if (state == SoundEffectState.Processing)
            {
                if (fadeDuration > 0f)
                {
                    float elapsed = Time.time - _startTime;
                    _currentVolume = Mathf.Lerp(startVolume, endVolume, elapsed / fadeDuration);
                    soundInstance.audioSource.volume = _currentVolume;

                    if (_currentVolume == endVolume)
                    {
                        state = SoundEffectState.Finished;
                    }
                }
                else
                {
                    Utils.HandleWarning($"{this.GetType().Name} was requested with a 0.0s fade duration.");
                    state = SoundEffectState.Finished;
                }
            }

            if (state == SoundEffectState.Finished)
            {
                onFinished?.Invoke(this);
                _needsProcessing = false;
            }
        }

        public bool Replace(ISoundEffect effect)
        {
            if (effect == null)
            {
                Utils.HandleWarning($"{name} was disabled because it was replaced by 'null'. This is allowed but not recommended.");
                _needsProcessing = false;
                onFinished?.Invoke(this);
                return true;
            }

            SoundEffectLinearFade otherFade = effect as SoundEffectLinearFade;

            state = SoundEffectState.Starting;
            startVolume = _currentVolume;
            endVolume = otherFade.endVolume;
            fadeDuration = otherFade.fadeDuration;
            return true;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace Sound
{
    public delegate void SoundInstanceDelegate(SoundInstance instance);

    public class SoundInstance
    {
        public const ulong InvalidId = 0;
        public readonly ulong id;
        public readonly SoundDataSO soundData;
        public AudioSource audioSource;
        public AudioMixerGroup outputMixerGroup;
        public List<SoundControl> soundControls;
        public SoundEmitter soundEmitter;
        public SoundVariation soundVariation;
        public AudioSourceConfigSO audioSourceConfig;

        public SoundInstanceDelegate onPlay;
        public SoundInstanceDelegate onPause;
        public SoundInstanceDelegate onStopped;
        public SoundInstanceDelegate onFail;
        public SoundRequestDelegate onStoppedFromRequest;

        public enum State
        {
            Error,
            Initializing,
            Playing,
            Paused,
            Stopping,
            Stopped
        }

        public State state { private set; get; } = State.Initializing;
        public bool InError => state == State.Error;
        public bool IsInitializing => state == State.Initializing;
        public bool IsPlaying => state == State.Playing;
        public bool IsPaused => state == State.Paused;
        public bool IsStopping => state == State.Stopping;
        public bool IsStopped => state == State.Stopped;
        public float configuredVolume => GetConfiguredVolume();
        private float _configuredVolumeCache = float.NaN;
        public float currentVolume => GetCurrentVolume();
        public bool IsValid => audioSource != null && audioSource.clip != null;

        private List<ISoundEffect> _effects = new List<ISoundEffect>();

        public SoundInstance(SoundRequest soundRequest)
        {
            id = soundRequest.id;
            soundData = soundRequest.soundData;
            outputMixerGroup = soundRequest.mixerGroup;
            soundVariation = soundRequest.variation;
            soundEmitter = soundRequest.soundEmitter;
            audioSourceConfig = soundRequest.audioSourceConfig;
            soundControls = soundRequest.soundControls;
            onStoppedFromRequest = soundRequest.onStop;
        }

        public void Update()
        {
            if (IsValid && IsPlaying && !audioSource.isPlaying)
            {
                Stop();
            }
            else
            {
                UpdateEffects();
            }
        }

        public void Play()
        {
            if (IsInitializing)
            {
                Utils.AssertNotNull(soundEmitter, "Cannot play SoundInstance without a SoundEmitter.");

                audioSource = soundEmitter.ProvideAudioSource(id, audioSourceConfig);

                if (audioSource != null)
                {
                    onStopped += soundEmitter.RecycleAudioSource;
                    onFail += soundEmitter.RecycleAudioSource;

                    if (soundData.audioClip != null)
                    {
                        FirstPlay();
                        return;
                    }
                    else
                    {
                        Utils.HandleError($"No AudioClip to play on SoundData {soundData.name}.");
                    }
                }
                else
                {
                    Utils.HandleError($"No AudioSource provided by AudioSourcePool on SoundEmitter {soundEmitter.name}.");
                }
            }
            else
            {
                switch (state)
                {
                    default:
                    case State.Initializing:
                    case State.Error:
                    case State.Stopping:
                    case State.Stopped:
                        Utils.HandleError($"Unexpected Play call, current state is {state}.");
                        break;

                    case State.Playing:
                        Utils.HandleWarning($"Unnecessary Play call, already playing.");
                        return;

                    case State.Paused:
                        ResumeFromPause();
                        return;
                }
            }

            state = State.Error;
            onFail?.Invoke(this);
            onStoppedFromRequest?.Invoke();
        }

        public void Pause()
        {
            if (IsValid && IsPlaying)
            {
                audioSource.Pause();
                state = State.Paused;
                onPause?.Invoke(this);
            }
            else
            {
                Utils.HandleWarning($"Unnecessary call to SoundInstance.Pause, state is currently {state}.");
            }
        }

        public void Stop(float fadeDuration = 0.1f)
        {
            if (IsValid && (IsPlaying || IsPaused))
            {
                state = State.Stopping;

                if (fadeDuration == 0f)
                {
                    Kill();
                }
                else
                {
                    AddEffect(new SoundEffectLinearFade
                    {
                        endVolume = 0f,
                        fadeDuration = fadeDuration,
                        onFinished = (_, _) => { Kill(); }
                    });
                }
            }
            else
            {
                Utils.HandleWarning($"Unnecessary call to {GetType().Name}.Stop, state is currently {state}.");
            }
        }

        public void Kill()
        {
            if (audioSource != null)
            {
                audioSource.Stop();
                soundEmitter.RecycleAudioSource(this);
                state = State.Stopped;
                onStopped?.Invoke(this);
                onStoppedFromRequest?.Invoke();
            }
        }

        // Get the 'final' volume at which the sound should be played
        public float GetConfiguredVolume()
        {
            if (!IsValid)
            {
                return float.NaN;
            }

            if (!float.IsNaN(_configuredVolumeCache))
            {
                return _configuredVolumeCache;
            }

            float targetDecibel = soundData.dB;

            if (soundVariation != null)
            {
                targetDecibel += soundVariation.dB;
            }

            if (audioSourceConfig != null)
            {
                targetDecibel += Utils.DecibelFromRatio(audioSourceConfig.volume);
            }

            _configuredVolumeCache = Utils.DecibelToRatio(targetDecibel);

            return _configuredVolumeCache;
        }

        public float GetCurrentVolume()
        {
            return IsValid ? audioSource.volume : 0f;
        }

        public void AddEffect<T>(T effectToAdd) where T : ISoundEffect
        {
            ISoundEffect effectFound = _effects.FirstOrDefault(effect => effect.id == effectToAdd.id);

            if (effectFound != null)
            {
                if (effectFound.Replace(effectToAdd))
                {
                    return;
                }

                _effects.Remove(effectFound);
            }

            _effects.Add(effectToAdd);
        }

        private void FirstPlay()
        {
            Utils.ConfigureOrderly(audioSource, soundData, soundVariation, audioSourceConfig, outputMixerGroup);

            if (soundControls != null)
            {
                foreach(SoundControl soundControl in soundControls)
                {
                    AddEffect(soundControl);
                }
            }

            UpdateEffects();
            audioSource.Play();
            state = State.Playing;
            onPlay?.Invoke(this);
        }

        private void ResumeFromPause()
        {
            UpdateEffects();
            audioSource.Play();
            state = State.Playing;
            onPlay?.Invoke(this);
        }

        private void UpdateEffects()
        {
            foreach (ISoundEffect effect in _effects)
            {
                effect.Update(this);
            }
        }
    }
}

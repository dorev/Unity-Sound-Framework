using System.Collections.Generic;
using UnityEngine;

namespace Sound
{
    public class SoundEmitter : MonoBehaviour
    {
        public int preWarmSize = 1;
        public int minPoolSize = 8;
        public int maxPoolSize = 0; // 0 for no limit
        public bool isPlaying => _audioSourcePool.IsPlaying();

        private AudioSourcePool _audioSourcePool;
        private Dictionary<ulong, AudioSource> _audioSources = new Dictionary<ulong, AudioSource>();

        private void OnEnable()
        {
            if (_audioSourcePool == null)
            {
                _audioSourcePool = new AudioSourcePool
                {
                    gameObject = this.gameObject,
                    minPoolSize = minPoolSize,
                    maxPoolSize = maxPoolSize,
                    onOverflow = () => Utils.HandleWarning($"Max AudioSource count reached on SoundEmitter {gameObject.name}."),
                };

                _audioSourcePool.PreWarm(preWarmSize);
            }
        }

        private void OnDisable()
        {
            Kill();
        }

        public AudioSource ProvideAudioSource(ulong soundInstanceId, AudioSourceConfigSO audioSourceConfig)
        {
            Utils.Assert(soundInstanceId != SoundInstance.InvalidId, $"{GetType().Name} was requested an AudioSource for an invalid SoundInstance id.");

            AudioSource audioSource = _audioSourcePool.GetRecycledOrNewAudioSource();

            if (audioSource != null)
            {
                _audioSources.Add(soundInstanceId, audioSource);
                if (audioSourceConfig != null)
                {
                    audioSourceConfig.Apply(audioSource);
                }
            }

            return audioSource;
        }

        public AudioSource GetAudioSourceForSoundInstance(ulong soundInstanceId)
        {
            if (_audioSources.TryGetValue(soundInstanceId, out AudioSource audioSource))
            {
                return audioSource;
            }

            return null;
        }

        public void RecycleAudioSource(SoundInstance soundInstance)
        {
            _audioSourcePool.RecycleAudioSource(soundInstance.audioSource);
        }

        public void Kill()
        {
            RemoveAllEffects();
            _audioSourcePool.Shutdown();
        }

        private void RemoveAllEffects()
        {
            AudioChorusFilter chorus = gameObject.GetComponent<AudioChorusFilter>();
            if (chorus != null)
            {
                GameObject.Destroy(chorus);
            }

            AudioLowPassFilter lowPass = gameObject.GetComponent<AudioLowPassFilter>();
            if (lowPass != null)
            {
                GameObject.Destroy(lowPass);
            }

            AudioHighPassFilter highPass = gameObject.GetComponent<AudioHighPassFilter>();
            if (highPass != null)
            {
                GameObject.Destroy(highPass);
            }

            AudioReverbFilter reverb = gameObject.GetComponent<AudioReverbFilter>();
            if (reverb != null)
            {
                GameObject.Destroy(reverb);
            }

            AudioEchoFilter echo = gameObject.GetComponent<AudioEchoFilter>();
            if (echo != null)
            {
                GameObject.Destroy(echo);
            }

            AudioDistortionFilter distortion = gameObject.GetComponent<AudioDistortionFilter>();
            if (distortion != null)
            {
                GameObject.Destroy(distortion);
            }
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace Sound
{
    public delegate void AudioSourcePoolOverflowDelegate();
    public delegate void AudioSourceDelegate(AudioSource audioSource);

    public class AudioSourcePool
    {
        private List<AudioSource> _activeAudioSources = new List<AudioSource>();
        private List<AudioSource> _pendingAudioSources = new List<AudioSource>();

        public GameObject gameObject;
        public int minPoolSize = 8;
        public int maxPoolSize = 0;
        public AudioSourcePoolOverflowDelegate onOverflow;

        public void Shutdown()
        {
            ForEachAudioSource(audioSource => { audioSource.Stop(); });

            foreach (AudioSource audioSource in _activeAudioSources)
            {
                if (audioSource != null)
                {
                    GameObject.Destroy(audioSource);
                }
            }
            _activeAudioSources.Clear();

            foreach (AudioSource audioSource in _pendingAudioSources)
            {
                if (audioSource != null)
                {
                    GameObject.Destroy(audioSource);
                }
            }
            _pendingAudioSources.Clear();
        }

        public void RecyclePausedOrStoppedSources()
        {
            Utils.AssertNotNull(gameObject, "AudioSourcePool must have an associated GameObject to clean AudioSource instances.");

            List<int> audioSourceIndicesToRecycle = new List<int>();

            for (int i = 0; i < _activeAudioSources.Count; i++)
            {
                AudioSource audioSource = _activeAudioSources[i];
                if (audioSource != null && !audioSource.isPlaying)
                {
                    audioSourceIndicesToRecycle.Add(i);
                }
            }

            audioSourceIndicesToRecycle.Reverse();

            foreach (int i in audioSourceIndicesToRecycle)
            {
                if (_pendingAudioSources.Count < minPoolSize)
                {
                    AudioSourceConfigSO.Reset(_activeAudioSources[i]);
                    _pendingAudioSources.Add(_activeAudioSources[i]);
                }
                else
                {
                    GameObject.Destroy(_activeAudioSources[i]);
                }
                _activeAudioSources.RemoveAt(i);
            }
        }

        public void RecycleAudioSource(AudioSource audioSource)
        {
            if (_activeAudioSources.Contains(audioSource))
            {
                if (_pendingAudioSources.Count < minPoolSize)
                {
                    AudioSourceConfigSO.Reset(audioSource);
                    audioSource.enabled = false;
                    _activeAudioSources.Remove(audioSource);
                    _pendingAudioSources.Add(audioSource);
                }
                else
                {
                    _activeAudioSources.Remove(audioSource);
                    GameObject.Destroy(audioSource);
                }
            }
        }

        public AudioSource GetRecycledOrNewAudioSource()
        {
            Utils.AssertNotNull(gameObject, "AudioSourcePool must have an associated GameObject to manage AudioSource instances.");

            if (_pendingAudioSources.Count > 0)
            {
                int index = _pendingAudioSources.Count - 1;
                AudioSource audioSource = _pendingAudioSources[index];
                _pendingAudioSources.RemoveAt(index);
                _activeAudioSources.Add(audioSource);
                audioSource.enabled = true;
                return audioSource;
            }

            if (maxPoolSize != 0 && _activeAudioSources.Count >= maxPoolSize)
            {
                Utils.HandleWarning($"AudioSourcePool on {gameObject.name} has reached its maximal capacity.");
                onOverflow?.Invoke();
                return null;
            }
            else
            {
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                _activeAudioSources.Add(audioSource);
                audioSource.enabled = true;
                return audioSource;
            }
        }

        public void PreWarm(int preWarmSize)
        {
            if (_pendingAudioSources.Count > 0)
            {
                Utils.HandleWarning("AudioSourcePool.PreWarm called after it started operating.");
                return;
            }

            Utils.AssertNotNull(gameObject, "AudioSourcePool must have an associated GameObject to prewarm AudioSource instances.");

            if (maxPoolSize != 0 && preWarmSize > maxPoolSize)
            {
                preWarmSize = maxPoolSize;
            }

            for (int i = 0; i < preWarmSize; i++)
            {
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.enabled = false;
                _pendingAudioSources.Add(audioSource);
            }
        }

        public void ForEachAudioSource(AudioSourceDelegate action, bool onActiveAudioSources = true, bool onPendingAudioSources = false)
        {
            if (onActiveAudioSources)
            {
                foreach (AudioSource audioSource in _activeAudioSources)
                {
                    action(audioSource);
                }
            }

            if (onPendingAudioSources)
            {
                foreach (AudioSource audioSource in _pendingAudioSources)
                {
                    action(audioSource);
                }
            }
        }

        public bool IsPlaying()
        {
            foreach (AudioSource audioSource in _activeAudioSources)
            {
                if (!audioSource.isVirtual && audioSource.isPlaying && audioSource.volume > 0.0f)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
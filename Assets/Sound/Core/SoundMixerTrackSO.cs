using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Sound
{
    [CreateAssetMenu(fileName = "New SoundMixerTrack", menuName = "Sound/Sound Mixer Track")]
    public class SoundMixerTrackSO : ScriptableObject
    {
        public AudioMixerGroup mixerGroup;
        [HideInInspector] public SoundMixerSO mixer;

        // TO IMPLEMENT
        public bool mute;
        public bool solo;
        public int soundLimitingCount;
        public float soundLimitingRange;

        [SerializeField] private ulong id;
        private uint _nextId = 0;

        private void OnValidate()
        {
            id = mixerGroup == null ? 0 : ((ulong)mixerGroup.name.GetHashCode()) << 32;
        }

        public ulong PlaySound(
            SoundDataSO soundData,
            SoundVariation soundVariation = null,
            SoundEmitter soundEmitter = null,
            AudioSourceConfigSO audioSourceConfig = null,
            List<SoundControl> soundControls = null,
            float fadeInDuration = 0.0f,
            SoundRequestDelegate onStop = null)
        {
            Utils.AssertNotNull(mixer, $"No SoundMixer associated to SoundMixerTrack {name}");
            Utils.AssertNotNull(mixerGroup, $"No AudioMixerGroup associated to SoundMixerTrack {name}");

            return mixer.PlaySound(new SoundRequest
            {
                id = id | ++_nextId,
                soundData = soundData,
                variation = soundVariation,
                soundEmitter = soundEmitter,
                audioSourceConfig = audioSourceConfig,
                soundControls = soundControls,
                fadeInDuration = fadeInDuration,
                mixerGroup = mixerGroup,
                onStop = onStop
            });
        }

        public void Stop(ulong soundInstanceId, float fadeDuration = 0.0f)
        {
            Utils.AssertNotNull(mixer, $"No SoundMixer associated to SoundMixerTrack {name}");
            Utils.AssertNotNull(mixerGroup, $"No AudioMixerGroup associated to SoundMixerTrack {name}");

            mixer.StopSound(soundInstanceId, fadeDuration);
        }
    }
}

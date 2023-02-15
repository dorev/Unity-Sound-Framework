using System.Collections.Generic;
using UnityEngine.Audio;

namespace Sound
{
    public delegate void SoundRequestDelegate();

    public struct SoundRequest
    {
        public ulong id;
        public SoundDataSO soundData;
        public SoundVariation variation;
        public SoundEmitter soundEmitter;
        public AudioSourceConfigSO audioSourceConfig;
        public List<SoundControl> soundControls;
        public float fadeInDuration;
        public AudioMixerGroup mixerGroup;
        public SoundRequestDelegate onStop;
    }
}

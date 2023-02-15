using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Sound
{
    public delegate ulong PlaySoundDelegate(SoundRequest soundRequestEvent);
    public delegate void StopSoundDelegate(ulong soundInstanceId, float fadeDuration);

    [CreateAssetMenu(fileName = "New SoundMixer", menuName = "Sound/Sound Mixer")]
    public class SoundMixerSO : ScriptableObject
    {
        public AudioMixer mixer;
        public List<SoundMixerTrackSO> tracks;

        public PlaySoundDelegate onPlaySound;
        public StopSoundDelegate onStopSound;

        private void OnValidate()
        {
            foreach (SoundMixerTrackSO track in tracks)
            {
                track.mixer = this;
            }
        }

        public ulong PlaySound(SoundRequest soundRequest)
        {
            Utils.AssertNotNull(onPlaySound, "No delegate available to sink SoundRequest from SoundMixerSO.");
            return onPlaySound(soundRequest);
        }

        public void StopSound(ulong soundInstanceId, float fadeDuration)
        {
            Utils.AssertNotNull(onStopSound, "No delegate available to sink SoundRequest from SoundMixerSO.");
            onStopSound(soundInstanceId, fadeDuration);
        }
    }
}

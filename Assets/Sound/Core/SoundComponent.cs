using System.Collections.Generic;
using UnityEngine;

namespace Sound
{
    public class SoundComponent : MonoBehaviour
    {
        public SoundMixerTrackSO mixerTrack;
        public SoundEmitter soundEmitter;
        public List<SoundDataSO> sounds = new List<SoundDataSO>();
        public List<SoundControl> soundControls = new List<SoundControl>();

        public ulong PlaySound(
            string soundName,
            SoundVariation variation = null,
            SoundEmitter soundEmitter = null,
            AudioSourceConfigSO audioSourceConfig = null)
        {
            if (soundEmitter == null)
            {
                soundEmitter = this.soundEmitter;

                if (soundEmitter == null)
                {
                    soundEmitter = GetComponent<SoundEmitter>();

                    if (soundEmitter == null)
                    {
                        Utils.HandleWarning($"Unable to locate SoundEmitter for {gameObject.name}.");
                        return SoundInstance.InvalidId;
                    }
                }
            }

            if (FindInComponentSounds(soundName, out SoundDataSO soundData))
            {
                return mixerTrack.PlaySound(soundData, variation, soundEmitter, audioSourceConfig, soundControls);
            }

            Utils.HandleWarning($"Unable to find {soundData.GetType().Name} with name {soundName} on {gameObject.name}.");

            return SoundInstance.InvalidId;
        }

        public void StopSound(ulong soundInstanceId, float fadeDuration = 0f)
        {
            mixerTrack.Stop(soundInstanceId, fadeDuration);
        }

        private bool FindInComponentSounds(string soundName, out SoundDataSO soundData)
        {
            foreach (SoundDataSO localSoundData in sounds)
            {
                if (localSoundData.name == soundName)
                {
                    soundData = localSoundData;
                    return true;
                }
            }

            soundData = null;
            return false;
        }

        private void Update()
        {
            if (soundControls.Count == 0)
            {
                return;
            }


        }
    }
}

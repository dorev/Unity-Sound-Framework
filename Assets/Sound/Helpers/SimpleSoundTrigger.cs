using UnityEngine;

namespace Sound
{
    public class SimpleSoundTrigger : MonoBehaviour
    {
        public SoundMixerTrackSO mixerTrack;
        public SoundDataSO soundData;
        public bool stopOnExit = false;
        private ulong _soundInstanceId;

        private void OnTriggerEnter(Collider other)
        {
            _soundInstanceId = mixerTrack.PlaySound(soundData);
        }

        private void OnTriggerExit(Collider other)
        {
            if (stopOnExit)
            {
                mixerTrack.Stop(_soundInstanceId);
            }
        }
    }
}

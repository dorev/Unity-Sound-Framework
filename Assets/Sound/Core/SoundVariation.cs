using UnityEngine;

namespace Sound
{
    [System.Serializable]
    public class SoundVariation
    {
        public static SoundVariation RandomSeek()
        {
            return new SoundVariation { randomSeek = true };
        }

        [Range(-24f, 24f)] public float dB = 0f;
        [Range(-3f, 3f)] public float pitch = 1f;
        [Range(0f, 1f)] public float seek = 0f;
        public bool randomSeek = false;

        public bool HasModification()
        {
            return dB != 0f
                || pitch != 1f
                || seek != 0f
                || randomSeek == true;
        }

        public void Apply(AudioSource audioSource)
        {
            Utils.AssertNotNull(audioSource, "Unable to apply SoundVariation to null AudioSource");

            if (dB != 0f)
            {
                Utils.AddDecibel(dB, audioSource);
            }

            if (pitch != 1f)
            {
                audioSource.pitch = pitch;
            }

            if (seek != 0f || randomSeek)
            {
                Utils.AssertNotNull(audioSource.clip, "Cannot seek on an AudioSource without AudioClip.");
                audioSource.time = (randomSeek ? Random.value : seek) * audioSource.clip.length;
            }
        }
    }
}

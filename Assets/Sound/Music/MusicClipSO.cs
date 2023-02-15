using System.Collections.Generic;
using UnityEngine;

namespace Sound
{
    public delegate void MusicClipDelegate();

    [CreateAssetMenu(fileName = "New Music Clip", menuName = "Sound/MusicClip")]
    public class MusicClipSO : ScriptableObject
    {
        public SoundDataSO soundData;
        public float bpm;
        public string description;
        public List<MusicMarker> markers;

        public MusicClipDelegate onPlay;
        public MusicClipDelegate onStop;

        void Seek(float time)
        {
        }

        float FadeOutOnNextMarker(float fadeDuration = 0.1f)
        {
            return 0f; // return time of next marker
        }

        float FadeOutOnMarker(MusicMarker marker, float fadeDuration = 0.1f)
        {
            return 0f; // return time of marker
        }

        void FadeInMarkerAtTime(MusicMarker marker, float time, float fadeDuration = 0.1f)
        {
        }

        void Play(SoundMixerTrackSO mixerTrack)
        {
            // no marker starts at beginning of clip
            // otherwise start at first marker
        }

        public bool GetSamples(ref float[] samples, ref int frames, ref int channels)
        {
            if(soundData != null && soundData.audioClip != null)
            {
                AudioClip clip = soundData.audioClip;
                
                if (samples == null || samples.Length < clip.samples)
                {
                    samples = new float[clip.samples * clip.channels];
                }

                clip.GetData(samples, 0);
                channels = clip.channels;
                frames = clip.samples;

                return true;
            }

            return false;
        }
    }
}

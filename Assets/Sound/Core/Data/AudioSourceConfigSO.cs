using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Sound
{
    [CreateAssetMenu(fileName = "New AudioSourceConfig", menuName = "Sound/AudioSource Config")]
    public class AudioSourceConfigSO : ScriptableObject
    {
        public AudioMixerGroup outputAudioMixerGroup = null;
        public bool mute = false;
        public bool bypassEffects = false;
        public bool bypassListenerEffects = false;
        public bool bypassReverbZones = false;
        public bool playOnAwake = true;
        public bool loop = false;
        [Range(0, 256)] public int priority = 128;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(-3f, 3f)] public float pitch = 1f;
        [Range(-1f, 1f)] public float panStereo = 0f;
        [Range(0f, 1f)] public float spatialBlend = 0f;
        [Range(0f, 1.1f)] public float reverbZoneMix = 1f;
        [Range(0f, 5f)] public float dopplerLevel = 1f;
        [Range(0, 360)] public int spread = 0;
        public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;
        public float minDistance = 1f;
        public float maxDistance = 500f;
        public bool spatialize = false;
        public GamepadSpeakerOutputType gamepadSpeakerOutputType = GamepadSpeakerOutputType.Speaker;
        public AnimationCurve customRolloffCurve = null;

        public void Apply(AudioSource audioSource)
        {
            Utils.AssertNotNull(audioSource, $"Cannot apply {GetType().Name} on null AudioSource.");

            audioSource.outputAudioMixerGroup = outputAudioMixerGroup;
            audioSource.mute = mute;
            audioSource.bypassEffects = bypassEffects;
            audioSource.bypassListenerEffects = bypassListenerEffects;
            audioSource.bypassReverbZones = bypassReverbZones;
            audioSource.playOnAwake = playOnAwake;
            audioSource.loop = loop;
            audioSource.priority = priority;
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.panStereo = panStereo;
            audioSource.spatialBlend = spatialBlend;
            audioSource.reverbZoneMix = reverbZoneMix;

            // 3D Sound Settings
            audioSource.dopplerLevel = dopplerLevel;
            audioSource.spread = spread;
            audioSource.rolloffMode = rolloffMode;
            audioSource.minDistance = minDistance;
            audioSource.maxDistance = maxDistance;

            // Hidden in Inspector
            audioSource.spatialize = spatialize;
            audioSource.gamepadSpeakerOutputType = gamepadSpeakerOutputType;

            if (customRolloffCurve != null && customRolloffCurve.length > 1)
            {
                audioSource.rolloffMode = AudioRolloffMode.Custom;
                audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, customRolloffCurve);
            }
        }

        public static void Reset(AudioSource audioSource)
        {
            audioSource.clip = null;
            audioSource.outputAudioMixerGroup = null;
            audioSource.mute = false;
            audioSource.bypassEffects = false;
            audioSource.bypassListenerEffects = false;
            audioSource.bypassReverbZones = false;
            audioSource.playOnAwake = true;
            audioSource.loop = false;
            audioSource.priority = 128;
            audioSource.volume = 1f;
            audioSource.pitch = 1f;
            audioSource.panStereo = 0f;
            audioSource.spatialBlend = 0f;
            audioSource.reverbZoneMix = 1f;

            // 3D Sound Settings
            audioSource.dopplerLevel = 1f;
            audioSource.spread = 0f;
            audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
            audioSource.minDistance = 1f;
            audioSource.maxDistance = 500f;

            // Hidden in Inspector
            audioSource.spatialize = false;
            audioSource.gamepadSpeakerOutputType = GamepadSpeakerOutputType.Speaker;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Sound
{
    public static class Effects
    {
        public static class Chorus
        {
            [Range(0f, 1f)] public const float dryMix = 0.5f;
            [Range(0f, 1f)] public const float wetMix1 = 0.5f;
            [Range(0f, 1f)] public const float wetMix2 = 0.5f;
            [Range(0f, 1f)] public const float wetMix3 = 0.5f;
            [Range(0.1f, 100f)] public const float delay = 40f;
            [Range(0f, 20f)] public const float rate = 0.8f;
            [Range(0f, 1f)] public const float depth = 0.03f;
        }

        public static class Distortion
        {
            [Range(0f, 1f)] public const float distortionLevel = 0.5f;
        }


        public static class HighPass
        {
            [Range(10f, 22000f)] public const float cutoffFrequency = 5000f;
            [Range(1f, 10f)] public const float resonance = 1f;
        }


        public static class LowPass
        {
            [Range(10f, 22000f)] public const float cutoffFrequency = 5000f;
            [Range(1f, 10f)] public const float resonance = 1f;
        }

        public static class Reverb
        {
            public const AudioReverbPreset preset = AudioReverbPreset.Generic;
            [Range(-10000, 0)] public const int dryLevel = 0;
            [Range(-10000, 0)] public const int room = -1000;
            [Range(-10000, 0)] public const int roomHF = -100;
            [Range(-10000, 0)] public const int roomLF = 0;
            [Range(0.1f, 20f)] public const float decayTime = 1.5f;
            [Range(0.1f, 2f)] public const float decayHFRatio = 0.85f;
            [Range(-10000, 1000)] public const int reflectionsLevel = -2600;
            [Range(0f, 0.3f)] public const float reflectionsDelay = 0f;
            [Range(-10000, 2000)] public const float reverbLevel = 200;
            [Range(0f, 0.1f)] public const float reverbDelay = 0.01f;
            [Range(1000, 20000)] public const int hfReference = 5000;
            [Range(20, 1000)] public const int lfReference = 250;
            [Range(0, 100)] public const int diffusion = 100;
            [Range(0, 100)] public const int density = 100;
        }

        public static class Echo
        {
            [Range(10, 5000)] public const int delay = 500;
            [Range(0f, 1f)] public const float decayRatio = 0.5f;
            [Range(0f, 1f)] public const float dryMix = 1f;
            [Range(0f, 1f)] public const float wetMix = 1f;
        }
    }
}

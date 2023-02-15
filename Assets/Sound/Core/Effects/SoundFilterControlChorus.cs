using System.Collections.Generic;
using UnityEngine;

namespace Sound
{
    [CreateAssetMenu(fileName = "New FilterConfigChorus", menuName = "Sound/Config/Filter Chorus Config")]
    public class FilterConfigChorusSO : ScriptableObject
    {
        [Range(0f, 1f)] public float dryMix = Effects.Chorus.dryMix;
        [Range(0f, 1f)] public float wetMix1 = Effects.Chorus.wetMix1;
        [Range(0f, 1f)] public float wetMix2 = Effects.Chorus.wetMix2;
        [Range(0f, 1f)] public float wetMix3 = Effects.Chorus.wetMix3;
        [Range(0.1f, 100f)] public float delay = Effects.Chorus.delay;
        [Range(0f, 20f)] public float rate = Effects.Chorus.rate;
        [Range(0f, 1f)] public float depth = Effects.Chorus.depth;

        public void Apply(GameObject gameObject)
        {
            AudioChorusFilter chorusFilter = Utils.GetOrCreateComponent<AudioChorusFilter>(gameObject);
            chorusFilter.dryMix = dryMix;
            chorusFilter.wetMix1 = wetMix1;
            chorusFilter.wetMix2 = wetMix2;
            chorusFilter.wetMix3 = wetMix3;
            chorusFilter.delay = delay;
            chorusFilter.rate = rate;
            chorusFilter.depth = depth;
        }
    }

    public enum ChorusParameter
    {
        DryMix,
        WetMix1,
        WetMix2,
        WetMix3,
        Delay,
        Rate,
        Depth,
    }

    [System.Serializable]
    public class SoundFilterControlChorus : ISoundFilterControl
    {
        public string name { get; private set; } = FilterType.Chorus.ToString();
        public FilterType filterType { get; } = FilterType.Chorus;

        [SerializeField]
        public Dictionary<string, SoundParameter> parameters { get; private set; } = new Dictionary<string, SoundParameter>
        {
            { "dryMix", new SoundParameter("dryMix", 0f, 1f, (int) ChorusParameter.DryMix) },
            { "wetMix1", new SoundParameter("wetMix1", 0f, 1f, (int) ChorusParameter.WetMix1) },
            { "wetMix2", new SoundParameter("wetMix2", 0f, 1f, (int) ChorusParameter.WetMix2) },
            { "wetMix3", new SoundParameter("wetMix3", 0f, 1f, (int) ChorusParameter.WetMix3) },
            { "delay", new SoundParameter("delay", 0.1f, 100f, (int) ChorusParameter.Delay) },
            { "rate", new SoundParameter("rate", 0f, 20f, (int) ChorusParameter.Rate) },
            { "depth", new SoundParameter("depth", 0f, 1f, (int) ChorusParameter.Depth) },
        };

        public AudioChorusFilter chorusFilter;

        public bool TryUpdateParameter(string parameterName, float value, SoundInstance soundInstance)
        {
            if(parameters.TryGetValue(parameterName, out SoundParameter parameter))
            {
                if (parameter.UpdateValue(value))
                {
                    Utils.AssertNotNull(soundInstance, $"{GetType().Name} was provided a null SoundInstance to update.");
                    if (soundInstance != null && soundInstance.audioSource != null)
                    {
                        GameObject gameObject = soundInstance.audioSource.gameObject;
                        UpdateFilter(gameObject, parameter);
                    }
                }
                return true;
            }

            return false;
        }

        private void UpdateFilter(GameObject gameObject, SoundParameter parameter)
        {
            chorusFilter = Utils.GetOrCreateComponent<AudioChorusFilter>(gameObject);

            switch ((ChorusParameter) parameter.userTag)
            {
                case ChorusParameter.DryMix:
                    chorusFilter.dryMix = parameters["dryMix"].FetchValue();
                    break;
                case ChorusParameter.WetMix1:
                    chorusFilter.wetMix1 = parameters["wetMix1"].FetchValue();
                    break;
                case ChorusParameter.WetMix2:
                    chorusFilter.wetMix2 = parameters["wetMix2"].FetchValue();
                    break;
                case ChorusParameter.WetMix3:
                    chorusFilter.wetMix3 = parameters["wetMix3"].FetchValue();
                    break;
                case ChorusParameter.Delay:
                    chorusFilter.delay = parameters["delay"].FetchValue();
                    break;
                case ChorusParameter.Rate:
                    chorusFilter.rate = parameters["rate"].FetchValue();
                    break;
                case ChorusParameter.Depth:
                    chorusFilter.depth = parameters["depth"].FetchValue();
                    break;
                default:
                    break;
            }
        }
    }
}

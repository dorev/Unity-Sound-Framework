using System.Collections.Generic;
using UnityEngine;

namespace Sound
{
    [CreateAssetMenu(fileName = "New FilterConfigEcho", menuName = "Sound/Config/Filter Echo Config")]
    public class FilterConfigEchoSO : ScriptableObject
    {
        [Range(0.1f, 500f)] public float delay = Effects.Echo.delay;
        [Range(0f, 10f)] public float decayRatio = Effects.Echo.decayRatio;
        [Range(0f, 1f)] public float dryMix = Effects.Echo.dryMix;
        [Range(0f, 1f)] public float wetMix = Effects.Echo.wetMix;

        public void Apply(GameObject gameObject)
        {
            AudioEchoFilter echoFilter = Utils.GetOrCreateComponent<AudioEchoFilter>(gameObject);
            echoFilter.delay = delay;
            echoFilter.decayRatio = decayRatio;
            echoFilter.dryMix = dryMix;
            echoFilter.wetMix = wetMix;
          }
    }

    public enum EchoParameter
    {
        Delay,
        DecayRatio,
        DryMix,
        WetMix,
    }

    [System.Serializable]
    public class SoundFilterControlEcho : ISoundFilterControl
    {
        public string name { get; private set; } = FilterType.Echo.ToString();
        public FilterType filterType { get; } = FilterType.Echo;

        [SerializeField]
        public Dictionary<string, SoundParameter> parameters { get; private set; } = new Dictionary<string, SoundParameter>
        {
            { "delay", new SoundParameter("delay", 0.1f, 500f, (int) EchoParameter.Delay) },
            { "decayRatio", new SoundParameter("decayRatio", 0f, 1f, (int) EchoParameter.DecayRatio) },
            { "dryMix", new SoundParameter("dryMix", 0f, 1f, (int) EchoParameter.DryMix) },
            { "wetMix1", new SoundParameter("wetMix", 0f, 1f, (int) EchoParameter.WetMix) },
        };

        public AudioEchoFilter echoFilter;

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
            echoFilter = Utils.GetOrCreateComponent<AudioEchoFilter>(gameObject);

            switch ((EchoParameter) parameter.userTag)
            {
                case EchoParameter.Delay:
                    echoFilter.delay = parameters["delay"].FetchValue();
                    break;
                case EchoParameter.DecayRatio:
                    echoFilter.decayRatio = parameters["decayRatio"].FetchValue();
                    break;
                case EchoParameter.DryMix:
                    echoFilter.dryMix = parameters["dryMix"].FetchValue();
                    break;
                case EchoParameter.WetMix:
                    echoFilter.wetMix = parameters["wetMix"].FetchValue();
                    break;
                default:
                    break;
            }
        }
    }
}

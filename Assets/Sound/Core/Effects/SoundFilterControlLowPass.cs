using System.Collections.Generic;
using UnityEngine;

namespace Sound
{
    [CreateAssetMenu(fileName = "New FilterConfigLowPass", menuName = "Sound/Config/Filter LowPass Config")]
    public class FilterConfigLowPassSO : ScriptableObject
    {
        [Range(10f, 22000f)] public const float cutoffFrequency = Effects.LowPass.cutoffFrequency;
        [Range(1f, 10f)] public const float resonance = Effects.LowPass.resonance;

        public void Apply(GameObject gameObject)
        {
            AudioLowPassFilter lowPassFilter = Utils.GetOrCreateComponent<AudioLowPassFilter>(gameObject);
            lowPassFilter.cutoffFrequency = cutoffFrequency;
            lowPassFilter.lowpassResonanceQ = resonance;
        }
    }

    public enum LowPassParameter
    {
        CutoffFrequency,
        Resonance,
    }

    [System.Serializable]
    public class SoundFilterControlLowPass : ISoundFilterControl
    {
        public string name { get; private set; } = FilterType.LowPass.ToString();
        public FilterType filterType { get; } = FilterType.LowPass;

        [SerializeField]
        public Dictionary<string, SoundParameter> parameters { get; private set; } = new Dictionary<string, SoundParameter>
        {
            { "cutoffFrequency", new SoundParameter("cutoffFrequency", 10f, 22000f, (int) LowPassParameter.CutoffFrequency) },
            { "resonance", new SoundParameter("resonance", 1f, 10f, (int) LowPassParameter.Resonance) },
        };

        public AudioLowPassFilter lowPassFilter;

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
            lowPassFilter = Utils.GetOrCreateComponent<AudioLowPassFilter>(gameObject);

            switch ((LowPassParameter) parameter.userTag)
            {
                case LowPassParameter.CutoffFrequency:
                    lowPassFilter.cutoffFrequency = parameters["cutoffFrequency"].FetchValue();
                    break;
                case LowPassParameter.Resonance:
                    lowPassFilter.lowpassResonanceQ = parameters["resonance"].FetchValue();
                    break;
                default:
                    break;
            }
        }
    }
}

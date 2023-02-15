using System.Collections.Generic;
using UnityEngine;

namespace Sound
{
    [CreateAssetMenu(fileName = "New FilterConfigHighPass", menuName = "Sound/Config/Filter HighPass Config")]
    public class FilterConfigHighPassSO : ScriptableObject
    {
        [Range(10f, 22000f)] public const float cutoffFrequency = Effects.HighPass.cutoffFrequency;
        [Range(1f, 10f)] public const float resonance = Effects.HighPass.resonance;

        public void Apply(GameObject gameObject)
        {
            AudioHighPassFilter highPassFilter = Utils.GetOrCreateComponent<AudioHighPassFilter>(gameObject);
            highPassFilter.cutoffFrequency = cutoffFrequency;
            highPassFilter.highpassResonanceQ = resonance;
        }
    }

    public enum HighPassParameter
    {
        CutoffFrequency,
        Resonance,
    }

    [System.Serializable]
    public class SoundFilterControlHighPass : ISoundFilterControl
    {
        public string name { get; private set; } = FilterType.HighPass.ToString();
        public FilterType filterType { get; } = FilterType.HighPass;

        [SerializeField]
        public Dictionary<string, SoundParameter> parameters { get; private set; } = new Dictionary<string, SoundParameter>
        {
            { "cutoffFrequency", new SoundParameter("cutoffFrequency", 10f, 22000f, (int) HighPassParameter.CutoffFrequency) },
            { "resonance", new SoundParameter("resonance", 1f, 10f, (int) HighPassParameter.Resonance) },
        };

        public AudioHighPassFilter highPassFilter;

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
            highPassFilter = Utils.GetOrCreateComponent<AudioHighPassFilter>(gameObject);

            switch ((HighPassParameter) parameter.userTag)
            {
                case HighPassParameter.CutoffFrequency:
                    highPassFilter.cutoffFrequency = parameters["cutoffFrequency"].FetchValue();
                    break;
                case HighPassParameter.Resonance:
                    highPassFilter.highpassResonanceQ = parameters["resonance"].FetchValue();
                    break;
                default:
                    break;
            }
        }
    }
}

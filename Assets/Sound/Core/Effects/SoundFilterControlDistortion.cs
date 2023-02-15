using System.Collections.Generic;
using UnityEngine;

namespace Sound
{
    [CreateAssetMenu(fileName = "New FilterConfigDistortion", menuName = "Sound/Config/Filter Distortion Config")]
    public class FilterConfigDistortionSO : ScriptableObject
    {
        [Range(0f, 1f)] public const float distortionLevel = Effects.Distortion.distortionLevel;

        public void Apply(GameObject gameObject)
        {
            AudioDistortionFilter distortionFilter = Utils.GetOrCreateComponent<AudioDistortionFilter>(gameObject);
            distortionFilter.distortionLevel = distortionLevel;
        }
    }

    public enum DistortionParameter
    {
        Level,
    }

    [System.Serializable]
    public class SoundFilterControlDistortion : ISoundFilterControl
    {
        public string name { get; private set; } = FilterType.Distortion.ToString();
        public FilterType filterType { get; } = FilterType.Distortion;

        [SerializeField]
        public Dictionary<string, SoundParameter> parameters { get; private set; } = new Dictionary<string, SoundParameter>
        {
            { "distortionLevel", new SoundParameter("distortionLevel", 0f, 1f, (int) DistortionParameter.Level) },
        };

        public AudioDistortionFilter distortionFilter;

        public bool TryUpdateParameter(string parameterName, float value, SoundInstance soundInstance)
        {
            if (parameters.TryGetValue(parameterName, out SoundParameter parameter))
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
            distortionFilter = Utils.GetOrCreateComponent<AudioDistortionFilter>(gameObject);

            switch ((DistortionParameter)parameter.userTag)
            {
                case DistortionParameter.Level:
                    distortionFilter.distortionLevel = parameters["distortionLevel"].FetchValue();
                    break;
                default:
                    break;
            }
        }
    }
}

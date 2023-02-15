using System.Collections.Generic;
using UnityEngine;

namespace Sound
{
    [CreateAssetMenu(fileName = "New FilterConfigReverb", menuName = "Sound/Config/Filter Reverb Config")]
    public class FilterConfigReverbSO : ScriptableObject
    {
        [Range(-10000, 0)] public int dryLevel = Effects.Reverb.dryLevel;
        [Range(-10000, 0)] public int room = Effects.Reverb.room;
        [Range(-10000, 0)] public int roomHF = Effects.Reverb.roomHF;
        [Range(-10000, 0)] public int roomLF = Effects.Reverb.roomLF;
        [Range(0.1f, 20f)] public float decayTime = Effects.Reverb.decayTime;
        [Range(0.1f, 2f)] public float decayHFRatio = Effects.Reverb.decayHFRatio;
        [Range(-10000, 1000)] public int reflectionsLevel = Effects.Reverb.reflectionsLevel;
        [Range(0f, 0.3f)] public float reflectionsDelay = Effects.Reverb.reflectionsDelay;
        [Range(-10000, 2000)] public float reverbLevel = Effects.Reverb.reverbLevel;
        [Range(0f, 0.1f)] public float reverbDelay = Effects.Reverb.reverbDelay;
        [Range(1000, 20000)] public int hfReference = Effects.Reverb.hfReference;
        [Range(20, 1000)] public  int lfReference = Effects.Reverb.lfReference;
        [Range(0, 100)] public int diffusion = Effects.Reverb.diffusion;
        [Range(0, 100)] public  int density = Effects.Reverb.density;

        public void Apply(GameObject gameObject)
        {
            AudioReverbFilter reverbFilter = Utils.GetOrCreateComponent<AudioReverbFilter>(gameObject);
            reverbFilter.dryLevel = dryLevel;
            reverbFilter.room = room;
            reverbFilter.roomHF = roomHF;
            reverbFilter.roomLF = roomLF;
            reverbFilter.decayTime = decayTime;
            reverbFilter.decayHFRatio = decayHFRatio;
            reverbFilter.reflectionsLevel = reflectionsLevel;
            reverbFilter.reflectionsDelay = reflectionsDelay;
            reverbFilter.reverbLevel = reverbLevel;
            reverbFilter.reverbDelay = reverbDelay;
            reverbFilter.hfReference = hfReference;
            reverbFilter.lfReference = lfReference;
            reverbFilter.diffusion = diffusion;
            reverbFilter.density = density;
        }
    }

    public enum ReverbParameter
    {
        DryLevel,
        Room,
        RoomHF,
        RoomLF,
        DecayTime,
        DecayHFRatio,
        ReflectionsLevel,
        ReflectionsDelay,
        ReverbLevel,
        ReverbDelay,
        HFReference,
        LFReference,
        Diffusion,
        Density,
    }

    [System.Serializable]
    public class SoundFilterControlReverb : ISoundFilterControl
    {
        public string name { get; private set; } = FilterType.Reverb.ToString();
        public FilterType filterType { get; } = FilterType.Reverb;

        [SerializeField]
        public Dictionary<string, SoundParameter> parameters { get; private set; } = new Dictionary<string, SoundParameter>
        {
            { "dryLevel", new SoundParameter("dryLevel", -10000f, 0f, (int) ReverbParameter.DryLevel) },
            { "room", new SoundParameter("room", -10000f, 0f, (int) ReverbParameter.Room) },
            { "roomHF", new SoundParameter("roomHF", -10000f, 0f, (int) ReverbParameter.RoomHF) },
            { "roomLF", new SoundParameter("roomLF", -10000f, 0f, (int) ReverbParameter.RoomLF) },
            { "decayTime", new SoundParameter("decayTime", 0.1f, 20f, (int) ReverbParameter.DecayTime) },
            { "decayHFRatio", new SoundParameter("decayHFRatio", 0.1f, 2f, (int) ReverbParameter.DecayHFRatio) },
            { "reflectionsLevel", new SoundParameter("reflectionsLevel", -10000f, 1000f, (int) ReverbParameter.ReflectionsLevel) },
            { "reflectionsDelay", new SoundParameter("reflectionsDelay", 0f, 0.3f, (int) ReverbParameter.ReflectionsDelay) },
            { "reverbLevel", new SoundParameter("reverbLevel", -10000f, 2000f, (int) ReverbParameter.ReverbLevel) },
            { "reverbDelay", new SoundParameter("reverbDelay", 0f, 0.1f, (int) ReverbParameter.ReverbDelay) },
            { "hfReference", new SoundParameter("hfReference",1000f, 20000f, (int) ReverbParameter.HFReference) },
            { "lfReference", new SoundParameter("lfReference", 20f, 1000f, (int) ReverbParameter.LFReference) },
            { "diffusion", new SoundParameter("diffusion", 0f, 100f, (int) ReverbParameter.Diffusion) },
            { "density", new SoundParameter("density", 0f, 100f, (int) ReverbParameter.Density) },
        };

        public AudioReverbFilter reverbFilter;

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
            reverbFilter = Utils.GetOrCreateComponent<AudioReverbFilter>(gameObject);

            switch ((ReverbParameter) parameter.userTag)
            {
                case ReverbParameter.DryLevel:
                    reverbFilter.dryLevel = parameters["dryLevel"].FetchValue();
                    break;
                case ReverbParameter.Room:
                    reverbFilter.room = parameters["room"].FetchValue();
                    break;
                case ReverbParameter.RoomHF:
                    reverbFilter.roomHF = parameters["roomHF"].FetchValue();
                    break;
                case ReverbParameter.RoomLF:
                    reverbFilter.roomLF = parameters["roomLF"].FetchValue();
                    break;
                case ReverbParameter.DecayTime:
                    reverbFilter.decayTime = parameters["decayTime"].FetchValue();
                    break;
                case ReverbParameter.DecayHFRatio:
                    reverbFilter.decayHFRatio = parameters["decayHFRatio"].FetchValue();
                    break;
                case ReverbParameter.ReflectionsLevel:
                    reverbFilter.reflectionsLevel = parameters["reflectionsLevel"].FetchValue();
                    break;
                case ReverbParameter.ReflectionsDelay:
                    reverbFilter.reflectionsDelay = parameters["reflectionsDelay"].FetchValue();
                    break;
                case ReverbParameter.ReverbLevel:
                    reverbFilter.reverbLevel = parameters["reverbLevel"].FetchValue();
                    break;
                case ReverbParameter.ReverbDelay:
                    reverbFilter.reverbDelay = parameters["reverbDelay"].FetchValue();
                    break;
                case ReverbParameter.HFReference:
                    reverbFilter.hfReference = parameters["hfReference"].FetchValue();
                    break;
                case ReverbParameter.LFReference:
                    reverbFilter.lfReference = parameters["lfReference"].FetchValue();
                    break;
                case ReverbParameter.Diffusion:
                    reverbFilter.diffusion = parameters["diffusion"].FetchValue();
                    break;
                case ReverbParameter.Density:
                    reverbFilter.density = parameters["density"].FetchValue();
                    break;
                default:
                    break;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sound
{
    public enum FilterType
    {
        Chorus,
        Distortion,
        Echo,
        HighPass,
        LowPass,
        Reverb,
    }

    public interface ISoundFilterControl
    {
        string name { get; }
        FilterType filterType { get; }
        Dictionary<string, SoundParameter> parameters { get; }
        bool TryUpdateParameter(string parameterName, float value, SoundInstance soundInstance);
    }
}

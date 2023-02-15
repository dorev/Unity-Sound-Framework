using System.Collections.Generic;
using UnityEngine;

namespace Sound
{
    [System.Serializable]
    public struct MusicMarker
    {
        float time;
        int sample;
        string name;
        int userTag;
    }
}

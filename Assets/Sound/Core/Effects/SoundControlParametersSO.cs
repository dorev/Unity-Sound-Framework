using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Sound
{
    [CreateAssetMenu(fileName = "New SoundControlParameters", menuName = "Sound/Sound Control Parameters")]
    public class SoundControlParametersSO : ScriptableObject
    {
        public List<SoundParameter> parameters;

        public List<SoundParameter> GetParameters()
        {
            return parameters;
        }

        public bool TryGetParameter(string parameterName, out SoundParameter parameter)
        {
            parameter = parameters.Find(param => param.name == parameterName);
            return parameter != null;
        }


    }
}

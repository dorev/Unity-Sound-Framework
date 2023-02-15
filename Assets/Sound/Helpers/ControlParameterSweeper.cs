using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sound
{
    public class ControlParameterSweeper : MonoBehaviour
    {
        public SoundControlParametersSO parameters;
        public string parameterName;

        // Update is called once per frame
        void Update()
        {
            if(parameters.TryGetParameter(parameterName, out SoundParameter parameter))
            {
                parameter.UpdateValue(parameter.minLimit + (Mathf.Sin(Time.unscaledTime) + 1) / 2 * (parameter.maxLimit - parameter.minLimit));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Sound
{
    [System.Serializable]
    public class SoundParameter
    {
        public string name;
        public float minLimit = float.MinValue;
        public float maxLimit = float.MaxValue;
        public int userTag = 0;
        public bool HasChanged => _hasChanged;

        [SerializeField] private float _value = 0f;
        [SerializeField] private bool _hasChanged = false;

        public SoundParameter(string name, float rangeMin = float.MinValue, float rangeMax = float.MaxValue, int userTag = 0)
        {
            if (rangeMin != rangeMax && (rangeMin != float.MinValue || rangeMax != float.MaxValue))
            {
                minLimit = rangeMin;
                maxLimit = rangeMax;
            }

            this.name = name;
            this.userTag = userTag;
            this._value = minLimit;
        }

        public bool UpdateValue(float newValue)
        {
            float clampedNewValue = Mathf.Clamp(newValue, minLimit, maxLimit);
            if (_value != clampedNewValue)
            {
                _hasChanged = true;
                _value = clampedNewValue;
                return true;
            }

            return false;
        }

        public float FetchValue()
        {
            _hasChanged = false;
            return _value;
        }

        public float ReadValue()
        {
            return _value;
        }
    }
}

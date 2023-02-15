using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

namespace Sound
{
    public enum SoundControlMode
    {
        Distance,
        ControlParameter,
    }

    public enum CurveMode
    {
        Linear,
        Logarithmic,
        InverseLogarithmic,
        Exponential,
        InverseExponential,
        Custom,
    }


    [System.Serializable]
    public class SoundControl : ISoundEffect
    {
        // ISoundEffect
        public int id  => (typeof(ISoundFilterControl).GetHashCode() << 16) | ((int) filterType << 8) | filterParameterIndex;
        public string name => filterType.ToString();
        public SoundEffectState state => SoundEffectState.Processing;
        public SoundEffectDelegate onFailed { get; set; }
        public SoundEffectDelegate onFinished { get; set; }

        // SoundControl
        public FilterType filterType;
        public SoundControlMode controlMode = SoundControlMode.Distance;
        public CurveMode curveMode = CurveMode.Logarithmic;

        public GameObject distanceFrom = null;
        public float distanceMin;
        public float distanceMax;

        public SoundControlParametersSO controlParameters;
        public int controlParameterIndex;
        public SoundParameter filterParameter;
        public int filterParameterIndex;
        public float filterRangeMin;
        public float filterRangeMax;
        public float controlRangeMin;
        public float controlRangeMax;
        public AnimationCurve customCurve; // TODO

        [SerializeReference] private ISoundFilterControl _soundEffect;

        public void Update(SoundInstance soundInstance)
        {
            float value = 0;
            float projectionRatio = 0;

            switch (controlMode)
            {
                case SoundControlMode.ControlParameter:
                    SoundParameter controlParameter = controlParameters.parameters[controlParameterIndex];
                    if (controlParameters.TryGetParameter(controlParameter.name, out SoundParameter parameter))
                    {
                        value = parameter.FetchValue();
                        projectionRatio = CalculateProjectionRatio(value, controlRangeMin, controlRangeMax);
                    }
                    else
                    {
                        Utils.HandleError($"SoundControl {name} unable to update based on control parameter {controlParameter.name}");
                    }
                    break;

                case SoundControlMode.Distance:
                    Utils.AssertNotNull(distanceFrom, $"SoundControl {name} unable to be updated based on distance because no reference GameObject was provided");
                    value = Vector3.Distance(distanceFrom.transform.position, soundInstance.soundEmitter.transform.position);
                    projectionRatio = CalculateProjectionRatio(value, distanceMin, distanceMax);
                    break;

                default:
                    Utils.HandleError($"SoundControl {name} control mode is invalid");
                    break;
            }

            float projectedValue = CalculateFilterProjectedValue(projectionRatio, filterRangeMin, filterRangeMax);
            _soundEffect.TryUpdateParameter(filterParameter.name, projectedValue, soundInstance);
        }

        private float CalculateProjectionRatio(float value, float min, float max)
        {
            float controlProjectionRatio;
            float clampedValue;

            if (max >= min)
            {
                clampedValue = Mathf.Clamp(value, min, max);
                controlProjectionRatio = (clampedValue - min) / (max - min);
            }
            else
            {
                clampedValue = Mathf.Clamp(value, max, min);
                controlProjectionRatio = (min - clampedValue) / (min - max);
            }

            return controlProjectionRatio;
        }

        private float CalculateFilterProjectedValue(float projectionValue, float parameterMin, float parameterMax)
        {
            Utils.Assert(projectionValue >= 0 && projectionValue <= 1, $"SoundControl distance parameter projection factor is invalid: {projectionValue}");
            float projectionRange = parameterMax - parameterMin;
            float projectionFactor;
            
            switch(curveMode)
            {
                default:
                case CurveMode.Linear:
                    projectionFactor = projectionValue;
                    break;
                case CurveMode.Exponential:
                case CurveMode.InverseExponential:
                    projectionFactor = Mathf.Pow(projectionValue, 5);
                    break;
                case CurveMode.Logarithmic:
                case CurveMode.InverseLogarithmic:
                    projectionFactor = 1 - Mathf.Pow((float)System.Math.E, -20 * projectionValue);
                    break;
            }

            if(curveMode == CurveMode.InverseLogarithmic || curveMode == CurveMode.InverseExponential)
            {
                projectionFactor = 1 - projectionFactor;
            }

            return parameterMin + projectionRange * projectionFactor;
        }

        public bool Replace(ISoundEffect effect)
        {
            return false;
        }
    }
}

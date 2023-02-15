using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sound
{
    [CustomPropertyDrawer(typeof(SoundControl), true)]
    class SoundControlDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float margin = 0;
            int lines = 7;

            SerializedProperty controlMode = property.FindPropertyRelative("controlMode");
            if (controlMode.enumValueIndex == (int) SoundControlMode.Distance)
            {
                lines += 4;
            }
            else
            {
                lines += 6;
            }

            SerializedProperty customCurveMode = property.FindPropertyRelative("curveMode");
            if (customCurveMode.enumValueIndex == (int) CurveMode.Custom)
            {
                lines += 1;
            }

            return EditorUtils.LinesHeight(lines) + margin;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            Rect labelRect = new Rect(position.x, position.y, position.width, EditorUtils.LineHeight);
            EditorGUI.LabelField(labelRect, $"Sound Control {label.text[label.text.Length - 1]}", EditorStyles.boldLabel);

            ISoundFilterControl soundFilterControl = UpdateSoundFilter(ref position, property);

            Rect filterParameterRect = EditorGUI.PrefixLabel(EditorUtils.NextLineRect(ref position), new GUIContent("Filter Parameter"));
            string[] filterParameterStrings = soundFilterControl.parameters.Keys.ToArray();

            SerializedProperty filterParameterIndexProp = property.FindPropertyRelative("filterParameterIndex");
            filterParameterIndexProp.intValue = EditorGUI.Popup(filterParameterRect, filterParameterIndexProp.intValue, filterParameterStrings);
            SoundParameter filterParameter = soundFilterControl.parameters[filterParameterStrings[filterParameterIndexProp.intValue]];

            SerializedProperty filterRangeMinValue = property.FindPropertyRelative("filterRangeMin");
            SerializedProperty filterRangeMaxValue = property.FindPropertyRelative("filterRangeMax");

            float tmpRangeMin = Mathf.Clamp(filterRangeMinValue.floatValue, filterParameter.minLimit, filterParameter.maxLimit);
            float tmpRangeMax = Mathf.Clamp(filterRangeMaxValue.floatValue, filterParameter.minLimit, filterParameter.maxLimit);

            EditorGUI.MinMaxSlider(EditorUtils.NextLineRect(ref position), new GUIContent("Filter Parameter Range"),
                ref tmpRangeMin,
                ref tmpRangeMax,
                filterParameter.minLimit,
                filterParameter.maxLimit);

            (Rect rangeMinRect, Rect rangeMaxRect) = CalculateDualPrefixedRectangles(ref position);
            filterRangeMinValue.floatValue = EditorGUI.FloatField(rangeMinRect, tmpRangeMin);
            filterRangeMaxValue.floatValue = EditorGUI.FloatField(rangeMaxRect, tmpRangeMax);

            SerializedProperty filterParameterProperty = property.FindPropertyRelative("filterParameter");
            if (filterParameterProperty != null)
            {
                filterParameterProperty.FindPropertyRelative("minLimit").floatValue = filterParameter.minLimit;
                filterParameterProperty.FindPropertyRelative("maxLimit").floatValue = filterParameter.maxLimit;
                filterParameterProperty.FindPropertyRelative("name").stringValue = filterParameter.name;
                filterParameterProperty.FindPropertyRelative("userTag").intValue = filterParameter.userTag;
            }

            EditorUtils.NextLineRect(ref position);
            SerializedProperty curveMode = EditorUtils.AppendProperty(ref position, property, "curveMode");
            if (curveMode.enumValueIndex == (int) CurveMode.Custom)
            {
                EditorUtils.AppendProperty(ref position, property, "customCurve");
            }

            DrawModeSpecificProperties(ref position, property);

            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
            }
        }

        private (Rect, Rect) CalculateDualPrefixedRectangles(ref Rect position)
        {
            Rect firstRect = EditorGUI.PrefixLabel(EditorUtils.NextLineRect(ref position), new GUIContent(" "));
            Rect secondRect = firstRect;
            firstRect.width = firstRect.width / 2 - 4;
            secondRect.width /= 2;
            secondRect.x += secondRect.width;

            return (firstRect, secondRect);
        }

        private void DrawModeSpecificProperties(ref Rect position, SerializedProperty property)
        {
            SerializedProperty controlMode = EditorUtils.AppendProperty(ref position, property, "controlMode");
            EditorUtils.NextLineRect(ref position);

            if (controlMode.enumValueIndex == (int) SoundControlMode.Distance)
            {
                EditorUtils.AppendProperty(ref position, property, "distanceFrom");

                SerializedProperty minDistanceProp = property.FindPropertyRelative("distanceMin");
                SerializedProperty maxDistanceProp = property.FindPropertyRelative("distanceMax");
                (Rect minDistanceRect, Rect maxDistanceRect) = CalculateDualPrefixedRectangles(ref position);
                minDistanceProp.floatValue = EditorGUI.FloatField(minDistanceRect, minDistanceProp.floatValue);
                maxDistanceProp.floatValue = EditorGUI.FloatField(maxDistanceRect, maxDistanceProp.floatValue);
            }
            else
            {
                SerializedProperty controlParameters = EditorUtils.AppendProperty(ref position, property, "controlParameters");
                SoundControlParametersSO so = (Sound.SoundControlParametersSO)(controlParameters.objectReferenceValue);

                if (so != null)
                {
                    List<SoundParameter> controlParametersList = so.GetParameters();
                    string[] parameterStrings = controlParametersList.Aggregate(new List<string>(),
                        (newList, item) => { newList.Add(item.name); return newList; }).ToArray();

                    Rect controlParameterListRect = EditorGUI.PrefixLabel(EditorUtils.NextLineRect(ref position), new GUIContent("Control Parameter"));
                    SerializedProperty controlParameterIndexProp = property.FindPropertyRelative("controlParameterIndex");
                    controlParameterIndexProp.intValue = EditorGUI.Popup(controlParameterListRect, controlParameterIndexProp.intValue, parameterStrings);

                    SoundParameter controlParameter = controlParametersList[controlParameterIndexProp.intValue];

                    SerializedProperty controlRangeMinValue = property.FindPropertyRelative("controlRangeMin");
                    SerializedProperty controlRangeMaxValue = property.FindPropertyRelative("controlRangeMax");
                    float tmpRangeMin = Mathf.Clamp(controlRangeMinValue.floatValue, controlParameter.minLimit, controlParameter.maxLimit);
                    float tmpRangeMax = Mathf.Clamp(controlRangeMaxValue.floatValue, controlParameter.minLimit, controlParameter.maxLimit);

                    // Set projection range
                    EditorGUI.MinMaxSlider(EditorUtils.NextLineRect(ref position), new GUIContent("Control Parameter Range"),
                        ref tmpRangeMin,
                        ref tmpRangeMax,
                        controlParameter.minLimit,
                        controlParameter.maxLimit);

                    (Rect minDistanceRect, Rect maxDistanceRect) = CalculateDualPrefixedRectangles(ref position);
                    controlRangeMinValue.floatValue = EditorGUI.FloatField(minDistanceRect, tmpRangeMin);
                    controlRangeMaxValue.floatValue = EditorGUI.FloatField(maxDistanceRect, tmpRangeMax);
                }
            }
        }

        private ISoundFilterControl UpdateSoundFilter(ref Rect position, SerializedProperty property)
        {
            SerializedProperty effect = EditorUtils.AppendProperty(ref position, property, "filterType");
            FilterType typeEnum = (FilterType)effect.enumValueIndex;

            SerializedProperty soundEffect = property.FindPropertyRelative("_soundEffect");
            Utils.AssertNotNull(soundEffect, "Unable to reach serialized effect");

            ISoundFilterControl soundFilterControl = soundEffect.managedReferenceValue as ISoundFilterControl;

            if (soundFilterControl == null || typeEnum != soundFilterControl.filterType)
            {
                switch (typeEnum)
                {
                    case FilterType.Chorus:
                        soundFilterControl = new SoundFilterControlChorus();
                        break;
                    case FilterType.Distortion:
                        soundFilterControl = new SoundFilterControlDistortion();
                        break;
                    case FilterType.Echo:
                        soundFilterControl = new SoundFilterControlEcho();
                        break;
                    case FilterType.HighPass:
                        soundFilterControl = new SoundFilterControlHighPass();
                        break;
                    case FilterType.LowPass:
                        soundFilterControl = new SoundFilterControlLowPass();
                        break;
                    case FilterType.Reverb:
                        soundFilterControl = new SoundFilterControlReverb();
                        break;
                }

                soundEffect.managedReferenceValue = soundFilterControl;
                soundEffect.serializedObject.ApplyModifiedProperties();
                property.FindPropertyRelative("filterParameterIndex").intValue = 0;
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }

            return soundFilterControl;
        }

    }
}

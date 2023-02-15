using UnityEditor;
using UnityEngine;

namespace Sound
{
    [CustomPropertyDrawer(typeof(SoundParameter))]
    class SoundParameterDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float margin = 0;
            int lines = 5;
            return EditorUtils.LinesHeight(lines) + margin;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            Rect labelRect = new Rect(position.x, position.y, position.width, EditorUtils.LineHeight);
            EditorGUI.LabelField(labelRect, label, EditorStyles.boldLabel);

            EditorGUI.indentLevel++;

            EditorUtils.AppendProperty(ref position, property, "name");

            EditorUtils.AppendProperty(ref position, property, "_value");
            EditorUtils.AppendProperty(ref position, property, "minLimit");
            EditorUtils.AppendProperty(ref position, property, "maxLimit");
            EditorGUI.indentLevel--;

            if(EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}

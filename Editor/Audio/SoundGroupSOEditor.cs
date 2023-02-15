using UnityEditor;
using UnityEngine;

namespace Sound
{
    [CustomEditor(typeof(SoundGroupSO), true)]
    class SoundGroupSOEditor : Editor
    {
        SoundGroupSO soundGroup;
        bool requestClear = false;

        private void OnEnable()
        {
            soundGroup = target as SoundGroupSO;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("playingPolicy"));
            EditorUtils.SectionTitle("Randomization");
            EditorUtils.PrefixedMinMaxSlider("Pitch", ref soundGroup.pitchVariation);
            EditorUtils.PrefixedMinMaxSlider("dB", ref soundGroup.dBVariation);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("sounds"));

            if (requestClear)
            {
                if (GUILayout.Button("Cancel"))
                {
                    requestClear = false;
                }
                if (GUILayout.Button("Confirm"))
                {
                    soundGroup.sounds.Clear();
                    requestClear = false;
                }
            }
            else
            {
                if (GUILayout.Button("Clear"))
                {
                    requestClear = true;
                    EditorUtility.SetDirty(soundGroup);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

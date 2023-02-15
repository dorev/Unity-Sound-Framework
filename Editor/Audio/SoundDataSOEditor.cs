using UnityEngine;
using UnityEditor;

namespace Sound
{
    [CustomEditor(typeof(SoundDataSO))]
    public class SoundDataSOEditor : Editor
    {
        SoundDataSO soundData;
        static GameObject gameObject = null;
        static AudioSource audioSource = null;
        GUIStyle style = new GUIStyle(EditorStyles.label);

        public void OnEnable()
        {
            soundData = target as SoundDataSO;
            style.normal.textColor = Color.red;
        }

        public void OnDisable()
        {
            if (audioSource != null)
            {
                audioSource.Stop();
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            if (soundData.audioClip != null)
            {
                GUILayout.Label("Click waveform for preview");
                Texture2D waveform = AssetPreview.GetAssetPreview(soundData.audioClip);
                if (GUILayout.Button(waveform, GUILayout.ExpandWidth(true)))
                {
                    if (gameObject == null)
                    {
                        GameObject gameObject = new GameObject("InspectorSoundPlayer");
                        gameObject.hideFlags = HideFlags.HideAndDontSave;
                        audioSource = gameObject.AddComponent<AudioSource>();
                    }

                    audioSource.clip = soundData.audioClip;
                    soundData.Apply(audioSource);
                    audioSource.loop = false;
                    audioSource.Play();
                }
            }
            else
            {
                GUILayout.Label("No AudioClip to preview in SoundData", style);
            }
        }
    }
}

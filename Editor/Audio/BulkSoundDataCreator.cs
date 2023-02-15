using UnityEditor;
using UnityEngine;

namespace Sound
{
    class BulkSoundDataCreator : EditorWindow
    {
        string destinationFolder;
        string prefix = string.Empty;

        [MenuItem("Sound/Bulk Sound Data Creator")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            BulkSoundDataCreator window = (BulkSoundDataCreator)EditorWindow.GetWindow(typeof(BulkSoundDataCreator));
            window.titleContent = new GUIContent { text = "Bulk SoundData Creator" };
            window.Show();
        }

        void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            destinationFolder = EditorGUILayout.TextField("Destination folder", destinationFolder);
            if (GUILayout.Button("...", GUILayout.MaxWidth(30)))
            {
                string absolutePath = EditorUtility.OpenFolderPanel("Select raw audio root folder", Application.dataPath, "");
                destinationFolder = "Assets" + absolutePath.Substring(Application.dataPath.Length);
            }

            EditorGUILayout.EndHorizontal();
            EditorUtils.PrefixedText("Add prefix", ref prefix);
            prefix.Trim();

            DragAndDropArea<AudioClip>.Draw("\nDrop audio clips to create SoundData\nscriptable object to target folder in bulk", EditorUtils.LinesHeight(4),
            audioClip =>
            {
                SoundDataSO soundData = ScriptableObject.CreateInstance<SoundDataSO>();
                soundData.audioClip = audioClip;

                string assetPath = string.IsNullOrEmpty(prefix)
                    ? $@"{destinationFolder}/{audioClip.name}.asset"
                    : $@"{destinationFolder}/{prefix}{audioClip.name}.asset";

                AssetDatabase.CreateAsset(soundData, assetPath);
            });

        }
    }
}

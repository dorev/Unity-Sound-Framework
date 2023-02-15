using System.Collections.Generic;
using UnityEditor;

namespace Sound
{
    [CustomEditor(typeof(RfxGroupSO), true)]
    class RfxGroupSOEditor : Editor
    {
        private RfxGroupSO rfxSoundGroup;
        private List<RfxGroupItem> rfxSoundGroupItemsToRemove = new List<RfxGroupItem>();

        private void OnEnable()
        {
            rfxSoundGroup = target as RfxGroupSO;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            foreach (RfxGroupItem item in rfxSoundGroup.rfxGroupItems)
            {
                DrawRfxSoundGroupItem(item);
            }

            foreach (RfxGroupItem item in rfxSoundGroupItemsToRemove)
            {
                rfxSoundGroup.rfxGroupItems.Remove(item);
            }

            rfxSoundGroupItemsToRemove.Clear();

            DragAndDropArea<SoundGroupSO>.Draw("\nDrop SoundGroupSO\n", EditorUtils.LinesHeight(3), soundGroup =>
            {
                rfxSoundGroup.rfxGroupItems.Add(new RfxGroupItem
                {
                    soundGroup = soundGroup,
                    overrideGroupVariations = false,
                });

                EditorUtility.SetDirty(rfxSoundGroup);
            });

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(rfxSoundGroup);
            }
        }

        private void DrawRfxSoundGroupItem(RfxGroupItem item)
        {
            EditorGUILayout.LabelField(item.soundGroup.name, EditorStyles.boldLabel);

            EditorUtils.PrefixedBool("Override randomization", ref item.overrideGroupVariations);
            EditorGUI.BeginDisabledGroup(!item.overrideGroupVariations);
            EditorUtils.PrefixedMinMaxSlider("Pitch", ref item.pitchVariation);
            EditorUtils.PrefixedMinMaxSlider("dB", ref item.dBVariation);
            EditorGUI.EndDisabledGroup();
            EditorUtils.PrefixedMinMaxSlider("Time interval", ref item.timeInterval);

            EditorGUILayout.BeginHorizontal();
            EditorUtils.Spaces(10);
            EditorUtils.CancelConfirmButton("Remove", ref item.requestRemove, () =>
            {
                rfxSoundGroupItemsToRemove.Add(item);
            });
            EditorGUILayout.EndHorizontal();

            EditorUtils.HorizontalLine();
        }
    }
}

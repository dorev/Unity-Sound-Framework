using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Sound
{
    [CreateAssetMenu(fileName = "New SoundBank", menuName = "Sound/Sound Bank")]
    public class SoundBankSO : ScriptableObject
    {
        public List<SoundDataSO> sounds;
        public List<SoundGroupSO> soundGroups;

        private Dictionary<string, SoundDataSO> _soundsByName = new Dictionary<string, SoundDataSO>();
        private Dictionary<string, SoundGroupSO> _soundGroupsByName = new Dictionary<string, SoundGroupSO>();

        private void OnValidate()
        {
            GenerateIndex();
            #if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            #endif
        }

        public void GenerateIndex()
        {
            _soundsByName.Clear();
            _soundGroupsByName.Clear();

            foreach (SoundDataSO sound in sounds)
            {
                if (!_soundsByName.TryAdd(sound.name, sound))
                {
                    Utils.HandleError("SoundBank cannot contain two sounds with the same name.");
                    return;
                }
            }

            foreach (SoundGroupSO soundGroup in soundGroups)
            {
                if (!_soundGroupsByName.TryAdd(soundGroup.name, soundGroup))
                {
                    Utils.HandleError("SoundBank cannot contain two sound groups with the same name.");
                    return;
                }
            }
        }

        public bool TryGetSound(string soundName, out SoundDataSO soundData)
        {
            return _soundsByName.TryGetValue(soundName, out soundData);
        }

        public bool TryGetSoundFromGroup(string soundGroupName, out SoundDataSO soundData, out SoundVariation soundVariation)
        {
            if (_soundGroupsByName.TryGetValue(soundGroupName, out SoundGroupSO soundGroup))
            {
                (soundData, soundVariation) = soundGroup.GetNextSound();
                return true;
            };

            soundData = null;
            soundVariation = null;
            return false;
        }
    }
}

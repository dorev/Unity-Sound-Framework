using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Sound
{
    [CreateAssetMenu(fileName = "New SoundGroup", menuName = "Sound/Sound Group")]
    public class SoundGroupSO : ScriptableObject
    {
        public List<SoundDataSO> sounds = new List<SoundDataSO>();
        public MinMaxFloat pitchVariation = new MinMaxFloat { min = 0.1f, low = 0.8f, high = 1.2f, max = 3f };
        public MinMaxFloat dBVariation = new MinMaxFloat { min = -24f, low = -6f, high = 0f, max = 0f };
        public int id { get; private set; }

        public enum PlayingPolicy
        {
            Random,
            RandomNoRepeat,
            Sequential
        }
        [SerializeField] public PlayingPolicy playingPolicy = PlayingPolicy.Random;

        private void OnEnable()
        {
            id = name.GetHashCode();
        }

        private int _nextIndex = 0;

        public (SoundDataSO, SoundVariation) GetNextSound()
        {
            Utils.Assert(sounds.Count > 0, "Trying to get sound from empty SoundGroup");

            SoundDataSO soundData = null;
            if (sounds.Count == 1)
            {
                soundData = sounds[0];
            }
            else
            {
                switch (playingPolicy)
                {
                    default:
                    case PlayingPolicy.Random:
                        soundData = sounds[Random.Range(0, sounds.Count)];
                        break;

                    case PlayingPolicy.Sequential:
                        soundData = sounds[_nextIndex++ % sounds.Count];
                        break;

                    case PlayingPolicy.RandomNoRepeat:
                        int indexToPlay = _nextIndex;
                        while (_nextIndex != indexToPlay)
                        {
                            _nextIndex = Random.Range(0, sounds.Count);
                        }
                        soundData = sounds[indexToPlay];
                        break;
                }
            }

            SoundVariation soundVariation = new SoundVariation
            {
                dB = dBVariation.GetRandom(),
                pitch = pitchVariation.GetRandom()
            };

            return (soundData, soundVariation);
        }

        private void OnValidate()
        {
            #if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            #endif
        }
    }
}

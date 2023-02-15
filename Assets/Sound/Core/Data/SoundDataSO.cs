using System.ComponentModel;
using UnityEditor;
using UnityEngine;

namespace Sound
{
    [CreateAssetMenu(fileName = "New SoundData", menuName = "Sound/Sound Data")]
    public class SoundDataSO : ScriptableObject
    {
        [SerializeField] public AudioClip audioClip;
        [Range(-100f, 0f)]
        [SerializeField] public float dB = 0f;
        [Range(0.1f, 3f)]
        [SerializeField] public float pitch = 1f;
        [SerializeField] public bool loopable = true;
        [SerializeField] public int id { get; private set; }

        private void OnStart()
        {
            id = audioClip.name.GetHashCode();
        }

        public void Apply(AudioSource audioSource)
        {
            Utils.AssertNotNull(audioSource, $"Attempting to configure SoundData {name} on a null AudioSource.");
            Utils.AssertNotNull(audioClip, $"Attempting to configure AudioSource while SoundData {name} has no associated AudioClip.");

            audioSource.clip = audioClip;
            audioSource.pitch = pitch;
            Utils.AddDecibel(dB, audioSource);

            if (loopable == false)
            {
                audioSource.loop = false;
            }
        }

        private void OnValidate()
        {
            #if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            #endif
        }
    }
}

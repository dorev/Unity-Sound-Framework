using System.Collections.Generic;
using UnityEngine;

namespace Sound
{
    [System.Serializable]
    public class RfxGroupItem
    {
        public bool overrideGroupVariations = false;
        public MinMaxFloat timeInterval = new MinMaxFloat { min = 1f, low = 5f, high = 15f, max = 30f };
        public MinMaxFloat pitchVariation = new MinMaxFloat { min = 0.1f, low = 0.8f, high = 1.2f, max = 3f };
        public MinMaxFloat dBVariation = new MinMaxFloat { min = -24f, low = -6f, high = 0f, max = 0f };
        public SoundGroupSO soundGroup;

#if UNITY_EDITOR
        [HideInInspector] public bool requestRemove = false;
#endif

        private float _nextTriggerTime = 0f;

        public bool NextOccurenceReached(out SoundDataSO soundData, out SoundVariation soundVariation)
        {
            if(Time.unscaledTime > _nextTriggerTime)
            {
                (soundData, soundVariation) = soundGroup.GetNextSound();

                if(overrideGroupVariations)
                {
                    soundVariation.pitch = pitchVariation.GetRandom();
                    soundVariation.dB = dBVariation.GetRandom();
                }

                _nextTriggerTime = Time.unscaledTime + timeInterval.GetRandom();

                return true;
            }

            soundData = null;
            soundVariation = null;
            return false;
        }

        public void ResetTriggerTime()
        {
            _nextTriggerTime = Time.unscaledTime + Random.Range(0, timeInterval.high);
        }
    }
}

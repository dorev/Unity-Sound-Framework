using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Sound
{
    [CreateAssetMenu(fileName = "New RFX Sound Group", menuName = "Sound/RFX Sound Group")]
    [System.Serializable]
    public class RfxGroupSO : ScriptableObject
    {
        public List<RfxGroupItem> rfxGroupItems;

        private List<AmbianceRfx> _registeredRfxs = new List<AmbianceRfx>();

        private void OnEnable()
        {
            _registeredRfxs.Clear();
        }

        public void Update()
        {
            foreach(RfxGroupItem item in rfxGroupItems)
            {
                if(item.NextOccurenceReached(out SoundDataSO soundData, out SoundVariation soundVariation))
                {
                    RandomRegisteredRfx().PlayRfx(soundData, soundVariation);
                }
            }
        }

        public void Register(AmbianceRfx rfx)
        {
            Utils.AssertNotNull(rfx, $"Invalid attempt to register {rfx.GetType().Name} in {GetType().Name}.");

            if(_registeredRfxs.Count == 0)
            {
                foreach(RfxGroupItem item in rfxGroupItems)
                {
                    item.ResetTriggerTime();
                }
            }

            if (!_registeredRfxs.Contains(rfx))
            {
                _registeredRfxs.Add(rfx);
            }
        }

        public void Unregister(AmbianceRfx rfx)
        {
            if (rfx != null && _registeredRfxs.Contains(rfx))
            {
                _registeredRfxs.Remove(rfx);
            }
        }

        private AmbianceRfx RandomRegisteredRfx()
        {
            return _registeredRfxs[Random.Range(0, _registeredRfxs.Count)];
        }

        private void OnValidate()
        {
            #if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            #endif
        }
    }
}

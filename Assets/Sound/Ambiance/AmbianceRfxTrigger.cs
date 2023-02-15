using UnityEngine;
using System.Collections.Generic;
using Cinemachine;

namespace Sound
{
    public class AmbianceRfxTrigger : MonoBehaviour
    {
        [TagField]
        public string triggeringTag;
        public List<AmbianceRfx> rfxToTrigger;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(triggeringTag))
            {
                foreach (AmbianceRfx rfx in rfxToTrigger)
                {
                    rfx.Activate();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(triggeringTag))
            {
                foreach (AmbianceRfx rfx in rfxToTrigger)
                {
                    rfx.Deactivate();
                }
            }
        }
    }
}

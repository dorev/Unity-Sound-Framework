using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sound
{
    public enum FootstepType
    {
        Walking,
        Running,
        Jumping,
        Landing
    }

    [CreateAssetMenu(fileName = "New Footstep Data", menuName = "Sound/Footstep Data")]
    public class FootstepDataSO : ScriptableObject
    {
        [TagField] public List<string> tags;
        public List<TerrainLayer> terrainLayers = new List<TerrainLayer>();
        public SoundGroupSO running;
        public SoundGroupSO walking;
        public SoundGroupSO jumping;
        public SoundGroupSO landing;

        public (SoundDataSO, SoundVariation) GetFootstepSoundWithVariation(FootstepType footstepType)
        {
            switch (footstepType)
            {
                case FootstepType.Walking: return walking != null ? walking.GetNextSound() : (null, null);
                case FootstepType.Running: return running != null ? running.GetNextSound() : (null, null);
                case FootstepType.Jumping: return jumping != null ? jumping.GetNextSound() : (null, null);
                case FootstepType.Landing: return landing != null ? landing.GetNextSound() : (null, null);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

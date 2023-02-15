using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sound
{
    public class FootstepPlayer : MonoBehaviour
    {
        public LayerMask groundLayerMask;
        public LayerMask terrainLayerMask;
        public float groundDetectionDistance;
        public FootstepDataSO defaultFootstep;
        public List<FootstepDataSO> footsteps = new List<FootstepDataSO>();
        public SoundEmitter soundEmitter;
        public SoundMixerTrackSO mixerTrack;
        public float minTimeBetweenSteps = 0.1f;

        private float _nextAllowedStepTime = 0;

        public void PlayFootstep(FootstepType footstepType)
        {
            float now = Time.unscaledTime;
            if (now > _nextAllowedStepTime)
            {
                _nextAllowedStepTime = now + minTimeBetweenSteps;
            }
            else
            {
                return;
            }

            FootstepDataSO footstepData = FindFootstepDataForCurrentGround();

            if (footstepData == null)
            {
                if (defaultFootstep != null)
                {
                    footstepData = defaultFootstep;
                }
                else
                {
                    throw new UnityException("No specific or default footstep sound available.");
                }
            }

            (SoundDataSO soundData, SoundVariation soundVariation) = footstepData.GetFootstepSoundWithVariation(footstepType);
            if (soundData != null)
            {
                mixerTrack.PlaySound(soundData, soundVariation, soundEmitter);
            }
            else
            {
                Debug.LogWarning($"Unable to find FootstepData for type {footstepType}.");
            }
        }

        private FootstepDataSO FindFootstepDataForCurrentGround()
        {
            Vector3 position = gameObject.transform.position;

            string groundTag = Utils.FindColliderTagBelow(position, groundDetectionDistance, groundLayerMask.value);
            if (!string.IsNullOrEmpty(groundTag))
            {
                return FindFootstepForGroundTag(groundTag);
            }

            TerrainLayer terrainLayer = Utils.FindTerrainLayerBelow(position, groundDetectionDistance, terrainLayerMask.value);
            if (terrainLayer != null)
            {
                return FindFootstepForTerrainLayer(terrainLayer);
            }

            return null;
        }

        private FootstepDataSO FindFootstepForGroundTag(string groundTag)
        {
            if (!string.IsNullOrEmpty(groundTag))
            {
                FootstepDataSO footstepData = footsteps.FirstOrDefault(footstepData =>
                {
                    foreach (string tag in footstepData.tags)
                    {
                        if (tag == groundTag)
                        {
                            return true;
                        }
                    }
                    return false;
                });

                if (footstepData != null)
                {
                    return footstepData;
                }
            }

            return defaultFootstep;
        }

        private FootstepDataSO FindFootstepForTerrainLayer(TerrainLayer terrainLayer)
        {
            if (terrainLayer != null)
            {
                FootstepDataSO footstepData = footsteps.FirstOrDefault(footstepData => footstepData.terrainLayers.Contains(terrainLayer));
                if (footstepData != null)
                {
                    return footstepData;
                }
            }

            return defaultFootstep;
        }

    }
}

using UnityEngine;
using System.Collections.Generic;

namespace Sound
{
    public class AmbiancePad : MonoBehaviour
    {
        public Collider ambianceSpaceCollider;
        public SoundMixerTrackSO mixerTrack;
        public GameObject targetListener;
        public SoundDataSO padSound;
        public float maxDistance = 20f;
        public AnimationCurve volumeRolloffCurve = Utils.DefaultAmbianceRolloff();
        public List<SoundControl> soundControls = new List<SoundControl>();

        private Vector3 _soundEmitterPosition;
        private SoundEmitter _soundEmitter;
        private AudioSourceConfigSO _audioSourceConfig;
        private AudioSource _padAudioSource;
        private ulong _padSoundInstanceId = SoundInstance.InvalidId;
        private bool _padIsPlaying;
        private float _distanceFromTarget = float.MaxValue;

        private void OnEnable()
        {
            Utils.AssertNotNull(ambianceSpaceCollider, $"{GetType().Name} has no associated collider.");
            Utils.AssertNotNull(targetListener, $"{GetType().Name} target listener should not be null.");

            if (_audioSourceConfig == null)
            {
                _audioSourceConfig = ScriptableObject.CreateInstance("AudioSourceConfigSO") as AudioSourceConfigSO;
            }
        }

        private void Update()
        {
            _soundEmitterPosition = ambianceSpaceCollider.ClosestPoint(targetListener.transform.position);
            _distanceFromTarget = Vector3.Distance(_soundEmitterPosition, targetListener.transform.position);

            if (_distanceFromTarget < maxDistance)
            {
                UpdatePadSourcePosition();
                UpdatePadSpatialBlend();

                if (!_padIsPlaying)
                {
                    Activate();
                }
            }
            else if (_padIsPlaying)
            {
                Deactivate();
            }

#if UNITY_EDITOR
            if (_soundEmitter != null)
            {
                Debug.DrawLine(_soundEmitterPosition, targetListener.transform.position, Color.green);
            }
#endif
        }

        private void UpdatePadSourcePosition()
        {
            if (_soundEmitter == null)
            {
                _soundEmitter = Utils.AppendSoundEmitter(gameObject, $"{GetType().Name} {padSound.name} Emitter");
            }

            _soundEmitter.transform.position = _soundEmitterPosition;
        }

        private void UpdatePadSpatialBlend()
        {
            if (_padAudioSource != null)
            {
                if (_soundEmitterPosition == targetListener.transform.position)
                {
                    _padAudioSource.spatialBlend = 0;
                    _padAudioSource.spatialize = false;
                }
                else if (_distanceFromTarget < maxDistance)
                {
                    _padAudioSource.spatialBlend = Mathf.Clamp01(_distanceFromTarget / maxDistance);
                    _padAudioSource.spatialize = true;
                }
            }
        }

        private void Activate()
        {
            _audioSourceConfig.spatialBlend = Mathf.Clamp01(_distanceFromTarget / maxDistance);
            _audioSourceConfig.loop = true;
            _audioSourceConfig.maxDistance = maxDistance;
            _audioSourceConfig.customRolloffCurve = volumeRolloffCurve;

            _padSoundInstanceId = mixerTrack.PlaySound(
                padSound,
                SoundVariation.RandomSeek(),
                _soundEmitter,
                _audioSourceConfig,
                soundControls,
                0.0f,
                ClearSoundEmitter);

            _padAudioSource = _soundEmitter.GetAudioSourceForSoundInstance(_padSoundInstanceId);
            _padIsPlaying = true;
        }

        private void Deactivate()
        {
            if (_padSoundInstanceId != SoundInstance.InvalidId)
            {
                mixerTrack.Stop(_padSoundInstanceId);
                _padIsPlaying = false;
            }
        }

        private void ClearSoundEmitter()
        {
            _soundEmitter.Kill();
            GameObject.Destroy(_soundEmitter.gameObject);
            _soundEmitter = null;
            _padAudioSource = null;
            _padSoundInstanceId = SoundInstance.InvalidId;
        }
    }
}

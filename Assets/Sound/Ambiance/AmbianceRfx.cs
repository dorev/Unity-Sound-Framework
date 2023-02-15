using UnityEngine;
using UnityEngine.Events;

namespace Sound
{
    public class AmbianceRfx : MonoBehaviour
    {
        public BoxCollider ambianceBox;

        public SoundMixerTrackSO mixerTrack;
        public RfxGroupSO rfxSoundGroup;
        public AudioSourceConfigSO audioSourceConfig;
        public UnityEvent<Vector3, string> onRfxOccurence;

        private int _refCount = 0;

        public void Update()
        {
            if (_refCount > 0)
            {
                rfxSoundGroup.Update();
            }
        }

        public void Activate()
        {
            _refCount++;

            if(_refCount == 1)
            {
                rfxSoundGroup.Register(this);
            }
        }

        public void Deactivate()
        {
            Utils.Assert(_refCount > 0, $"{GetType().Name} reference count should not be decremented below 0.");

            _refCount--;

            if (_refCount == 0)
            {
                rfxSoundGroup.Unregister(this);
            }
        }

        public ulong PlayRfx(SoundDataSO soundData, SoundVariation soundVariation)
        {
            SoundEmitter soundEmitter = Utils.AppendObjectWithComponent<SoundEmitter>(gameObject, $"{rfxSoundGroup.name} sound emitter");
            soundEmitter.transform.position = GetRandomPointInsideBox();

            onRfxOccurence?.Invoke(soundEmitter.transform.position, soundData.name);

            return mixerTrack.PlaySound(
                soundData,
                soundVariation,
                soundEmitter,
                audioSourceConfig,
                null,
                0f,
                () => { GameObject.Destroy(soundEmitter.gameObject); });
        }

        public Vector3 GetRandomPointInsideBox()
        {
            Vector3 extents = ambianceBox.size / 2f;
            Vector3 point = new Vector3(
                Random.Range(-extents.x, extents.x),
                Random.Range(-extents.y, extents.y),
                Random.Range(-extents.z, extents.z)
            ) + ambianceBox.center;

            return ambianceBox.transform.TransformPoint(point);
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace Sound
{
    public class SoundPlayer : Singleton<SoundPlayer>
    {
        public SoundMixerSO mixer;
        public AudioSourceConfigSO defaultEmitterConfig;
        public SoundEmitter defaultEmitter;

        private Dictionary<ulong, SoundInstance> _soundInstances = new Dictionary<ulong, SoundInstance>();
        private List<ulong> _soundInstancesIdToRemove = new List<ulong>();

        protected override void Awake()
        {
            base.Awake();
        }

        private void OnEnable()
        {
            mixer.onPlaySound += Play;
            mixer.onStopSound += Stop;
        }

        private void OnDisable()
        {
            mixer.onPlaySound -= Play;
            mixer.onStopSound -= Stop;

            foreach (SoundInstance soundInstance in _soundInstances.Values)
            {
                soundInstance.Kill();
            }
        }

        private void Update()
        {
            foreach (SoundInstance soundInstance in _soundInstances.Values)
            {
                soundInstance.Update();
                if (soundInstance.IsStopped || soundInstance.InError)
                {
                    _soundInstancesIdToRemove.Add(soundInstance.id);
                }
            }

            foreach (ulong soundInstanceId in _soundInstancesIdToRemove)
            {
                _soundInstances.Remove(soundInstanceId);
            }

            _soundInstancesIdToRemove.Clear();
        }

        public ulong Play(SoundRequest soundRequest)
        {
            Utils.AssertNotNull(soundRequest.soundData, $"{soundRequest.GetType().Name} contains no SoundData.");

            if (soundRequest.soundEmitter == null)
            {
                ConfigureForDefaultEmitter(ref soundRequest);
            }

            SoundInstance soundInstance = new SoundInstance(soundRequest);

            if (_soundInstances.TryAdd(soundInstance.id, soundInstance))
            {
                if (soundRequest.fadeInDuration > 0f)
                {
                    SoundEffectLinearFade fade = new SoundEffectLinearFade
                    {
                        fadeDuration = soundRequest.fadeInDuration,
                        startVolume = 0f,
                        endVolume = soundInstance.configuredVolume
                    };

                    soundInstance.AddEffect(fade);
                }

                soundInstance.Play();
                return soundInstance.id;
            }
            else
            {
                Utils.HandleError(
                    $"{GetType().Name}.Play requested with an already existing id. This means that either two MixerTrack " +
                    "have the same name or that, for some reason, a MixerTrack provided the same id twice.");

                return SoundInstance.InvalidId;
            }
        }

        public void Stop(ulong soundInstanceId, float fadeDuration)
        {
            if (_soundInstances.TryGetValue(soundInstanceId, out SoundInstance soundInstance))
            {
                soundInstance.Stop();
            }
            else
            {
                Utils.HandleError("SoundInstance id requested for Stop does not exist.");
            }
        }

        private void ConfigureForDefaultEmitter(ref SoundRequest soundRequest)
        {
            soundRequest.soundEmitter = GetDefaultEmitter();
            soundRequest.audioSourceConfig = defaultEmitterConfig;
        }

        private SoundEmitter GetDefaultEmitter()
        {
            if (defaultEmitter == null)
            {
                defaultEmitter = gameObject.AddComponent<SoundEmitter>();
            }

            return defaultEmitter;
        }
    }
}

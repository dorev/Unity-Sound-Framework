using UnityEngine;
using UnityEngine.Audio;
using System;

namespace Sound
{
    public static class Utils
    {
        public static AnimationCurve DefaultAmbianceRolloff()
        {
            return new AnimationCurve(
                new Keyframe(0f, 1f),
                new Keyframe(0.05f, 1f, 0.0175f, 0.175f, 0f, 0.135f),
                new Keyframe(0.5f, 0.3f, -1.5f, -1.5f, 0.33f, 0.33f),
                new Keyframe(1f, 0f)
            );
        }

        public static float DecibelToRatio(float dB)
        {
            return Mathf.Pow(10f, (dB / 10f));
        }

        public static float DecibelFromRatio(float ratio)
        {
            return 10f * Mathf.Log10(ratio);
        }

        public static void AddDecibel(float dB, AudioSource audioSource)
        {
            float value = DecibelToRatio(DecibelFromRatio(audioSource.volume) + dB);
            audioSource.volume = value;
        }

        public static void ConfigureOrderly(AudioSource audioSource, SoundDataSO soundData = null, SoundVariation soundVariation = null, AudioSourceConfigSO audioSourceConfig = null, AudioMixerGroup audioMixerGroupOverride = null)
        {
            Utils.AssertNotNull(audioSource, "Null AudioSource provided to setup.");

            if (soundData != null)
            {
                soundData.Apply(audioSource);

                if (soundVariation != null)
                {
                    soundVariation.Apply(audioSource);
                }
            }

            if (audioSourceConfig != null)
            {
                audioSourceConfig.Apply(audioSource);
            }

            if (audioMixerGroupOverride != null)
            {
                audioSource.outputAudioMixerGroup = audioMixerGroupOverride;
            }
        }

        public static float SquaredDistance(Vector3 position1, Vector3 position2)
        {
            float x = position1.x - position2.x;
            float y = position1.y - position2.y;
            float z = position1.z - position2.z;
            return x * x + y * y + z * z;
        }

        public static float SquaredHorizontalDistance(Vector3 position1, Vector3 position2)
        {
            float x = position1.x - position2.x;
            float z = position1.z - position2.z;
            return x * x + z * z;
        }

        public static bool FastDistanceSmallerThan(Vector3 a, Vector3 b, float distance)
        {
            return SquaredDistance(a, b) < (distance * distance);
        }

        public static GameObject CreateChildObject(GameObject parent, string name, params Type[] components)
        {
            GameObject gameObject = new GameObject(name, components);
            gameObject.transform.parent = parent.transform;
            return gameObject;
        }

        public static T AppendObjectWithComponent<T>(GameObject parentObject, string objectName)
        {
            GameObject gameObject = new GameObject(objectName, typeof(T));
            gameObject.transform.parent = parentObject.transform;
            return gameObject.GetComponent<T>();
        }

        public static T GetOrCreateComponent<T>(GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }

            return component;
        }

        public static SoundEmitter AppendSoundEmitter(GameObject parentObject, string objectName)
        {
            return AppendObjectWithComponent<SoundEmitter>(parentObject, objectName);
        }

        public static TerrainLayer FindTerrainLayerBelow(Vector3 position, float distance, int layerMask)
        {
            RaycastHit raycastHit;
            if (Physics.Raycast(position, Vector3.down, out raycastHit, distance, layerMask))
            {
                if (raycastHit.collider is TerrainCollider)
                {
                    Terrain terrain = raycastHit.collider.transform.gameObject.GetComponent<Terrain>();
                    if (terrain != null)
                    {
                        return GetStrongestTerrainLayer(position, terrain);
                    }
                }
            }

            return null;
        }

        public static string FindColliderTagBelow(Vector3 position, float distance, int layerMask)
        {
            RaycastHit raycastHit;
            if (Physics.Raycast(position, Vector3.down, out raycastHit, distance, layerMask))
            {
                return raycastHit.collider.tag;
            }

            return string.Empty;
        }

        private static TerrainLayer GetStrongestTerrainLayer(Vector3 position, Terrain terrain)
        {
            Vector3 terrainPosition = terrain.transform.position;
            TerrainData terrainData = terrain.terrainData;

            int x = Mathf.RoundToInt((position.x - terrainPosition.x) / terrainData.size.x * terrainData.alphamapWidth);
            int z = Mathf.RoundToInt((position.z - terrainPosition.z) / terrainData.size.z * terrainData.alphamapWidth);
            float[,,] splatMapData = terrainData.GetAlphamaps(x, z, 1, 1);

            float strongest = 0f;
            int maxIndex = 0;

            for (int i = 0; i < splatMapData.GetUpperBound(2) + 1; ++i)
            {
                if (splatMapData[0, 0, i] > strongest)
                {
                    maxIndex = i;
                    strongest = splatMapData[0, 0, i];
                }
            }

            return terrain.terrainData.terrainLayers[maxIndex];
        }

        public static void HandleError(string message)
        {
#if UNITY_EDITOR
            throw new UnityException(message);
#else
        Debug.LogError(message);
#endif
        }

        public static void HandleWarning(string message)
        {
            Debug.LogWarning(message);
        }

        public static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                HandleError(message);
            }
        }

        public static void AssertNotNull(object obj, string message)
        {
            if (obj == null)
            {
                HandleError(message);
            }
        }
    }
}

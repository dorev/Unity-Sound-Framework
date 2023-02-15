
[System.Serializable]
public class MinMaxFloat
{
    public float min;
    public float low;
    public float high;
    public float max;

    public float GetRandom()
    {
        return UnityEngine.Random.Range(low, high);
    }
}

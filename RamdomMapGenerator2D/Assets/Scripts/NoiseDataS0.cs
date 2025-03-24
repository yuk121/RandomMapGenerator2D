using UnityEngine;

[CreateAssetMenu(fileName = "Noise Settings", menuName = "Data/Noise Settings")]
public class NoiseDataS0 : ScriptableObject
{
    [Range(0.01f, 0.9f)]
    public float startFrequency = 0.1f;
    [Min(1)]
    public int octaves = 3;
    [Min(0)]
    public float persistance = 0.5f;
    [Min(0)]
    public float frequencyModifier = 1.2f;

    public Vector2Int offset = new Vector2Int(1000, 0);

    public int noiseRangeMin = 0;
    public int noiseRangeMax = 20;
}

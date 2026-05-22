using Roguelike.Noise;
using TMPro.EditorUtilities;
using UnityEngine;

public class BiomeValueGenerator : MonoBehaviour
{
    static public BiomeValueGenerator singleton;

    public float temperatureScale;
    public float humidityScale;

    [HideInInspector]public Vector2 temperatureOffset;
    [HideInInspector]public Vector2 humidityOffset;
    [HideInInspector]public Vector2 variationOffset;

    private int _seed;

    public void Awake()
    {
        singleton = this;
        _seed = 0;
        System.Random _prng = new System.Random(_seed + 69);
        temperatureOffset = new Vector2(_prng.Next(-10000, 10000), _prng.Next(-10000, 10000));
        humidityOffset = new Vector2(_prng.Next(-10000, 10000), _prng.Next(-10000, 10000));
        variationOffset = new Vector2(_prng.Next(-10000, 10000), _prng.Next(-10000, 10000));
        
    }

    public float GetTemperature(Vector2 coord) => GetTemperature(coord.x, coord.y);
    public float GetTemperature(float x, float y)
    {
        float dx = (x + temperatureOffset.x) / temperatureScale;
        float dy = (y + temperatureOffset.y) / temperatureScale;
        return Mathf.PerlinNoise(dx, dy);
    }

    public float GetHumidity(Vector2 coord) => GetHumidity(coord.x, coord.y);
    public float GetHumidity(float x, float y)
    {
        float dx = (x + humidityOffset.x) / humidityScale;
        float dy = (y + humidityOffset.y) / humidityScale;
        return Mathf.PerlinNoise(dx, dy);
    }

    public float GetVariation(Vector2 coord) => GetVariation(coord.x, coord.y);
    public float GetVariation(float x, float y)
    {
        return Noise.RandomNoise((int)x, (int)y, _seed);
    }
}

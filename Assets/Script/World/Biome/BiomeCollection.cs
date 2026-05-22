using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BiomeCollection", menuName = "Scriptable Objects/Biome/Biome Collection")]
public class BiomeCollection : ScriptableObject
{
    public BiomeData[] biomes;

    public int biomeBlendingFactor = 10;

    private Dictionary<Vector2Int, Dictionary<BiomeData, float>[,]> _chunkWeightsCache = new();

    public BiomeData GetBiome(int x, int y, Vector2Int chunkCoord)
    {
        float temperature = BiomeValueGenerator.singleton.GetTemperature(x + chunkCoord.x * Chunk.k_xSize, y + chunkCoord.y * Chunk.k_ySize);
        float humidity = BiomeValueGenerator.singleton.GetHumidity(x + chunkCoord.x * Chunk.k_xSize, y + chunkCoord.y * Chunk.k_ySize);
        
        BiomeData bestBiome = null;
        float bestMatch = float.MaxValue;

        foreach (BiomeData biome in biomes)
        {
            float match = GetBiomeMatchValue(biome, temperature, humidity);
            if (match < bestMatch)
            {
                bestMatch = match;
                bestBiome = biome;
            }
        }
        return bestBiome;
    }

    public Dictionary<BiomeData, float>[,] GetChunkBiomeWeights(Vector2Int chunkCoord)
    {
        if (_chunkWeightsCache.TryGetValue(chunkCoord, out var cachedWeights))
        {
            return cachedWeights;
        }
        
        Dictionary<BiomeData, float>[,] result = new Dictionary<BiomeData, float>[Chunk.k_xSize + 1, Chunk.k_ySize + 1];
        for (int y = 0; y < Chunk.k_ySize + 1; ++y)
        {
            for (int x = 0; x < Chunk.k_xSize + 1; ++x)
            {
                if (result[x, y] == null)
                {
                
                    result[x, y] = new Dictionary<BiomeData, float>();
                }
                float temperature = BiomeValueGenerator.singleton.GetTemperature(x + chunkCoord.x * Chunk.k_xSize, y + chunkCoord.y * Chunk.k_ySize);
                float humidity = BiomeValueGenerator.singleton.GetHumidity(x + chunkCoord.x * Chunk.k_xSize, y + chunkCoord.y * Chunk.k_ySize);
                foreach (BiomeData biome in biomes)
                {
                    float match = GetBiomeMatchValue(biome, temperature, humidity);
                    match = 1 / (match + 0.1f);
                    match = Mathf.Pow(match + 1, biomeBlendingFactor);
                    result[x, y].Add(biome, match);
                }
            }
        }
        if (_chunkWeightsCache.Count > 256)
        {
            _chunkWeightsCache.Clear();
        }
        _chunkWeightsCache.Add(chunkCoord, result);
        return result;
    }
    
    public float[,] GetAltitudeMap(Vector2Int chunkCoord, ref float[,] data)
    {
        float[,] altitudeMap = new float[Chunk.k_xSize + 1, Chunk.k_ySize + 1];
        var chunkWeights = GetChunkBiomeWeights(chunkCoord);
        for (int y = 0; y < Chunk.k_ySize + 1; ++y)
        {
            for (int x = 0; x < Chunk.k_xSize + 1; ++x)
            {
                try
                {
                    var weights = chunkWeights[x, y];
                    float totalWeight = 0;
                    float totalAltitude = 0;

                    foreach (var weight in weights)
                    {
                        if(weight.Value <= 0.001f) 
                        {
                            continue;
                        }
                        totalWeight += weight.Value;
                        totalAltitude += weight.Key.GetHeight(x, y, ref data) * weight.Value;
                    }
                    altitudeMap[x, y] = totalAltitude / totalWeight;
                }
                catch (Exception e)
                {
                    Debug.Log("Error in GetAltitude: " + e);
                }
            }
        }

        return altitudeMap;
    }

    public float GetBiomeMatchValue(BiomeData biome, float temperature, float humidity)
    {
        float temperatureMatch = Mathf.Abs(biome.temperature - temperature);
        float humidityMatch = Mathf.Abs(biome.humidity - humidity);
        return temperatureMatch + humidityMatch;
    }

}

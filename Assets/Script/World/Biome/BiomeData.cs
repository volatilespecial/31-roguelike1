using System;
using System.Collections.Generic;

using UnityEngine;

using Roguelike.Tilemap.NTile;
using Roguelike.Tilemap.NProp;
using Roguelike.Noise;


public enum TerrainPropGeneration
{
    FOREST,
    RANDOM,
}

[Serializable]
public class TerrainProp
{
    public float influence;
    public float minValue;
    public PropType type;
    public TerrainPropGeneration generationType;
}

[Serializable]
public class TerrainType
{
    public TileType type;
    public int layerCount = 1;
}


[CreateAssetMenu(fileName = "BiomeData", menuName = "Scriptable Objects/Biome/Biome Data")]
public class BiomeData : ScriptableObject
{
    public string biomeName;
    public float temperature;
    public float humidity;

    public List<TerrainProp> terrainProp = new();
    public List<TerrainType> terrainTypes = new();
    
    public int maxHeight;

    public float GetHeight(int x, int y, ref float[,] data)
    {
        float height = data[x, y];
        return height * maxHeight;
    }

    public PropType GetPropType(int x, int y, int seed)
    {
        if (terrainProp.Count == 0) return PropType.NONE;
        PropType propType = PropType.NONE;
        float best = 0.0f;
        for(int i = 0; i < terrainProp.Count; ++i)
        {
            TerrainProp terrain = terrainProp[i];
            float minValue = terrain.minValue;
            switch (terrain.generationType)
            {
                case TerrainPropGeneration.FOREST:
                    {
                    float noise = Noise.RandomNoise(x, y, seed+i);
                    float perlin = Noise.PerlinNoise(x * 0.1f, y * 0.1f);
                    float value = noise * perlin;
                    if (value > minValue)
                    {
                        value = (value - minValue) * terrain.influence;
                        if (value > best) {
                            best = value;
                            propType = terrain.type;
                        }
                    }
                    break;
                    }
                case TerrainPropGeneration.RANDOM:
                    {
                        float value = Noise.RandomNoise(x, y, seed+i);
                        if (value > minValue)
                        {
                            value = (value - minValue) * terrain.influence;
                            if (value > best) {
                                best = value;
                                propType = terrain.type;
                            }
                        }
                        break;
                    }
            }
        }
        return propType;
    }

    public TileType GetBiomeTileType(int z, int maxz)
    {
        int layer = maxz - z + 1;
        if (z == 0) return TileType.WATER;
        foreach(TerrainType terrain in terrainTypes)
        {
            layer -= terrain.layerCount;
            if (layer <= 0) return terrain.type;
        }
        return terrainTypes[terrainTypes.Count - 1].type;        
    }
}

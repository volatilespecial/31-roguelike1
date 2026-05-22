using UnityEngine;

using Roguelike.Tilemap;
using Roguelike.Tilemap.NTile;
using Roguelike.Tilemap.NProp;
using Roguelike.Noise;
using Roguelike.Utils;

public class Chunk
{
    public static readonly int k_xSize = 17;
    public static readonly int k_ySize = 17;
    public static readonly int k_zSize = 64;

    public Tilemap tilemap;  
    public Vector3Int position;

    public Chunk(GameObject parent, Vector3Int pos)
    {
        position = pos;
        tilemap = new Tilemap("Tilemap " + pos.x + " " + pos.y, pos);
        tilemap.gameObject.transform.SetParent(parent.transform);
    }

    public void Generate(BiomeCollection biomeCollection)
    {
        tilemap.Clear();
        float[,] noiseMap = MapGenerator.Generate(new Vector2Int(k_xSize + 1, k_ySize + 1), new Vector2(position.x * Chunk.k_xSize, position.y * Chunk.k_ySize), MapType.PERLIN_FRACTAL_NOISE);
        float[,] altitudeMap = biomeCollection.GetAltitudeMap(new Vector2Int(position.x, position.y), ref noiseMap);
        for (int y = 0; y < k_ySize; ++y)
        {
            for (int x = 0; x < k_xSize; ++x)
            {
                BiomeData biome = biomeCollection.GetBiome(x, y, new Vector2Int(position.x, position.y));
                int maxz =  (int)altitudeMap[x, y];
                float variationValue = BiomeValueGenerator.singleton.GetVariation(x + position.x * k_xSize, y + position.y * k_ySize);
                for (int z = maxz; z >= 0; --z)
                {
                    TileType tileType = biome.GetBiomeTileType(z, maxz);
                    Vector3Int tilePosition = new Vector3Int(x + position.x * k_xSize, y + position.y * k_ySize, z);
                    int nbVariation = Tile.GetTileTypeVariationCount(tileType);
                    int variation = 0;
                    float variationThreshold = 0.95f;
                    if (nbVariation > 1 && variationValue > variationThreshold)
                    {
                        variation = (int)Mathf.Lerp(0, nbVariation, (variationValue - variationThreshold) / (1.0f - variationThreshold));
                    }
                    Tile tile = Tile.GetTileFromType(tileType, variation, tilePosition);
                    tilemap.SetTile(tilePosition, tile, false);
                }
                Vector3Int propPosition = new Vector3Int(x + position.x * k_xSize, y + position.y * k_ySize, maxz+1);
                PropType type = biome.GetPropType(propPosition.x, propPosition.y, 0); 
                if (type != PropType.NONE) tilemap.SetProp(propPosition, Prop.GetPropFromType(type, propPosition), false);

            }
        }
        UpdateBorderedTile();
        tilemap.Generate();
    }


    public void UpdateBorderedTile()
    {
        for (int z = 0; z < k_zSize; ++z)
        {
            for (int y = 0; y < k_ySize; ++y)
            {
                for (int x = 0; x < k_xSize; ++x)
                {
                    Vector3Int local = new Vector3Int(x, y, z);
                    if (tilemap.GetTile(local) == null) continue;
                    Vector3Int[] offsets =
                    {
                        new Vector3Int( 1,  0,  0) + local,  
                        new Vector3Int(-1,  0,  0) + local,  
                        new Vector3Int( 0,  1,  0) + local,  
                        new Vector3Int( 0, -1,  0) + local,  
                    };
                    // Check each direction
                    TileFlagBorderDirection flag = 0;
                    flag |= IsInBound(offsets[0]) 
                            ? (tilemap.GetTile(offsets[0]) == null 
                                ? TileFlagBorderDirection.E 
                                : 0)
                            : 0;
                    flag |= IsInBound(offsets[1]) 
                            ? (tilemap.GetTile(offsets[1]) == null 
                                ? TileFlagBorderDirection.W 
                                : 0)
                            : 0;
                    flag |= IsInBound(offsets[2]) 
                            ? (tilemap.GetTile(offsets[2]) == null 
                                ? TileFlagBorderDirection.N
                                : 0)
                            : 0;
                    flag |= IsInBound(offsets[3]) 
                            ? (tilemap.GetTile(offsets[3]) == null 
                                ? TileFlagBorderDirection.S 
                                : 0)
                            : 0;
                    tilemap.GetTile(local).SetTileBorderDirection(flag);
                }   
            }
        }
    }

    public void UpdateBorderedTile(Tilemap neighbordtilemap, Vector3Int neighbordPos)
    {
        Vector3Int direction = neighbordPos - position;
        for (int z = 0; z < k_zSize; ++z)
        {
            bool xdir = direction.x == 0;
            int size = xdir ? k_xSize : k_ySize;
            for (int i = 0; i < size; ++i)
            {
                int x = xdir ? i : (direction.x < 0 ? 0 : (k_xSize - 1));
                int y = xdir ? (direction.y < 0 ? 0 : (k_ySize - 1)) : i;
                Vector3Int local = new Vector3Int(x, y, z);
                if (tilemap.GetTile(local) == null) continue;
                Vector3Int neighbord = Coordinate.IsoToChunkLocalPosition(local + direction);  
                TileFlagBorderDirection flag = tilemap.GetTile(local).Flag;
                if (direction.x < 0)
                {
                    flag |= IsInBound(neighbord) 
                            ? (neighbordtilemap.GetTile(neighbord) == null 
                                ? TileFlagBorderDirection.W 
                                : 0)
                            : TileFlagBorderDirection.W;
                }
                else if (direction.x > 0)
                {
                    flag |= IsInBound(neighbord) 
                            ? (neighbordtilemap.GetTile(neighbord) == null 
                                ? TileFlagBorderDirection.E 
                                : 0)
                            : TileFlagBorderDirection.E;
                }
                else if (direction.y < 0)
                {
                    flag |= IsInBound(neighbord) 
                            ? (neighbordtilemap.GetTile(neighbord) == null 
                                ? TileFlagBorderDirection.S
                                : 0)
                            : TileFlagBorderDirection.S;
                }
                else
                {
                    flag |= IsInBound(neighbord) 
                            ? (neighbordtilemap.GetTile(neighbord) == null 
                                ? TileFlagBorderDirection.N 
                                : 0)
                            : TileFlagBorderDirection.N;
                }
                tilemap.GetTile(local).SetTileBorderDirection(flag);
            }
        }
    }

    public static bool IsInBound(Vector3Int local)
    {
        if (local.x < 0 || local.x >= k_xSize) return false;
        if (local.y < 0 || local.y >= k_ySize) return false;
        if (local.z < 0 || local.z >= k_zSize) return false;
        return true;
    }

    public static uint LocalToIndex(Vector3Int local)
    {
        return (uint)(local.z * k_ySize * k_xSize + local.y * k_xSize + local.x);
    }
}

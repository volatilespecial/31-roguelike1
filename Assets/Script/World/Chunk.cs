using UnityEngine;

using Roguelike.Tilemaps;
using Roguelike.Noise;
using Roguelike.Utils;

public class Chunk
{
    public static readonly int k_xSize = 17;
    public static readonly int k_ySize = 17;
    public static readonly int k_zSize = 10;

    public Tilemap tilemap;    
    public Vector3Int position;

    public Chunk(GameObject parent, Vector3Int pos)
    {
        position = pos;
        tilemap = new Tilemap("Tilemap " + pos.x + " " + pos.y);
        tilemap.gameObject.transform.SetParent(parent.transform);
    }

    public void Generate()
    {
        tilemap.Clear();
        float[] noiseMap = MapGenerator.Generate(new Vector2Int(k_xSize, k_ySize), new Vector2(position.x, position.y), MapType.PERLIN_NOISE);
        for (int y = 0; y < k_ySize; ++y)
        {
            for (int x = 0; x < k_xSize; ++x)
            {
                TileType type;
                int maxz =  Mathf.RoundToInt(noiseMap[y * k_xSize + x] * 5 + 2);
                switch (maxz)
                {
                    case 2:
                        type = TileType.WATER;
                        break;
                    case 3: 
                        type = TileType.SAND;
                        break;
                    case 4:
                        type = TileType.GRASS;
                        break;
                    case 5:
                        type = TileType.ROCK;
                        break;
                    default:
                        type = TileType.DIRT;
                        break;
                }
                for (int z = maxz; z >= 0; --z)
                {
                    Tile tile;
                    Vector3Int tilePosition = new Vector3Int(x + position.x * k_xSize, y + position.y * k_ySize, z);
                    if (z == maxz) tile = Tile.GetTileFromType(type, 0, tilePosition);
                    else tile = Tile.GetTileFromType(TileType.DIRT, 0, tilePosition);
                    tilemap.SetTile(tilePosition, tile, false);
                }
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

    public static int LocalToIndex(Vector3Int local)
    {
        return local.z * k_ySize * k_xSize + local.y * k_xSize + local.x;
    }
}

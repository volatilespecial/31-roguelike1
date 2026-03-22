using System.Collections.Generic;
using UnityEngine;

using Roguelike.Tilemaps;
using Roguelike.Noise;

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

    public void Generate(Dictionary<string, Sprite[]> sprites)
    {
        tilemap.tiles.Clear();
        // float[] noiseMap = MapGenerator.Generate(new Vector2Int(k_xSize, k_ySize), new Vector2(position.x, position.y), MapType.PERLIN_NOISE);
        // float[] noiseMap = MapGenerator.Generate(new Vector2Int(k_xSize, k_ySize), new Vector2(position.x, position.y), MapType.PERLIN_FRACTAL_NOISE);
        float[] noiseMap = MapGenerator.Generate(new Vector2Int(k_xSize, k_ySize), new Vector2(position.x, position.y), MapType.DIAMOND_SQUARE);
        for (int y = 0; y < k_ySize; ++y)
        {
            for (int x = 0; x < k_xSize; ++x)
            {
                string type = "";
                int maxz =  Mathf.RoundToInt(noiseMap[y * k_xSize + x]);
                switch (maxz)
                {
                    case 2:
                        type = "Water";
                        break;
                    case 3: 
                        type = "Sand";
                        break;
                    case 4:
                        type = "Grass";
                        break;
                    case 5:
                        type = "Rock";
                        break;
                    default:
                        type = "Dirt";
                        break;
                }
                for (int z = maxz; z >= 0; --z)
                {
                    TileData tileData = new TileData();
                    int random;
                    if (sprites[type].Length == 1) random = 0;
                    else
                    {
                        random = Random.Range(0, 10);
                        if (random <= 7) random = 0;
                        else random = Random.Range(1, sprites[type].Length);
                    }
                    if (z == maxz) tileData.sprite = sprites[type][random];
                    else tileData.sprite = sprites["Dirt"][0];
                    tilemap.SetTile(new Vector3Int(x + position.x * k_xSize, y + position.y * k_ySize, z), tileData);
                }
            }
        }
        tilemap.Generate();
    }

}

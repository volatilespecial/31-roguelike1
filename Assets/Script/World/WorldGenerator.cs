using System.Collections.Generic;
using UnityEngine;

using Roguelike.Utils;

public class WorldGenerator : MonoBehaviour
{
    public static WorldGenerator singleton = null;


    private Dictionary<Vector3Int, Chunk> _chunks;

    public int renderDistance;
    public Transform center;
    public BiomeCollection biomeCollection;

    void Awake()
    {
        singleton = this;
        _chunks = new Dictionary<Vector3Int, Chunk>();
    }


    void Update()
    {
        UpdateChunk();
    }

    public void UpdateChunk()
    {
        Vector3Int pos = Coordinate.WorldToChunk(center.position);
        // Generate new Chunk
        for (int i = -renderDistance; i < renderDistance+1; ++i)
        {
            for (int j = -renderDistance; j < renderDistance+1; ++j)
            {
                Vector3Int chunkPos = new Vector3Int(i + pos.x, j + pos.y, 0);
                Vector3Int[] offsets =
                {
                    new Vector3Int( 1,  0,  0) + chunkPos,
                    new Vector3Int(-1,  0,  0) + chunkPos,
                    new Vector3Int( 0,  1,  0) + chunkPos,
                    new Vector3Int( 0, -1,  0) + chunkPos,

                };
                if (GetChunk(chunkPos) != null) continue;
                Chunk newChunk = new Chunk(gameObject, chunkPos);
                newChunk.Generate(biomeCollection);
                for (int k = 0; k < 4; ++k)
                {
                    if (GetChunk(offsets[k]) == null) continue;
                    Chunk neighbord = GetChunk(offsets[k]);
                    neighbord.UpdateBorderedTile(newChunk.tilemap, newChunk.position);
                    newChunk.UpdateBorderedTile(neighbord.tilemap, neighbord.position);
                }
                _chunks.Add(chunkPos, newChunk);
            }
        }
        // Update current chunk
        // Remove non visible chunk
        List<Vector3Int> toRemove = new List<Vector3Int>();
        foreach(var (chunkPos, _) in _chunks)
        {
            Vector3Int diff = pos - chunkPos;
            if (diff.x > renderDistance || diff.x < -renderDistance 
                || diff.y > renderDistance || diff.y < -renderDistance)
            {
                toRemove.Add(chunkPos);
            }
        }

        foreach(var chunkPos in toRemove)
        {
            UnityEngine.Object.Destroy(_chunks[chunkPos].tilemap.gameObject);
            _chunks.Remove(chunkPos);
        }
    }
    
    public Chunk GetChunk(Vector3Int position)
    {
        if (!_chunks.ContainsKey(position)) return null;
        return _chunks[position];
    }

}

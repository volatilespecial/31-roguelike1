using UnityEngine;

using Roguelike.Utils;
using NUnit.Framework;

namespace Roguelike.Tilemaps{
    public class Tilemap
    {
        public GameObject gameObject = null;
        private Tile[] _tiles;
        
        public Tilemap(string name = "Tilemap")
        {
            gameObject = new GameObject(name);
            _tiles = new Tile[Chunk.k_xSize * Chunk.k_ySize * Chunk.k_zSize];
        }

        public void Generate()
        {
            string name = "Tilemap";
            if (!gameObject) gameObject = new GameObject(name);

            Vector3Int[] offsets =
            {
                new Vector3Int(-1, 0, 0),
                new Vector3Int(0, -1, 0),
                new Vector3Int(0, 0, 1),
            };

            for (int z = 0; z < Chunk.k_zSize; ++z)
            {
                for (int y = 0; y < Chunk.k_ySize; ++y)
                {
                    for (int x = 0; x < Chunk.k_xSize; ++x)
                    {
                        Vector3Int tilePos = new Vector3Int(x, y, z);
                        int i = Chunk.LocalToIndex(tilePos);
                        bool visible = false;
                        if (_tiles[i] == null) continue;
                        for (int j = 0; j < 3 && !visible; ++j)
                        {
                            Vector3Int offPos = tilePos + offsets[j];
                            if (!Chunk.IsInBound(offPos) || _tiles[Chunk.LocalToIndex(offPos)] == null) visible = true;
                        }
                        if (!visible)
                        {
                            if (_tiles[i].gameObject != null) _tiles[i].Destroy();
                            continue;  
                        }
                        if (_tiles[i].gameObject != null) continue;
                        _tiles[i].Generate();
                        _tiles[i].gameObject.transform.SetParent(gameObject.transform); 
                    }
                }
            }
        }

        public Tile GetTile(Vector3Int position, bool isLocalPos = true)
        {
            Vector3Int local = position;
            if (!isLocalPos)
                local = Coordinate.IsoToChunkLocalPosition(position); 
            if (!Chunk.IsInBound(local)) return null;
            return _tiles[Chunk.LocalToIndex(local)];
        }

        public int SetTile(Vector3Int position, Tile tile,  bool isLocalPos = true)
        {
            Vector3Int local = position;
            if (!isLocalPos)
                local = Coordinate.IsoToChunkLocalPosition(position);
            if (!Chunk.IsInBound(local)) return 1;
            _tiles[Chunk.LocalToIndex(local)] = tile;
            return 0;
        }

        public void Clear()
        {
            int size = Chunk.k_xSize * Chunk.k_ySize * Chunk.k_zSize;
            for (int i = 0; i < size; ++i)
                _tiles[i] = null;          
        }
    }
}



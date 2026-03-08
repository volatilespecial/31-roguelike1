using System;
using System.Collections.Generic;
using UnityEngine;

using Roguelike.Utils;
using Roguelike.Resources;

namespace Roguelike.Tilemaps
{
    public enum TileType
    {
        WATER, 
        SAND, 
        DIRT, GRASS, 
        ROCK,
    }

    [Serializable]
    public class TileData
    {
        public Sprite sprite;
        public TileType type;
        public int variation;
        public TileFlagBorderDirection flag;

        public TileData(Sprite sprite, TileType type, int variation, TileFlagBorderDirection flag = 0)
        {
            this.sprite = sprite;
            this.type = type;
            this.variation = variation;
            this.flag = flag; 
        }
    }

    public class Tile
    {
        protected static Dictionary<TileType, TileBorderDirectionSprite[]> _sprites = ResourcesManager.LoadTilesData();
        public GameObject gameObject = null;

        protected Vector3Int _position;
        protected TileData _tileData;

        public Tile(Vector3Int position, TileData tileData){
            _position = position;
            _tileData = tileData;
        }

        public void Generate()
        {
            if (gameObject) UnityEngine.Object.Destroy(gameObject);
            gameObject = new GameObject("Tile " + _position.x + " " + _position.y + " " + _position.z);
            SpriteRenderer sp = gameObject.AddComponent<SpriteRenderer>();
            sp.sprite = _tileData.sprite;

            Vector3 pos = Coordinate.IsoToWorld(_position);

            sp.sortingOrder = -(_position.x + _position.y) + _position.z * 5;
            gameObject.transform.position = new Vector3(pos.x, pos.y, 0.0f);
        }

        public Sprite GetSprite(){ return _tileData.sprite; }
        public void SetSprite(Sprite sprite)
        {
            _tileData.sprite = sprite;
            gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
        }

        public static Tile GetTileFromType(TileType type, Vector3Int position)
        {
            switch (type)
            {
                case TileType.WATER:    return new Water(position);
                case TileType.SAND:     return new Sand(position);
                case TileType.DIRT:     return new Dirt(position);
                case TileType.GRASS:    return new Grass(position);
                case TileType.ROCK:     return new Rock(position);
            }
            return null;
        }
    }

#region TileType
    public class Water : Tile
    {
        public Water(Vector3Int position) : base(position, new TileData(_sprites[TileType.WATER][0], false)) { }
    }
    public class Sand : Tile
    {
        public Sand(Vector3Int position) : base(position, new TileData(_sprites[TileType.SAND][0], false)) { }
    }
    public class Dirt : Tile
    {
        public Dirt(Vector3Int position) : base(position, new TileData(_sprites[TileType.DIRT][0], false)) { }
    }
    public class Grass : Tile
    {
        public Grass(Vector3Int position) : base(position, new TileData(_sprites[TileType.GRASS][0], false)) { }
    }
    public class Rock : Tile
    {
        public Rock(Vector3Int position) : base(position, new TileData(_sprites[TileType.ROCK][0], false)) { }
    }
#endregion
}
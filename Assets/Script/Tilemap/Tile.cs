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

        public Vector3Int position;
        protected TileData _tileData;

        public Tile(Vector3Int position, TileData tileData){
            this.position   = position;
            _tileData       = tileData;
        }
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

        public static Tile GetTileFromType(TileType type, int variation,  Vector3Int position)
        {
            return type switch
            {
                TileType.WATER          => new Water(position, variation),
                TileType.SAND           => new Sand(position, variation),
                TileType.DIRT           => new Dirt(position, variation),
                TileType.GRASS          => new Grass(position, variation),
                TileType.ROCK           => new Rock(position, variation),
                _                       => null,
            };
        }
            }
            return null;
        }
    }


#region TileType
    public class Water : Tile
    {
        public Water(Vector3Int position, int variation) 
            : base(position, new TileData(_sprites[TileType.WATER][variation].sp, TileType.WATER, variation)) 
        { }
    }
    public class Sand : Tile
    {
        public Sand(Vector3Int position, int variation) 
            : base(position, new TileData(_sprites[TileType.SAND][variation].sp, TileType.SAND, variation)) 
        { }
    }
    public class Dirt : Tile
    {
        public Dirt(Vector3Int position, int variation) 
            : base(position, new TileData(_sprites[TileType.DIRT][variation].sp, TileType.DIRT, variation)) 
        { }
    }
    public class Grass : Tile
    {
        public Grass(Vector3Int position, int variation) 
            : base(position, new TileData(_sprites[TileType.GRASS][variation].sp, TileType.GRASS, variation)) 
        { }
    }
    public class Rock : Tile
    {
        public Rock(Vector3Int position, int variation) 
            : base(position, new TileData(_sprites[TileType.ROCK][variation].sp, TileType.ROCK, variation)) 
        { }
    }
#endregion
}
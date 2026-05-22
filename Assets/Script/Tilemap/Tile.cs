using System;
using System.Collections.Generic;
using UnityEngine;

using Roguelike.Utils;
using Roguelike.Resources;

namespace Roguelike.Tilemap.NTile
{
    public enum TileType
    {
        NONE,
        WATER, 
        SAND, 
        DIRT, GRASS, 
        ROCK,
        SNOW, 
        SNOWY_DIRT, SNOWY_GRASS, SNOWY_ROCK,  
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
            this.sprite     = sprite;
            this.type       = type;
            this.variation  = variation;
            this.flag       = flag; 
        }
    }

    public class Tile
    {
        protected static GameObject _prefab = ResourcesManager.LoadTilePrefab();
        protected static Dictionary<TileType, TileBorderDirectionSprite[]> _sprites = ResourcesManager.LoadTilesData();
        public GameObject gameObject = null;

        public Vector3Int position;
        protected TileData _tileData;

        public Tile(Vector3Int position, TileData tileData){
            this.position   = position;
            _tileData       = tileData;
        }

        public void SetTileBorderDirection(TileFlagBorderDirection flag)
        {
            _tileData.flag = flag;
            Sprite = GetTileDirectionSprite(flag);
            if (gameObject)
            {
                SpriteRenderer sp = gameObject.GetComponent<SpriteRenderer>();
                sp.sprite = Sprite;
                sp.color = GetTileBrightnessColorFromElevation(position.z);
            }
                
        }

        public void Generate()
        {
            if (gameObject) UnityEngine.Object.Destroy(gameObject);
            gameObject = UnityEngine.Object.Instantiate(_prefab);
            SpriteRenderer sp = gameObject.GetComponent<SpriteRenderer>();
            sp.sprite = Sprite;
            sp.color = GetTileBrightnessColorFromElevation(position.z);
            Vector3 pos = Coordinate.IsoToWorld(position);
            SetSortingOrder();
            gameObject.transform.position = new Vector3(pos.x, pos.y, pos.z);
        }

        public void Destroy()
        {
            if (gameObject) UnityEngine.Object.Destroy(gameObject);
            gameObject = null;
        }

        public void SetSortingOrder()
        {
            int average = 0;
            for (int y = -1; y <= 1; ++y)
            {
                for (int x = -1; x <= 1; ++x)
                {
                    average += -1 * (x + position.x + y + position.y); 
                }
            }
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = average / 9 + position.z;
        }

        public Sprite GetTileDirectionSprite(TileFlagBorderDirection flag)
        {
            return flag switch
            {
                TileFlagBorderDirection.NSEW  => _sprites[Type][Variation].spNSEW,
                TileFlagBorderDirection.N     => _sprites[Type][Variation].spN,
                TileFlagBorderDirection.S     => _sprites[Type][Variation].spS,
                TileFlagBorderDirection.E     => _sprites[Type][Variation].spE,
                TileFlagBorderDirection.W     => _sprites[Type][Variation].spW,
                TileFlagBorderDirection.NW    => _sprites[Type][Variation].spNW,
                TileFlagBorderDirection.SE    => _sprites[Type][Variation].spSE,
                TileFlagBorderDirection.SW    => _sprites[Type][Variation].spSW,
                TileFlagBorderDirection.NE    => _sprites[Type][Variation].spNE,
                TileFlagBorderDirection.NS    => _sprites[Type][Variation].spNS,
                TileFlagBorderDirection.EW    => _sprites[Type][Variation].spEW,
                TileFlagBorderDirection.NSW   => _sprites[Type][Variation].spNSW,
                TileFlagBorderDirection.SEW   => _sprites[Type][Variation].spSEW,
                TileFlagBorderDirection.NSE   => _sprites[Type][Variation].spNSE,
                TileFlagBorderDirection.NEW   => _sprites[Type][Variation].spNEW,
                _                             => _sprites[Type][Variation].sp,
            };
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
                TileType.SNOW           => new Snow(position, variation),
                TileType.SNOWY_DIRT     => new SnowyDirt(position, variation),
                TileType.SNOWY_GRASS    => new SnowyGrass(position, variation),
                TileType.SNOWY_ROCK     => new SnowyRock(position, variation),
                _                       => null,
            };
        }

        public static int GetTileTypeVariationCount(TileType type)
        {
            return _sprites[type].Length;
        }

        public static Color GetTileBrightnessColorFromElevation(float elevation)
        {
            float brightness = Mathf.Lerp(0.8f, 1.0f, elevation / Chunk.k_zSize);
            return new Color(brightness, brightness, brightness);
        }

        public Sprite Sprite 
        {
            get => _tileData.sprite; 
            set
            {
                _tileData.sprite = value;
                if (gameObject) gameObject.GetComponent<SpriteRenderer>().sprite = value;
            }
        }

        public TileType Type { get => _tileData.type; }
        public int Variation { get => _tileData.variation; }
        public TileFlagBorderDirection Flag { get => _tileData.flag; }
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

    public class Snow : Tile
    {
        public Snow(Vector3Int position, int variation) 
            : base(position, new TileData(_sprites[TileType.ROCK][variation].sp, TileType.SNOW, variation)) 
        { }
    }
    
    public class SnowyDirt : Tile
    {
        public SnowyDirt(Vector3Int position, int variation) 
            : base(position, new TileData(_sprites[TileType.ROCK][variation].sp, TileType.SNOWY_DIRT, variation)) 
        { }
    }

    public class SnowyGrass : Tile
    {
        public SnowyGrass(Vector3Int position, int variation) 
            : base(position, new TileData(_sprites[TileType.ROCK][variation].sp, TileType.SNOWY_GRASS, variation)) 
        { }
    }

    public class SnowyRock : Tile
    {
        public SnowyRock(Vector3Int position, int variation) 
            : base(position, new TileData(_sprites[TileType.ROCK][variation].sp, TileType.SNOWY_ROCK, variation)) 
        { }
    }

#endregion
}
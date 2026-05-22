using System.Collections.Generic;
using UnityEngine;

using Roguelike.Tilemap.NTile;
using Roguelike.Tilemap.NProp;

namespace Roguelike.Resources{
    public static class ResourcesManager
    {
        public static Dictionary<TileType, TileBorderDirectionSprite[]> LoadTilesData()
        {
            Dictionary<TileType, TileBorderDirectionSprite[]> ret = new Dictionary<TileType, TileBorderDirectionSprite[]>();
            Object[] tilesData = UnityEngine.Resources.LoadAll("ScriptableObject/Tile", typeof(TileSpriteData));
            foreach(TileSpriteData data in tilesData)
            {  
                ret.Add(data.type, data.sprites); 
            }
            return ret;
        }

        public static Dictionary<PropType, PropData> LoadPropData()
        {
            Dictionary<PropType, PropData> ret = new Dictionary<PropType, PropData>();
            Object[] propsData = UnityEngine.Resources.LoadAll("ScriptableObject/Prop", typeof(PropData));
            foreach (PropData data in propsData)
            {
                ret.Add(data.type, data);
            }
            return ret;
        }

        public static GameObject LoadTilePrefab()
        {
            Object tile = UnityEngine.Resources.Load("Prefabs/Tile", typeof(GameObject));
            return (GameObject)tile;
        }
    }
}

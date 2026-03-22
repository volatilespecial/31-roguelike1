using System.Collections.Generic;
using UnityEngine;

using Roguelike.Tilemaps;

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
    }
}

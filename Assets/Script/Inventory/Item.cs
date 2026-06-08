using System.Collections;
using System.Collections.Generic;
using Roguelike.Tilemap.NProp;
using Roguelike.Tilemap.NTile;
using UnityEngine;



[CreateAssetMenu(menuName = "Scriptable object/Item")]
public class Item : ScriptableObject
{
    [Header("Only gameplay")]
    public TileType tile;
    public PropType prop;
    public ItemType type;
    public ActionType actionType;
    public Vector2Int range = new Vector2Int(5, 4);

    [Header("Only UI")]
    public bool stackable = true;

    [Header("Both")]
    public Sprite image;
}

public enum ItemType
{
    BuildingBlock,
    Tool,
    Equipment

}

public enum ActionType
{
    Build,
    Dig,
    Mine,
    Cut,
    Attack,
    Defense
}

using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "Tilemap/TileData")]
public class TileSpriteData : ScriptableObject
{
    public Roguelike.Tilemaps.TileType type;
    public TileBorderDirectionSprite[] sprites;
}

public enum TileFlagBorderDirection
{
    None = 0,
    N    = 1,
    S    = 2,
    E    = 4,
    W    = 8,
    NW   = 9,
    SE   = 6,
    NE   = 5,
    SW   = 10,
    NS   = 3,
    EW   = 12,
    NSE  = 7,
    NSW  = 11,
    NEW  = 13,
    SEW  = 14,
    NSEW = 15
}

[Serializable]
public struct TileBorderDirectionSprite
{
    public Sprite spNSEW;
    public Sprite sp;
    public Sprite spW;
    public Sprite spN;
    public Sprite spS;
    public Sprite spE;
    public Sprite spNW;
    public Sprite spSE;
    public Sprite spNE;
    public Sprite spSW;
    public Sprite spNS;
    public Sprite spEW;
    public Sprite spNSW;
    public Sprite spSEW;
    public Sprite spNSE;
    public Sprite spNEW;
}
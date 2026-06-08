using UnityEngine;

using System.Collections.Generic;

using Roguelike.Utils;
using Roguelike.Tilemap.NTile;
using Roguelike.Tilemap.NProp;

namespace Roguelike.Tilemap{
    public class Tilemap
    {
        public Vector3Int position;
        public GameObject gameObject = null;
        public bool generated = false;
        private int[] _arrangement;
        private Dictionary<uint, Tile> _tiles;
        private Dictionary<uint, Prop> _props;
        private Dictionary<GameObject, Tile> _gTiles;
        private Dictionary<GameObject, Prop> _gProps;

        public Tilemap(string name, Vector3Int position)
        {
            this.position   = position;
            gameObject      = new GameObject(name);
            _arrangement    = new int[Chunk.k_xSize * Chunk.k_ySize * Chunk.k_zSize];
            _tiles          = new Dictionary<uint, Tile>();
            _props          = new Dictionary<uint, Prop>();
            _gTiles         = new Dictionary<GameObject, Tile>();
            _gProps         = new Dictionary<GameObject, Prop>();
        }

        public void Generate()
        {
            string name = "Tilemap";
            if (!gameObject) gameObject = new GameObject(name);

            
            for (int z = 0; z < Chunk.k_zSize; ++z)
            {
                for (int y = 0; y < Chunk.k_ySize; ++y)
                {
                    for (int x = 0; x < Chunk.k_xSize; ++x)
                    {
                        Vector3Int tilePos = new Vector3Int(x, y, z);
                        uint i = Chunk.LocalToIndex(tilePos);
                        bool visible = false;
                        if (_arrangement[i] == 0) continue;
                        if (_arrangement[i] > 0)
                        {
                            uint index = (uint)_arrangement[i]; 
                            visible = isVisible(tilePos);
                            if (!visible)
                            {
                                if (_tiles[index].gameObject != null) _tiles[index].Destroy();
                                continue;  
                            }
                            Tile tile = _tiles[index];
                            if (_tiles[index].gameObject != null) continue;
                            GenerateTile(tile);
                        }
                        else
                        {
                            uint index = (uint)-_arrangement[i];
                            if (_props[index].gameObject != null) continue;
                            Prop prop = _props[index];
                            GenerateProp(prop);
                        }
                    }
                }
            }
            generated = true;
        }

        public bool ContainObject(Vector3Int position, bool isLocalPos = true)
        {
            Vector3Int local = position;
            if (!isLocalPos)
                local = Coordinate.IsoToChunkLocalPosition(position); 
            if (!Chunk.IsInBound(local)) return true;
            int index = _arrangement[Chunk.LocalToIndex(local)];
            if (index == 0) return false;
            return true;
        }

        public Tile GetTile(Vector3Int position, bool isLocalPos = true)
        {
            Vector3Int local = position;
            if (!isLocalPos)
                local = Coordinate.IsoToChunkLocalPosition(position); 
            if (!Chunk.IsInBound(local)) return null;
            int index = _arrangement[Chunk.LocalToIndex(local)];
            if (index <= 0) return null;
            return _tiles[(uint)index];
        }

        public Tile GetTile(GameObject gb)
        {
            if (_gTiles.ContainsKey(gb)) return _gTiles[gb];
            return null;
        }

        public int SetTile(Vector3Int position, Tile tile,  bool isLocalPos = true)
        {
            Vector3Int local = position;
            if (!isLocalPos)
                local = Coordinate.IsoToChunkLocalPosition(position);
            if (!Chunk.IsInBound(local)) return 1;
            
            uint index = Chunk.LocalToIndex(local);
            if (_arrangement[index] < 0) return 2;
            if (_arrangement[index] > 0) {
                Tile replacedTile = _tiles[(uint)_arrangement[index]];
                _tiles.Remove((uint)_arrangement[index]);
                DestroyTile(replacedTile);
            }
            
            _arrangement[index] = (int)index + 1;
            if (generated && isVisible(local)) {
                GenerateTile(tile);
                UpdateTile(tile);
            }
            _tiles[(uint)_arrangement[index]] = tile; 
            return 0;
        }

        public int RemoveTile(Vector3Int position, bool isLocalPos = true)
        {
            Vector3Int local = position;
            if (!isLocalPos)
                local = Coordinate.IsoToChunkLocalPosition(position);
            if (!Chunk.IsInBound(local)) return 1;

            uint index = Chunk.LocalToIndex(local);
            if (_arrangement[index] < 0) return 2;

            if (_arrangement[index] > 0) {
                Tile replacedTile = _tiles[(uint)_arrangement[index]];
                _tiles.Remove((uint)_arrangement[index]);
                DestroyTile(replacedTile);
            }
            UpdateNeighborTile(local);
            _arrangement[index] = 0;
            return 0;
        }

        public Prop GetProp(Vector3Int position, bool isLocalPos = true)
        {
            Vector3Int local = position;
            if (!isLocalPos)
                local = Coordinate.IsoToChunkLocalPosition(position); 
            if (!Chunk.IsInBound(local)) return null;
            int index = _arrangement[Chunk.LocalToIndex(local)];
            if (index >= 0) return null;
            return _props[(uint)-index];
        }
        
        public Prop GetProp(GameObject gb)
        {
            if (_gProps.ContainsKey(gb)) return _gProps[gb];
            return null;
        }

        public int SetProp(Vector3Int position, Prop prop,  bool isLocalPos = true)
        {
            Vector3Int local = position;
            if (!isLocalPos)
                local = Coordinate.IsoToChunkLocalPosition(position);
            if (!Chunk.IsInBound(local)) return 1;
            
            uint index = Chunk.LocalToIndex(local);
            if (_arrangement[index] > 0) return 2;
            if (_arrangement[index] < 0) {
                Prop replacedProp = _props[(uint)-_arrangement[index]];
                _props.Remove((uint)-_arrangement[index]);
                DestroyProp(replacedProp);
            }

            _arrangement[index] = -((int)index + 1);
            if (generated && isVisible(local)) 
                GenerateProp(prop);
            _props[(uint)-_arrangement[index]] = prop;
            return 0;
        }

        public int RemoveProp(Vector3Int position, bool isLocalPos = true)
        {
            Vector3Int local = position;
            if (!isLocalPos)
                local = Coordinate.IsoToChunkLocalPosition(position);
            if (!Chunk.IsInBound(local)) return 1;

            uint index = Chunk.LocalToIndex(local);
            if (_arrangement[index] > 0) return 2;

            if (_arrangement[index] < 0) {
                Prop replacedProp = _props[(uint)-_arrangement[index]];
                _props.Remove((uint)-_arrangement[index]);
                DestroyProp(replacedProp);
             }
            _arrangement[index] = 0;
            return 0;
        }

        public void Clear()
        {
            int size = Chunk.k_xSize * Chunk.k_ySize * Chunk.k_zSize;
            for (int i = 0; i < size; ++i)
            {
                if (_arrangement[i] == 0) continue;
                if (_arrangement[i] > 0) _tiles[(uint)_arrangement[i]].Destroy();
                else _props[(uint)-_arrangement[i]].Destroy();
                _arrangement[i] = 0;
            }
            _tiles.Clear();
            _props.Clear();
        }

        private bool isVisible(Vector3Int tilePos)
        {
            Vector3Int[] offsets =
            {
                new Vector3Int(-1, 0, 0),
                new Vector3Int(0, -1, 0),
                new Vector3Int(0, 0, 1),
            };

            bool visible = false;
            for (int j = 0; j < offsets.Length && !visible; ++j)
            {
                Vector3Int offPos = tilePos + offsets[j];
                if (!Chunk.IsInBound(offPos) || _arrangement[Chunk.LocalToIndex(offPos)] <= 0) visible = true;
            }
            return visible;
        }

        private void GenerateTile(Tile tile)
        {
            tile.Generate();
            tile.gameObject.transform.SetParent(gameObject.transform); 
            _gTiles[tile.gameObject] = tile;
        }

        private void GenerateProp(Prop prop)
        {
            prop.Generate();
            prop.gameObject.transform.SetParent(gameObject.transform); 
            _gProps[prop.gameObject] = prop;
        }


        private void DestroyTile(Tile tile)
        {
            if (tile.gameObject == null) return;
            if (_gTiles.ContainsKey(tile.gameObject)) 
                _gTiles.Remove(tile.gameObject);   
            tile.Destroy();   
        }

        private void DestroyProp(Prop prop)
        {
            if (prop.gameObject == null) return;
            if (_gProps.ContainsKey(prop.gameObject)) 
                _gProps.Remove(prop.gameObject);   
            prop.Destroy();
        }


        private void UpdateTile(Tile tile)
        {
            Vector3Int tilePos = Coordinate.IsoToChunkLocalPosition(tile.position);
            Vector3Int[] flagOffsets =
            {
                new Vector3Int( 1,  0,  0),
                new Vector3Int(-1,  0,  0),
                new Vector3Int( 0,  1,  0),
                new Vector3Int( 0, -1,  0),
            };

            TileFlagBorderDirection[] flags =
            {
                TileFlagBorderDirection.E,
                TileFlagBorderDirection.W,
                TileFlagBorderDirection.N,
                TileFlagBorderDirection.S,
            };

            TileFlagBorderDirection[] flagMasks =
            {
                TileFlagBorderDirection.NSE,
                TileFlagBorderDirection.NSW,
                TileFlagBorderDirection.NEW,
                TileFlagBorderDirection.SEW,
            };

            
            TileFlagBorderDirection tileFlags = 0;
            for (int j = 0; j < flagOffsets.Length; ++j)
            {
                Vector3Int offPos = tilePos + flagOffsets[j];
                if (Chunk.IsInBound(offPos))
                {
                    Tile offTile = GetTile(offPos); 
                    if (offTile == null)
                    {
                        tileFlags |= flags[j];
                    }
                    else
                    {
                        offTile.SetTileBorderDirection(offTile.Flag & flagMasks[j]);
                    }
                }

                else
                {
                    Chunk neighborChunk = WorldGenerator.singleton.GetChunk(position + flagOffsets[j]);
                    if (neighborChunk == null) continue;
                    Tile offTile = neighborChunk.tilemap.GetTile(Coordinate.IsoToChunkLocalPosition(offPos));
                    if (offTile == null)
                    {
                        tileFlags |= flags[j];
                    }
                    else
                    {
                        offTile.SetTileBorderDirection(offTile.Flag & flagMasks[j]);
                    }
                }
            }
            tile.SetTileBorderDirection(tileFlags);
        }


        private void UpdateNeighborTile(Vector3Int tilePos)
        {
            Vector3Int[] visibilityOffsets =
            {
                new Vector3Int(1, 0, 0),
                new Vector3Int(0, 1, 0),
                new Vector3Int(0, 0, -1),
            };
            
            // Update tile that could be visible
            for (int j = 0; j < visibilityOffsets.Length; ++j)
            {
                Vector3Int offPos = tilePos + visibilityOffsets[j];
                Tile offTile = GetTile(offPos); 
                if (offTile != null && offTile.gameObject == null)
                {
                    GenerateTile(offTile);
                }
            }

            // Update Bordered Tile of neighbor tile
            Vector3Int[] flagOffsets =
            {
                new Vector3Int( 1,  0,  0),
                new Vector3Int(-1,  0,  0),
                new Vector3Int( 0, 1,  0),
                new Vector3Int( 0, -1,  0),
            };

            TileFlagBorderDirection[] flags =
            {
                TileFlagBorderDirection.W,
                TileFlagBorderDirection.E,
                TileFlagBorderDirection.S,
                TileFlagBorderDirection.N,
            };

            for (int j = 0; j < flagOffsets.Length; ++j)
            {
                Vector3Int offPos = tilePos + flagOffsets[j];
                if (Chunk.IsInBound(offPos))
                {
                    Tile offTile = GetTile(offPos); 
                    if (offTile != null)
                    {
                        offTile.SetTileBorderDirection(offTile.Flag | flags[j]);
                    }
                }

                else
                {
                    Chunk neighborChunk = WorldGenerator.singleton.GetChunk(position + flagOffsets[j]);
                    if (neighborChunk == null) continue;
                    Tile offTile = neighborChunk.tilemap.GetTile(Coordinate.IsoToChunkLocalPosition(offPos));
                    if (offTile != null)
                    {
                        offTile.SetTileBorderDirection(offTile.Flag | flags[j]);
                    }
                }
                
            }

        }

    }
}



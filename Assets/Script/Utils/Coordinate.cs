using UnityEngine;

namespace Roguelike.Utils
{
    public static class Coordinate
    {
        public static readonly float tileWidth = 1f;
        public static readonly float tileHeight = 0.5f;

        public static readonly float elevationScale = 0.25f;

        public static Vector3Int WorldToIso(Vector3 worldPos)
        {
            float halfWidth = tileWidth * 0.5f; 
            float halfHeight = tileHeight * 0.5f; 
            float y = worldPos.y - worldPos.z * elevationScale - elevationScale;  
            float a = worldPos.x / halfWidth; 
            float b = y / halfHeight; 
            int isoX = Mathf.RoundToInt((a + b) * 0.5f); 
            int isoY = Mathf.RoundToInt((b - a) * 0.5f); 
            int isoZ = Mathf.RoundToInt(worldPos.z); 
            return new Vector3Int(isoX, isoY, isoZ);
        }

        public static Vector3 WorldToIsoCoordinate(Vector3 worldPos)
        {
            float halfWidth = tileWidth * 0.5f; 
            float halfHeight = tileHeight * 0.5f; 
            float y = worldPos.y - worldPos.z;  
            float a = worldPos.x / halfWidth; 
            float b = y / halfHeight;  
            float isoX = (a + b) * 0.5f; 
            float isoY = (b - a) * 0.5f; 
            float isoZ = worldPos.z; 
            return new Vector3(isoX, isoY, (int)isoZ); 
        }


        public static Vector3Int WorldToChunk(Vector3 worldPos)
        {
            Vector3Int iso = WorldToIso(worldPos);
            return new Vector3Int(Mathf.FloorToInt((float)iso.x / (float)Chunk.k_xSize), 
                                  Mathf.FloorToInt((float)iso.y / (float)Chunk.k_ySize),
                                  0);
        }

        public static Vector3 IsoToWorld(Vector3Int iso)
        {
            float x = (iso.x - iso.y) * (tileWidth * 0.5f);  
            float y = (iso.x + iso.y) * (tileHeight * 0.5f) + iso.z * elevationScale;
            return new Vector3(x, y, iso.z);
        
        }
        
        public static Vector3 IsoToWorld(Vector3 iso)
        {
            float x = (iso.x - iso.y) * (tileWidth * 0.5f);  
            float y = (iso.x + iso.y) * (tileHeight * 0.5f) + iso.z * elevationScale;
            return new Vector3(x, y, iso.z);
        }

        public static Vector3Int IsoToChunk(Vector3Int iso)
        {
            return new Vector3Int(Mathf.FloorToInt((float)iso.x / (float)Chunk.k_xSize), 
                                  Mathf.FloorToInt((float)iso.y / (float)Chunk.k_ySize),
                                  0);
        }

        public static Vector3Int IsoToChunk(Vector3 iso)
        {
            return new Vector3Int(Mathf.FloorToInt(iso.x / Chunk.k_xSize), 
                                  Mathf.FloorToInt(iso.y / Chunk.k_ySize),
                                  0);
        }

        public static Vector3Int IsoToChunkLocalPosition(Vector3Int iso)
        {
            int x = Math.Mod(iso.x, Chunk.k_xSize);
            int y = Math.Mod(iso.y, Chunk.k_ySize);
            return new Vector3Int(x, y, iso.z);
        }
        
        public static Vector3Int IsoToChunkLocalPosition(Vector3 iso)
        {
            int x = Math.Mod((int)iso.x, Chunk.k_xSize);
            int y = Math.Mod((int)iso.y, Chunk.k_ySize);
            return new Vector3Int(x, y, (int)iso.z);
        }

    }
}


    

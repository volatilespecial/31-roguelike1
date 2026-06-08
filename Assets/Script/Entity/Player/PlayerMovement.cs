using UnityEngine;
using UnityEngine.InputSystem;

using Roguelike.Utils;
using Roguelike.Tilemap;

public class PlayerMovement : MonoBehaviour
{
    private InputAction _moveAction;
    public WorldGenerator worldGenerator;
    public float speed;
    
    public Vector3 tilePosition = new Vector3(0, 0, 0);
    private Vector3Int lastTilePosition = new Vector3Int(0, 0, 0);

    void Start()
    {
        transform.position = Coordinate.IsoToWorld(tilePosition);
        _moveAction = InputSystem.actions.FindAction("Move");
    }

    void Update()
    {
        Vector3 dirValue = _moveAction.ReadValue<Vector2>();
        Vector3 worldDir = new Vector3(dirValue.x, dirValue.y, 0);
        Vector3 direction = Coordinate.WorldToIsoCoordinate(worldDir);
        tilePosition = Vector3.Normalize(direction) * speed * Time.deltaTime + tilePosition;
        if (lastTilePosition != Vector3Int.FloorToInt(tilePosition))
        {
            Vector3Int chunkPos = Coordinate.IsoToChunk(Vector3Int.FloorToInt(tilePosition));
            Vector3Int tilePos = Coordinate.IsoToChunkLocalPosition(Vector3Int.FloorToInt(tilePosition));
            Chunk chunk = worldGenerator.GetChunk(chunkPos);
            bool found = false;
            while(chunk.tilemap.GetTile(tilePos) != null || !found)
            {
                if (tilePos.z < 0)
                {
                    tilePos.z = 0;
                    break;
                }
                if (chunk.tilemap.GetTile(tilePos) != null)
                {
                    found = true;
                    ++tilePos.z;
                }
                else --tilePos.z;
            }
            tilePosition.z = tilePos.z;
            lastTilePosition = tilePos;            
        }
        Vector3 temp = Coordinate.IsoToWorld(tilePosition);
        transform.position = new Vector3(temp.x, temp.y, 0);
        SetSortingOrder();
    }

    private void SetSortingOrder()
    {
        int average = 0;
        for (int y = -1; y <= 1; ++y)
        {
            for (int x = -1; x <= 1; ++x)
            {
                average += -1 * (x + Mathf.FloorToInt(tilePosition.x) + y + Mathf.FloorToInt(tilePosition.y)); 
            }
        }
        transform.GetComponent<SpriteRenderer>().sortingOrder = average / 9 + (int)tilePosition.z;
    }
}
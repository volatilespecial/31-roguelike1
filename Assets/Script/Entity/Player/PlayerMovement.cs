using UnityEngine;
using UnityEngine.InputSystem;

using Roguelike.Utils;
using Roguelike.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    private InputAction _moveAction;
    
    public WorldGenerator worldGenerator;
    public float speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 tempPos = Vector3.zero;
        Vector3Int chunkPos = Vector3Int.zero;
        Vector3Int tilePos = Vector3Int.zero;

        Vector2 moveValue = _moveAction.ReadValue<Vector2>();
        moveValue = moveValue.normalized;

        int zValue = 0;
        while (zValue < Chunk.k_zSize)
        {
            tempPos = new Vector3(transform.position.x, transform.position.y, zValue);
            chunkPos = Coordinate.WorldToChunk(tempPos);
            tilePos = Coordinate.WorldToIso(tempPos);
            // if (!worldGenerator.chunks.ContainsKey(chunkPos)) break; 
            // else if (worldGenerator.chunks[chunkPos].tilemap.tiles.ContainsKey(tilePos)) ;
            // else break;
            zValue += 1;
            // break;
        }

        Debug.Log("Chunk : " + chunkPos.x + " " + chunkPos.y + " Tile : " + tilePos.x + " " + tilePos.y + " " + zValue);
        transform.Translate(new Vector3(moveValue.x, moveValue.y, 0.0f) * speed);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = (int)(tilePos.x + tilePos.y) * (-1) + (int)zValue * 5;
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

using Roguelike.Utils;
using Roguelike.Tilemap.NTile;
using Roguelike.Tilemap.NProp;

public abstract class WorldSelector : MonoBehaviour
{
    public Camera cam;
    public WorldGenerator worldGenerator;

    public GameObject player;
    private PlayerMovement _playerMovement;
    
    private float _range = 5.0f;

    public bool active = true;
    
    protected InputAction _mouse;

    // Tell which one is in front of the others 
    protected int _element = -1;  

    protected bool _updateTile = true;
    protected Tile _selectedTile = null;
    protected Chunk _tileChunk = null;
    protected GameObject _tileHit = null;
    public Tile SelectedTile { get => _selectedTile; }

    protected bool _updateProp = true;
    protected Prop _selectedProp = null;
    protected Chunk _propChunk = null;
    protected GameObject _propHit = null;
    public Prop SelectedProp { get => _selectedProp; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        _mouse = InputSystem.actions.FindAction("Mouse");

        _playerMovement = player.GetComponent<PlayerMovement>();
    }

    public void SetActive(bool value)
    {
        active = value;
        if (!value)
        {
            RemoveLastSelectedProp();
            RemoveLastSelectedTile();
        }
    }

    protected void UpdateSelection()
    {
        SetSelectedTile();
        SetSelectedProp();

        if (_selectedTile == null && _selectedProp == null) _element = -1;
        else if (_selectedTile == null) _element = 1;
        else if (_selectedProp == null) _element = 0;
        else
        {
            int tileSO = _tileHit.GetComponent<SpriteRenderer>().sortingOrder;
            int propSO = _propHit.GetComponent<SpriteRenderer>().sortingOrder; 
            if (tileSO > propSO) _element = 0;
            else _element = 1;
        }
    }

    protected abstract void SelectTile();
    protected abstract void SelectProp();
    protected abstract void DeselectTile();
    protected abstract void DeselectProp();

    protected void SetSelectedTile()
    {
        Vector2 mousePosition = _mouse.ReadValue<Vector2>();
        Vector3 worldPosition = cam.ScreenToWorldPoint(mousePosition);

        RaycastHit2D[] hits = Physics2D.RaycastAll(worldPosition, Vector2.zero, float.MaxValue, LayerMask.GetMask("Tile"));
        RaycastHit2D hit = new RaycastHit2D();
        float y = float.MinValue;
        float z = float.MinValue;
        foreach(RaycastHit2D h in hits)
        {
            if (h.transform.position.z > z)
            {
                hit = h;
                y = h.transform.position.y;
                z = h.transform.position.z;
            }
            else if (h.transform.position.y > y) {
                hit = h;
                y = h.transform.position.y;
                z = h.transform.position.z;
            }
        }

        if(hits.Length != 0)
        {
            Vector3Int[] offsets =
            {
                new Vector3Int(0, 0, 0),
                new Vector3Int(1, 0, 0),
                new Vector3Int(0, 1, 0),
                new Vector3Int(1, 1, 0),
            };

            Vector3Int chunkPos = Coordinate.WorldToChunk(hit.transform.position);

            Tile tile = null;
            Chunk chunk = null;
            foreach (Vector3Int offset in offsets)
            {
                chunk = worldGenerator.GetChunk(chunkPos + offset);
                if (chunk != null)
                {
                    if ((tile = chunk.tilemap.GetTile(hit.transform.gameObject)) != null) break;
                    continue;
                }
            }
            if (tile == null || chunk.tilemap.GetTile(tile.position + new Vector3Int(0, 0, 1), false) != null)
            {
                RemoveLastSelectedTile();
                
            }
            else if (_selectedTile == null || tile.position !=  _selectedTile.position)
            {
                RemoveLastSelectedTile();
                if (_playerMovement && Vector3.Distance(_playerMovement.tilePosition, tile.position) <= _range)
                {
                    _selectedTile = tile;
                    _tileChunk = chunk;
                    _tileHit = hit.collider.gameObject;
                    _updateTile = false;
                }
            }
        }
        else
        {
            RemoveLastSelectedTile();
        } 
    }

    protected void SetSelectedProp()
    {
        Vector2 mousePosition = _mouse.ReadValue<Vector2>();
        Vector3 worldPosition = cam.ScreenToWorldPoint(mousePosition);
        
        RaycastHit2D[] hits = Physics2D.RaycastAll(worldPosition, Vector2.zero, float.MaxValue, LayerMask.GetMask("Prop"));
        RaycastHit2D hit = new RaycastHit2D();
        int sortingOrder = int.MinValue;
        foreach(RaycastHit2D h in hits)
        {
            SpriteRenderer sp = h.transform.GetComponent<SpriteRenderer>();
            if (sortingOrder < sp.sortingOrder)
            {
                sortingOrder = sp.sortingOrder;
                hit = h;
            }    
        }

        if(hits.Length != 0)
        {
            Vector3Int[] offsets =
            {
                new Vector3Int(0, 0, 0),
                new Vector3Int(1, 0, 0),
                new Vector3Int(0, 1, 0),
                new Vector3Int(1, 1, 0),
            };

            Transform parent = hit.transform.parent;
            Vector3Int chunkPos = Coordinate.WorldToChunk(parent.position);
            Prop prop = null;
            Chunk chunk = null;
            foreach (Vector3Int offset in offsets)
            {
                chunk = worldGenerator.GetChunk(chunkPos + offset);
                if (chunk != null)
                {
                    if ((prop = chunk.tilemap.GetProp(parent.gameObject)) != null) break;
                    continue;
                }
            }
            if (prop == null)
            {
                RemoveLastSelectedProp();
            }
            else if (_selectedProp == null || prop.position !=  _selectedProp.position)
            {
                RemoveLastSelectedProp();
                if (_playerMovement && Vector3.Distance(_playerMovement.tilePosition, prop.position) <= _range)
                {
                    _selectedProp = prop;
                    _propChunk = chunk;
                    _propHit = hit.collider.gameObject;
                    _updateProp = false;
                }
            }
        }
        else
        {
            RemoveLastSelectedProp();

        }
    }

    protected void RemoveLastSelectedTile()
    {
        if ( _selectedTile != null)
            DeselectTile();
            _selectedTile = null;
            _tileChunk = null;
            _tileHit = null;
            _updateTile = true;
    }

    protected void RemoveLastSelectedProp()
    {
        if ( _selectedProp != null)
            DeselectProp();
            _selectedProp = null;
            _propChunk = null;
            _propHit = null;
            _updateProp = true;
    }
}

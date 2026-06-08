using UnityEngine;
using UnityEngine.InputSystem;

using Roguelike.Tilemap.NTile;
using Roguelike.Tilemap.NProp;
using UnityEditor.U2D.Aseprite;

public class WorldInteractor : WorldSelector
{

    private InventoryManager _inventoryManager;

    public GameObject selector;
    
    protected InputAction _break;
    protected InputAction _place;

    private float _range = 2.0f;

    public override void Start()
    {
        base.Start();
        _break = InputSystem.actions.FindAction("Break");
        _place = InputSystem.actions.FindAction("Place");

        _inventoryManager = FindFirstObjectByType<InventoryManager>();
    }

    void Update()
    {
        if (!active) return;
        UpdateSelection();

        if (!_updateTile && _element == 0)
        {
            DeselectProp();
            SelectTile();
            _updateTile = true;
        }

        if (!_updateProp && _element == 1)
        {
            DeselectTile();
            SelectProp();
            _updateProp = true;
        }

        if (_break.triggered)
        {   
            if (_element == 0)
            {   
                if (!_tileChunk.tilemap.ContainObject(_selectedTile.position + new Vector3Int(0, 0, 1)))
                {
                    Loot[] loots = _selectedTile.LootTable.GetLoot();
                    if (_inventoryManager && loots != null)
                    {
                        foreach(Loot loot in loots)
                        {
                            _inventoryManager.AddItem(loot.item, loot.value);
                        }
                    }
                    _tileChunk.tilemap.RemoveTile(_selectedTile.position, false);
                    RemoveLastSelectedTile();
                }
            }
            if (_element == 1) {
                Loot[] loots = _selectedProp.LootTable.GetLoot();
                if (_inventoryManager && loots != null)
                {
                    foreach(Loot loot in loots)
                    {
                        _inventoryManager.AddItem(loot.item, loot.value);
                    }
                }
                _propChunk.tilemap.RemoveProp(_selectedProp.position, false);
                RemoveLastSelectedProp();
            }
        }

        if (_place.triggered)
        {
            if (_element == 0 && _inventoryManager)
            {
                if (!_tileChunk.tilemap.ContainObject(_selectedTile.position + new Vector3Int(0, 0, 1)))
                {
                    Item item = _inventoryManager.GetSelectedItem(false);
                    if (item != null)
                    {
                        Vector3Int elementPos = _selectedTile.position + new Vector3Int(0, 0, 1);
                        if (item.actionType == ActionType.Build)
                        {
                            if (item.tile != TileType.NONE)
                            {
                                Tile tile = Tile.GetTileFromType(item.tile, 0, elementPos);
                                _tileChunk.tilemap.SetTile(elementPos, tile, false);
                                _inventoryManager.GetSelectedItem(true);
                                RemoveLastSelectedTile();
                            }

                            else if (item.prop != PropType.NONE)
                            {
                                Prop prop = Prop.GetPropFromType(item.prop, elementPos);
                                _tileChunk.tilemap.SetProp(elementPos, prop, false);
                                _inventoryManager.GetSelectedItem(true);                      
                                RemoveLastSelectedTile();
                            }
                        }
                    }
                }
            }
        }
    }

    protected override void SelectTile()
    {
        if (_selectedTile == null) return;
        selector.SetActive(true);
        selector.transform.position = _selectedTile.gameObject.transform.position;
        selector.transform.position = selector.transform.position + new Vector3(0, 0, 1);
        selector.GetComponent<SpriteRenderer>().sortingOrder = _selectedTile.gameObject.GetComponent<SpriteRenderer>().sortingOrder;
    }
    
    protected override void SelectProp()
    {
        if (_selectedProp == null) return;
        _selectedProp.SetOutlineValue(true);
    }

    protected override void DeselectTile()
    {
        if (_selectedTile == null) return;
        selector.SetActive(false);
        _updateProp = false;
    }

    protected override void DeselectProp()
    {
        if (_selectedProp == null) return;
        _selectedProp.SetOutlineValue(false);
        _updateTile = false;
    }
}

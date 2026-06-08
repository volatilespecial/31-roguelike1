using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public int maxStackedItems = 4;
    public InventorySlot[] inventorySlots;
    public CraftSlot[] craftSlots;
    public GameObject inventoryItemPrefab;
    public GameObject inventoryGroup;
    int selectedSlot = -1;

    private InputAction _inventory;
    private InputAction _numKeys;


    void Awake()
    {
        _inventory = InputSystem.actions.FindAction("Inventory");
        _numKeys = InputSystem.actions.FindAction("NumKey");
    }

    void Start()
    {
        ChangeSelectedSlot(0);
        inventoryGroup = GameObject.FindWithTag("InventoryGroup");
        if (inventoryGroup)
        {
            inventoryGroup.gameObject.SetActive(false);
        }
    }

    void ChangeSelectedSlot(int newValue)
    {
        if (selectedSlot >= 0)
        {
            inventorySlots[selectedSlot].Deselect();
        }
        if (inventorySlots[newValue])
            inventorySlots[newValue].Select();
        selectedSlot = newValue;
    }

    void Update()
    {
        _numKeys.performed += context =>
        {
            bool isNumber = int.TryParse(context.control.name, out int number);
            if (isNumber && number > 0 && number < 10)
            {
                ChangeSelectedSlot(number - 1);
            }
        };
        if (_inventory.triggered && inventoryGroup)
        {
            inventoryGroup.SetActive(!inventoryGroup.activeSelf);
            WorldInteractor worldInteractor = FindFirstObjectByType<WorldInteractor>();
            if (worldInteractor)
            {
                worldInteractor.SetActive(!inventoryGroup.activeSelf);
            }
        }
    }
    public bool AddItem(Item item, int value)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null
                && itemInSlot.item == item
                && itemInSlot.count < maxStackedItems
                && itemInSlot.item.stackable)
            {
                int maxValue =  maxStackedItems - itemInSlot.count;
                int addedValue = maxValue < value ? maxValue : value;
                value -= addedValue;
                itemInSlot.count += addedValue;
                itemInSlot.RefreshCount();
                if(value <= 0) return true;
            }
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null)
            {

                int maxValue =  maxStackedItems;
                int addedValue = maxValue < value ? maxValue : value;
                value -= addedValue;
                SpawnNewItem(item, slot, addedValue);
                if(value <= 0) return true;
            }
        }
        return false;
    }

    void SpawnNewItem(Item item, InventorySlot slot, int count)
    {
        GameObject newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItemGo.GetComponent<InventoryItem>();
        inventoryItem.InitialiseItem(item, count);
    }

    public Item GetSelectedItem(bool use)
    {
        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        if (itemInSlot != null)
        {
            Item item = itemInSlot.item;
            if (use == true)
            {
                itemInSlot.count--;
                if (itemInSlot.count <= 0)
                {
                    Destroy(itemInSlot.gameObject);
                }
                else
                {
                    itemInSlot.RefreshCount();
                }
            }

            return item;
        }
        return null;
    }
}

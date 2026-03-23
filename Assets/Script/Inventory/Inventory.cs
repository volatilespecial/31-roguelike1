using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{

    public ItemSO woodItem;
    public ItemSO axeItem;
    public GameObject inventorySlotParent;

    private List<Slot> inventorySlots = new List<Slot>();
    private List<Slot> allSlots = new List<Slot>();

    private void Awake()
    {
        inventorySlots.AddRange(inventorySlotParent.GetComponentsInChildren<Slot>());

        allSlots.AddRange(inventorySlots);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            AddItem(woodItem, 3);
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            AddItem(axeItem, 1);
        }
    }

    public void AddItem(ItemSO itemToAdd, int amount)
    {
        int remaining = amount;

        foreach (Slot slot in allSlots)
        {
            if (slot.HasItem() && slot.GetItem() == itemToAdd)
            {
                int currentAmount = slot.GetAmount();
                int maxStack = itemToAdd.maxStackSize;

                if (currentAmount < maxStack)
                {
                    int spaceLeft = maxStack - currentAmount;
                    int amountToAdd = Mathf.Min(spaceLeft, remaining);

                    slot.SetItem(itemToAdd, currentAmount + amountToAdd);
                    remaining -= amountToAdd;

                    if (remaining <= 0) return;
                }
            }
        }

        foreach (Slot slot in allSlots)
        {
            if (!slot.HasItem())
            {
                int amountToPlace = Mathf.Min(itemToAdd.maxStackSize, remaining);
                slot.SetItem(itemToAdd, amountToPlace);
                remaining -= amountToPlace;

                if (remaining <= 0) return;
            }
        }

        if (remaining > 0)
        {
            Debug.Log("Inventory is full. Could not add " + remaining + " of " + itemToAdd.itemName);
        }
    }
}

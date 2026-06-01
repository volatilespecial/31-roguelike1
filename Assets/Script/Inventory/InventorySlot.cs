using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public Image image;
    public Color selectedColor, notSelectedColor;

    private void Awake()
    {
        if (image == null)
        {
            image = GetComponent<Image>();
        }
        Deselect();
    }

    public void Select()
    {
        image.color = selectedColor;
    }

    public void Deselect()
    {    
        image.color = notSelectedColor;
    }
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        InventoryItem inventoryItem = dropped.GetComponent<InventoryItem>();

        if (transform.childCount != 0)
        {
            GameObject current = transform.GetChild(0).gameObject;
            InventoryItem currentItem = current.GetComponent<InventoryItem>();

            if (currentItem.item == inventoryItem.item && currentItem.item.stackable)
            {
                InventoryManager manager = FindFirstObjectByType<InventoryManager>();
                if (currentItem.count < manager.maxStackedItems)
                {
                    int roomLeft = manager.maxStackedItems - currentItem.count;
                    if (inventoryItem.count <= roomLeft)
                    {
                        currentItem.count += inventoryItem.count;
                        currentItem.RefreshCount();
                        Destroy(inventoryItem.gameObject);
                        return;
                    }
                    else
                    {
                        currentItem.count = manager.maxStackedItems;
                        inventoryItem.count -= roomLeft;
                        currentItem.RefreshCount();
                        inventoryItem.RefreshCount();
                        return;
                    }
                }
            }


            currentItem.transform.SetParent(inventoryItem.parentAfterDrag);
        }
        inventoryItem.parentAfterDrag = transform;

    }
}

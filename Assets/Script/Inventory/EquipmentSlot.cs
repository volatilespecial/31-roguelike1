using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipmentSlot : MonoBehaviour, IDropHandler
{
    public EquipmentType allowedType;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        InventoryItem inventoryItem = dropped.GetComponent<InventoryItem>();


        if (inventoryItem == null) return;

        if (inventoryItem.item.equipmentType != allowedType)
        {
            Debug.Log("Wrong type of equipment");
            return;
        }

        if (transform.childCount != 0)
        {
            GameObject current = transform.GetChild(0).gameObject;
            InventoryItem currentItem = current.GetComponent<InventoryItem>();

            currentItem.transform.SetParent(inventoryItem.parentAfterDrag);
        }

        inventoryItem.parentAfterDrag = transform;

    }
}

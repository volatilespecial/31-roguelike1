using UnityEngine;
using UnityEngine.UI;

public class EquipmentManager : MonoBehaviour
{

    public EquipmentSlot[] equipmentSlots;
    public Image[] backgrounds;
    private Item[] lastState = new Item[6];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            EquipmentSlot equipmentSlot = equipmentSlots[i];
            if (equipmentSlot == null)
            {
                Debug.Log("EquipmentSlot not assigned");
            }
            
            InventoryItem itemInSlot = equipmentSlot.GetComponentInChildren<InventoryItem>();


            if (itemInSlot == null)
            {
                if (backgrounds[i] != null)
                {
                    backgrounds[i].gameObject.SetActive(false);
                }
            }
            else
            {
                if (backgrounds[i] != null)
                {
                    backgrounds[i].gameObject.SetActive(true);
                }
            }
        }
        for (int i = 0; i < lastState.Length; i++)
        {
            lastState[i] = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (EquipmentHasChanged())
        {
            EquipmentUpdate();
        }
    }

    private bool EquipmentHasChanged()
    {
        bool isChanged = false;

        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (equipmentSlots[i] == null) continue;

            InventoryItem itemInSlot = equipmentSlots[i].GetComponentInChildren<InventoryItem>();
            Item currentItem = (itemInSlot != null) ? itemInSlot.item : null;

            if (currentItem != lastState[i])
            {
                isChanged = true;
                lastState[i] = currentItem;
                if (currentItem)
                {
                    if (backgrounds[i] != null)
                    {
                        backgrounds[i].gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (backgrounds[i] != null)
                    {
                        backgrounds[i].gameObject.SetActive(false);
                    }
                }
            }
        }
        return isChanged;
    }

    private void EquipmentUpdate()
    {
        Debug.Log("Updated Equipment");
    }
}

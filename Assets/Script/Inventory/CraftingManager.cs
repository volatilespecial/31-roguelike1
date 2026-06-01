using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public CraftSlot[] craftSlots;
    public List<CraftingRecipe> recipes;

    public CraftResult resultSlot;
    public GameObject inventoryItemPrefab;

    private Item[] lastState = new Item[4];
    private int[] lastCounts = new int[4];

    public void Start()
    {
        for (int i = 0; i < lastState.Length; i++)
        {
            lastState[i] = null;
            lastCounts[i] = 0;
        }
    }

    public void Update()
    {
        if (StateHasChanged())
        {
            CheckRecipes();
        }
        // InventoryItem atLeastOne = craftSlots[0].GetComponentInChildren<InventoryItem>();

        // if (atLeastOne != null)
        // {
        //     CheckRecipes();
        // }
    }

    private bool StateHasChanged()
    {
        bool isChanged = false;
        for (int i = 0; i < 4; i++)
        {
            if (craftSlots[i] == null) continue;
            InventoryItem itemInSlot = craftSlots[i].GetComponentInChildren<InventoryItem>();
            Item currentItem = (itemInSlot != null) ? itemInSlot.item : null;
            int currentCount = (itemInSlot != null) ? itemInSlot.count : 0;

            if (currentItem != lastState[i])
            {
                isChanged = true;
                lastState[i] = currentItem;
                lastCounts[i] = currentCount;
            }
        }
        return isChanged;
    }

    public void CheckRecipes()
    {
        if (craftSlots == null || craftSlots.Length < 4)
        {
            Debug.Log("not 4 craft slots in the inspector");
            return;
        }

        Item[] currentGrid = new Item[4];
        for (int i = 0; i < 4; i++)
        {
            if (craftSlots[i] == null)
            {
                Debug.Log("Craft Slot null");
                return;
            }

            InventoryItem itemInSlot = craftSlots[i].GetComponentInChildren<InventoryItem>();
            currentGrid[i] = (itemInSlot != null) ? itemInSlot.item : null;
        }

        if (recipes == null)
        {
            Debug.Log("Recipes list null");
            return;
        }

        foreach (CraftingRecipe recipe in recipes)
        {
            if (recipe != null && recipe.Matches(currentGrid))
            {
                DisplayResult(recipe);
                Debug.Log("Recipe found");
                return;
            }
        }

        ClearResult();
    }

    private void DisplayResult(CraftingRecipe recipe)
    {
        ClearResult();

        GameObject newResultGo = Instantiate(inventoryItemPrefab, resultSlot.transform);
        InventoryItem resultItem = newResultGo.GetComponent<InventoryItem>();
        resultItem.InitialiseItem(recipe.resultItem);
        resultItem.count = recipe.resultCount;
        resultItem.RefreshCount();
    }

    private void ClearResult()
    {
        if (resultSlot.transform.childCount > 0)
        {
            Destroy(resultSlot.transform.GetChild(0).gameObject);
        }
    }

    public void TryCraftItem()
    {
        InventoryItem resultItem = resultSlot.GetComponentInChildren<InventoryItem>();
        if (resultItem == null) return;

        InventoryManager inventoryManager = FindFirstObjectByType<InventoryManager>();
        if (inventoryManager == null) return;

        int itemsToCraft = resultItem.count;
        bool successfullyAddedAny = false;

        for (int i = 0; i < itemsToCraft; i++)
        {
            if (inventoryManager.AddItem(resultItem.item))
            {
                successfullyAddedAny = true;
            }
            else
            {
                if (i == 0)
                {
                    Debug.LogWarning("Inventory full");
                    return;
                }
                break;
            }
        }

        if (successfullyAddedAny)
        {
            ConsumeIngredients();
        }
    }

    private void ConsumeIngredients()
    {
        for (int i = 0; i < 4; i++)
        {
            if (craftSlots[i] == null) continue;

            InventoryItem itemInSlot = craftSlots[i].GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null)
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
        }
    }
}



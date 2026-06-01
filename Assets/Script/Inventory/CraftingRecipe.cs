using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Inventory/Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public Item[] ingredients = new Item[4];

    public Item resultItem;
    public int resultCount = 1;

    public bool Matches(Item[] craftedItems)
    {
        for (int i = 0; i < 4; i++)
        {
            if (craftedItems[i] != ingredients[i])
            {
                return false;
            }
        }
        return true;
    }
}

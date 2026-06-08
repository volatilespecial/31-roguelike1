using System;
using UnityEngine;

[Serializable]
public struct LootData
{
    public Item item;
    public int min;
    public int max;
}

public class Loot
{
    public Item item;
    public int value;

    public Loot(Item item, int value)
    {
        this.item = item;
        this.value = value;
    }
}

[Serializable]
public class LootTable
{
    [SerializeField]
    private LootData[] loots;

    public Loot[] GetLoot()
    {
        if (loots == null || loots.Length == 0) return null;
        Loot[] ret = new Loot[loots.Length];
        for (int i = 0; i < loots.Length; ++i)
        {
            float rand = UnityEngine.Random.Range(0.0f, 1.0f);
            LootData lootData = loots[i];
            ret[i] = new Loot(lootData.item, Mathf.RoundToInt(Mathf.Lerp(lootData.min, lootData.max, rand)));
        }
        return ret;
    }    
}

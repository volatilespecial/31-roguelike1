using UnityEngine;

[CreateAssetMenu(fileName="Item", menuName="NewItem")]

public class ItemSO : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public string itemDescription;
    public int maxStackSize;
    public GameObject itemPrefab;
    public GameObject handItemPrefab;
}

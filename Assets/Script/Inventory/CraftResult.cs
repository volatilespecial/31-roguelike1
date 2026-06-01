using UnityEngine;
using UnityEngine.EventSystems;

public class CraftResult : MonoBehaviour, IPointerClickHandler
{
    private CraftingManager craftingManager;

    private void Awake()
    {
        craftingManager = FindFirstObjectByType<CraftingManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (transform.childCount > 0)
            {
                craftingManager.TryCraftItem();
            }
        }
    }
}
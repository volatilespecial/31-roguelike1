using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI")]
    public Image image;
    public Text countText;
    [HideInInspector] public Item item;
    [HideInInspector] public int count = 1;
    [HideInInspector] public Transform parentAfterDrag;

    // private void Start()
    // {
    //     InitialiseItem(item);
    // }

    public void InitialiseItem(Item newItem)
    {
        InitialiseItem(newItem, 1);
    }

    public void InitialiseItem(Item newItem, int count)
    {
        item = newItem;
        this.count = count;
        if (image == null)
        {
            image = GetComponent<Image>();
        }
        if (newItem.image)
        {
            Debug.Log("Has image");
        }
        if (image)
        {
            Debug.Log("Image good");
        }
        image.sprite = newItem.image;
        RefreshCount();

    }

    public void RefreshCount()
    {
        countText.text = count.ToString();
        bool textActive = count > 1;
        countText.gameObject.SetActive(textActive);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Debug.Log(transform.parent.name);
        if (GetComponentInParent<CraftResult>() != null) return;
        // Debug.Log("Begin drag");
        image.raycastTarget = false;
        if (countText != null) countText.raycastTarget = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData) {
        if (GetComponentInParent<CraftResult>() != null) return;
        // Debug.Log("Dagging");
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (GetComponentInParent<CraftResult>() != null) return;
        // Debug.Log("End Drag");
        image.raycastTarget = true;
        if (countText != null) countText.raycastTarget = true;
        transform.SetParent(parentAfterDrag);
    }
}

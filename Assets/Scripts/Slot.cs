using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    private Color32 EmptySlotColor = new Color32(137, 127, 107, 255);
    private Color32 RedSlotColor = new Color32(126, 35, 27, 255);
    private Color32 GreenSlotColor = new Color32(49, 156, 90, 255);

    [SerializeField] private Image SlotBackground;

    public Vector2Int slotCoordinate;

    private SlotColor slotColor;

    public event EventHandler slotClicked;
    public event EventHandler slotSelected;

    public void Initialize(Vector2Int slotCoordinate)
    {
        this.slotCoordinate = slotCoordinate;
        this.slotColor = SlotColor.Empty;
    }

    public void SetSlotColor(SlotColor playerColor)
    {
        switch (playerColor)
        {
            case SlotColor.Empty:
                SlotBackground.color = EmptySlotColor;
                SlotBackground.gameObject.SetActive(false);
                slotColor = SlotColor.Empty;
                break;

            case SlotColor.Red:
                SlotBackground.color = RedSlotColor;
                SlotBackground.gameObject.SetActive(true);
                slotColor = SlotColor.Red;
                break;

            case SlotColor.Green:
                SlotBackground.color = GreenSlotColor;
                SlotBackground.gameObject.SetActive(true);
                slotColor = SlotColor.Green;
                break;

            default:
                break;
        }
    }

    public SlotColor GetSlotColor()
    {
        return slotColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        slotSelected?.Invoke(this, new EventArgs());
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        slotClicked?.Invoke(this, new EventArgs());
    }
}

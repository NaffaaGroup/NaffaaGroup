using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject cardScaleOnTable;
    [SerializeField] private GameObject cardScaleOnHand;
    
    public enum Slot { DISCHARGE, MELDS, HAND };
    public Slot typeOfSlot;

    public void OnPointerEnter(PointerEventData eventData)
    {
        //will be used to highlight the drop zone if it's a valid drop or not 
        if (eventData.pointerDrag != null)
        {
            Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
            Debug.Log(name + " ENTER");

            if (HandManager.instance.players[HandManager.instance.mainPlayerID].playerTurn)
            {
                d.placeholderParent = transform;

                if (gameObject.GetComponent<DropZone>().typeOfSlot != Slot.HAND)
                {
                    d.placeholder.GetComponent<RectTransform>().localScale = cardScaleOnTable.GetComponent<RectTransform>().localScale;
                    d.GetComponent<RectTransform>().anchoredPosition = cardScaleOnTable.GetComponent<RectTransform>().anchoredPosition;
                    // Debug.Log("FUCK YOUUU");
                }
                else
                {
                    d.placeholder.GetComponent<RectTransform>().localScale = cardScaleOnHand.GetComponent<RectTransform>().localScale;
                    d.placeholder.GetComponent<LayoutElement>().preferredWidth = 150f;
                    d.placeholder.GetComponent<LayoutElement>().preferredHeight = cardScaleOnHand.GetComponent<LayoutElement>().preferredHeight;
                    d.GetComponent<LayoutElement>().preferredWidth = cardScaleOnHand.GetComponent<LayoutElement>().preferredWidth;
                    d.GetComponent<RectTransform>().anchoredPosition = cardScaleOnHand.GetComponent<RectTransform>().anchoredPosition;
                    d.GetComponent<RectTransform>().anchorMin = cardScaleOnHand.GetComponent<RectTransform>().anchorMin;
                    d.GetComponent<RectTransform>().anchorMax = cardScaleOnHand.GetComponent<RectTransform>().anchorMax;
                    d.GetComponent<RectTransform>().sizeDelta = cardScaleOnHand.GetComponent<RectTransform>().sizeDelta;
                    Debug.Log(d.placeholderParent + " WHEREEEE");
                }
            }         
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //will be used to turn off the highlights of the drop zone
        if (eventData.pointerDrag != null)
        {
            
            Draggable d = eventData.pointerDrag.GetComponent<Draggable>();

            Debug.Log(gameObject.name + " EXIT");
            //d.parentToReturnTo = null;
        }
    }

}

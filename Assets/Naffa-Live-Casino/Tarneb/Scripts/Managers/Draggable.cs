using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public Transform placeholderParent { get; set; }
    public GameObject placeholder { get; set; }
    public int siblingIndex { get; set; }
    public int placeholderPreferredHeight { get; set; }
    public int placeholderPreferredWidth { get; set; }

    [SerializeField] private GameObject cardScaleOnTable;
    [SerializeField] private GameObject cardScaleOnHand;

    private Transform playerHandUIHolder;

    private GameObject dischargePile;
    private GameObject playerHand;
    public bool CanDrag { get; set; }
    public bool ReadyToDrag;

    private int clicks;
    private float clickTime = 0f;
    private float clickDelay = 1f;

    private void Start()
    {
        dischargePile = GameObject.FindGameObjectWithTag("DischargePile");
        playerHandUIHolder = GameObject.FindGameObjectWithTag("PlayerHand").transform;

        for (int i = 0; i < playerHandUIHolder.childCount; i++)
            if (playerHandUIHolder.GetChild(i).CompareTag("MainPlayer"))
                playerHand = playerHandUIHolder.GetChild(i).gameObject;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (CanDrag)
        {

            Debug.Log("ON BEGIN DRAG");
            placeholderParent = transform.parent;
            siblingIndex = transform.GetSiblingIndex();
            Debug.Log(siblingIndex + " SIBLING INDEX");
            placeholder = new GameObject();

            placeholder.AddComponent<LayoutElement>();

            placeholder.GetComponent<RectTransform>().localScale = cardScaleOnHand.GetComponent<RectTransform>().localScale;
            placeholder.GetComponent<LayoutElement>().preferredWidth = 150f;
            placeholder.GetComponent<LayoutElement>().preferredHeight = cardScaleOnHand.GetComponent<LayoutElement>().preferredHeight;
            GetComponent<LayoutElement>().preferredWidth = cardScaleOnHand.GetComponent<LayoutElement>().preferredWidth;
            GetComponent<RectTransform>().anchoredPosition = cardScaleOnHand.GetComponent<RectTransform>().anchoredPosition;
            GetComponent<RectTransform>().anchorMin = cardScaleOnHand.GetComponent<RectTransform>().anchorMin;
            GetComponent<RectTransform>().anchorMax = cardScaleOnHand.GetComponent<RectTransform>().anchorMax;
            GetComponent<RectTransform>().sizeDelta = new Vector2(88, 110);
            transform.rotation = Quaternion.identity;
            ReadyToDrag = false;

            placeholder.transform.SetParent(placeholderParent);
            placeholder.transform.SetSiblingIndex(siblingIndex);

            transform.SetParent(transform.root);
            GetComponent<CanvasGroup>().blocksRaycasts = false;

        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!transform.GetComponent<CardInfo>().player.playerTurn){
            CanDrag=false;
        }
        if(ReadyToDrag==false){
            transform.rotation = Quaternion.identity;
        }
        
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (CanDrag)
        {
            Debug.Log("DRAG");
            transform.position = eventData.position;

            if (placeholderParent != null)
            {
                for (int i = 0; i < placeholderParent.childCount; i++)
                {
                    if (transform.position.x < placeholderParent.GetChild(i).position.x)
                    {
                        placeholder.transform.SetParent(placeholderParent);
                        placeholder.transform.SetSiblingIndex(i);
                        siblingIndex = placeholder.transform.GetSiblingIndex();
                        break;
                    }
                }
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //behavior of dropping the card in a drop zone
        if (CanDrag)
        {
            Debug.Log("ON END DRAG");

            transform.SetParent(placeholderParent);
            if (placeholderParent.GetComponent<DropZone>().typeOfSlot == DropZone.Slot.DISCHARGE)
            {
                transform.SetAsLastSibling();
                HandManager.instance.players[HandManager.instance.mainPlayerID].GetComponent<TimerController>().EndPlayerTurn();
            }
            else
            {
                transform.SetSiblingIndex(siblingIndex);
            }

            Debug.Log(siblingIndex + " SIBLING INDEX");
            transform.GetComponent<RectTransform>().localRotation = Quaternion.identity;
            GetComponent<CanvasGroup>().blocksRaycasts = true;

            if (placeholderParent.GetComponent<DropZone>().typeOfSlot == DropZone.Slot.MELDS)
            {
                HandManager.instance.players[HandManager.instance.mainPlayerID].GetComponent<TimerController>().EndPlayerTurn();
                //set parent transform to this transform
                transform.SetParent(HandManager.instance.cardPos[0].transform);
                transform.localPosition = Vector3.zero;
                transform.gameObject.transform.localScale = Vector3.one;
                transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.5f, 0.5f);
                transform.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
                transform.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
                GetComponent<RectTransform>().sizeDelta = new Vector2(88, 110);

                Debug.Log("MELDING");
                Destroy(transform.GetComponent<Draggable>());
                HandManager.instance.GetComponent<RoundController>().playerPlayed++;




                if (HandManager.instance.ReadyForHardMode)
                {
                    if (transform.GetComponent<CardInfo>().cardInfo.cardType.ToString() == RoundController.instance.GetTypeName(RoundController.instance.TypeNumber))
                    {
                        transform.GetComponent<AudioSource>().Play();
                        HandManager.instance.ReadyForHardMode = false;

                    }
                    else
                    {

                        HandManager.instance.ReadyForHardMode = false;
                        SoundSystem.Instance.GetComponent<AudioSource>().clip = SoundSystem.Instance.TableHard;
                        SoundSystem.Instance.GetComponent<AudioSource>().Play();
                    }
                }
                else
                {
                    transform.GetComponent<AudioSource>().Play();
                }
            }

            Destroy(placeholder);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (true)
        {
            clicks++;


            if (clicks >= 1)
            {

                //rotate 30 degrees
                //loop through all the cards in the hand
                if (ReadyToDrag)
                {
                    foreach (Transform child in transform.GetComponent<CardInfo>().player.cardsHolderUI.transform)
                    {
                        child.rotation = Quaternion.identity;
                        child.GetComponent<Draggable>().ReadyToDrag = false;
                    }
                    ReadyToDrag = false;

                }
                else
                {
                    foreach (Transform child in transform.GetComponent<CardInfo>().player.cardsHolderUI.transform)
                    {
                        child.rotation = Quaternion.identity;
                        child.GetComponent<Draggable>().ReadyToDrag = false;
                    }
                    transform.rotation = Quaternion.Euler(0, 0, -15);
                    ReadyToDrag = true;

                }
            }
            if (clicks == 1)
            {
                clickTime = Time.time;
            }

            if (clicks > 1 && Time.time - clickTime < clickDelay && !transform.parent.CompareTag("Deck") && CanDrag)
            {
                // Double click detected
                clicks = 0;
                clickTime = 0;
                Debug.Log("Double Click");
                HandManager.instance.players[HandManager.instance.mainPlayerID].GetComponent<TimerController>().EndPlayerTurn();
                //set parent transform to this transform
                transform.SetParent(HandManager.instance.cardPos[0].transform);
                transform.localPosition = Vector3.zero;
                transform.localPosition = Vector3.zero;
                transform.gameObject.transform.localScale = Vector3.one;
                transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.5f, 0.5f);
                transform.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
                transform.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
                GetComponent<RectTransform>().sizeDelta = new Vector2(88, 110);
                Debug.Log("MELDING");
                Destroy(transform.GetComponent<Draggable>());
                HandManager.instance.GetComponent<RoundController>().playerPlayed++;
                ReadyToDrag = false;

                //rsest rotation
                transform.rotation = Quaternion.identity;

                if (HandManager.instance.ReadyForHardMode)
                {
                    if (transform.GetComponent<CardInfo>().cardInfo.cardType.ToString() == RoundController.instance.GetTypeName(RoundController.instance.TypeNumber))
                    {
                        transform.GetComponent<AudioSource>().Play();
                        HandManager.instance.ReadyForHardMode = false;

                    }
                    else
                    {

                        HandManager.instance.ReadyForHardMode = false;
                        SoundSystem.Instance.GetComponent<AudioSource>().clip = SoundSystem.Instance.TableHard;
                        SoundSystem.Instance.GetComponent<AudioSource>().Play();
                    }
                }
                else
                {
                    transform.GetComponent<AudioSource>().Play();
                }

            }
            else if (clicks >= 2 || Time.time - clickTime > 0.3f)
                clicks = 0;
        }

    }

    public void ThrowToDischarge()
    {
        if (HandManager.instance.players[HandManager.instance.mainPlayerID].drewCard)
        {
            transform.SetParent(dischargePile.transform);
            transform.SetAsLastSibling();
            GetComponent<RectTransform>().anchoredPosition = cardScaleOnTable.GetComponent<RectTransform>().anchoredPosition;
            GetComponent<RectTransform>().anchorMin = cardScaleOnTable.GetComponent<RectTransform>().anchorMin;
            GetComponent<RectTransform>().anchorMax = cardScaleOnTable.GetComponent<RectTransform>().anchorMax;
            HandManager.instance.players[HandManager.instance.mainPlayerID].GetComponent<TimerController>().EndPlayerTurn();
        }
    }

    public void TakeCardFromDischarge()
    {   //conditions will be involved with this
        Debug.Log("TAKE from discharge");
        transform.SetParent(playerHand.transform);
        transform.SetAsLastSibling();
        GetComponent<RectTransform>().anchoredPosition = cardScaleOnHand.GetComponent<RectTransform>().anchoredPosition;
        GetComponent<RectTransform>().anchorMin = cardScaleOnHand.GetComponent<RectTransform>().anchorMin;
        GetComponent<RectTransform>().anchorMax = cardScaleOnHand.GetComponent<RectTransform>().anchorMax;
    }

    public void DropInMelds(Transform meld)
    {
        for (int i = 0; i < meld.childCount; i++)
        {
            // if (HandManager.instance.players[HandManager.instance.mainPlayerID].melds[meld.parent.GetSiblingIndex()][i].cardNumber
        }

    }
}

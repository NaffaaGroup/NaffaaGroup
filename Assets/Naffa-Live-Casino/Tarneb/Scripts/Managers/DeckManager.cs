using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckManager : MonoBehaviour
{
    [SerializeField] private GameObject cardScaleOnHand;
    public void TakeCardFromDeck(Transform playerHolder)
    {
        if (HandManager.instance.players[HandManager.instance.mainPlayerID].playerTurn && !HandManager.instance.players[HandManager.instance.mainPlayerID].drewCard)
        {
            if (HandManager.instance.deck.Count == 0)
            {
                HandManager.instance.deck.Clear();
                HandManager.instance.GenerateDeck();
                HandManager.instance.ShuffleDeck();
                Debug.Log("REGENRATE DECK");
            }

            Debug.Log("TAKE from deck");
            GameObject card = Instantiate(cardScaleOnHand, playerHolder.transform);
            card.GetComponent<Image>().sprite = HandManager.instance.deck[HandManager.instance.deck.Count - 1].cardImage;
            card.tag = HandManager.instance.deck[HandManager.instance.deck.Count - 1].ID;
            HandManager.instance.deck.RemoveAt(HandManager.instance.deck.Count - 1);
            HandManager.instance.players[HandManager.instance.mainPlayerID].drewCard = true;
        }
    }
}

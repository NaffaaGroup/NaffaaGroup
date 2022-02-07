

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerHandler : MonoBehaviour
{
    [SerializeField] private GameObject cardHandUI;
    [SerializeField] private GameObject cardBackUI;
    [SerializeField] private GameObject cardTableUI;
    [SerializeField] private GameObject meldSetUI;
    [SerializeField] private GameObject meldGroupsUI;
    [SerializeField] public int playerNumber;
    [SerializeField] private Text scoreTxt;
    [SerializeField] public GameObject cardsHolderUI;
    [SerializeField] private TextMeshProUGUI meldSumText;
    [SerializeField] private Button meldSumGoDown;
    [SerializeField] public Text PlayerOrder;
    public GameObject playerUI;
    int currentScore = 0;
    public int orderNumber = 0;
    public bool isOrder = false;
    public List<Card> cards { get; set; }
    public List<List<Card>> melds { get; set; }
    public bool playerTurn;
    public bool turnStart;
    public bool drewCard { get; set; }
    public bool goDownWithMelds { get; set; }
    public int sum { get; set; }
    public int score;
    public Sprite playerImage;
    public float timer { get; set; }
    public string playerTeam;
    public string playerName;
    public Text playerNameText;
    public enum PlayerOrderIndex { first, second, third, fourth };
    public PlayerOrderIndex playerOrderIndex;
    public enum PlayerType { Bot, Human };
    public PlayerType playerType;
    public int LoopTimes = 0;
    [SerializeField] public Image PlayerImageMesh;
    private void Awake()
    {
        cards = new List<Card>();
        melds = new List<List<Card>>();


    }
    private void Start()
    {
        Debug.Log(playerTurn + " " + playerNumber);

        CheckForMelds();

        if (gameObject.tag == "MainPlayer")
        {
            HandManager.instance.mainPlayerID = playerNumber;
            // HighlightMelds();
            // CalculateMeldSum();

        }
        if (playerType.ToString() == "Bot")
        {
            BotData data = NamesSystem.Instance.GetRandomData();
            playerName = data.name;
            this.PlayerImageMesh.sprite =data.image;
            this.playerImage = data.image;
            this.playerNameText.text = playerName;
        }
        else
        {
            this.playerNameText.text = playerName;
            this.PlayerImageMesh.sprite = playerImage;

        }

    }

    private void Update()
    {
        PlayerOrder = TypesAssets.Instance.getScoreObject(playerNumber);
        scoreTxt = CurrentOrder.Instance.TopScores[playerNumber];
        if (RoundController.instance.CheckCurrentPlayerTeam(this) == "Team1")
        {

            this.scoreTxt.text = RoundController.instance.Team1Score.ToString();
            ResultPanel.instance.Team1Score.text = RoundController.instance.Team1Score.ToString();
        }

        else
        {
            this.scoreTxt.text = RoundController.instance.Team2Score.ToString();
            ResultPanel.instance.Team2Score.text = RoundController.instance.Team2Score.ToString();
        }
        CurrentOrder.Instance.SetPlayerScore(score, playerNumber);
        RefreshCards();
        // CheckForMelds();

        if (gameObject.tag == "MainPlayer")
        {
            // HighlightMelds();
            // CalculateMeldSum();
        }
        if (playerType.ToString() != "Bot")
        {
            this.playerNameText.text = playerName;
        }

    }
    public void SetPlayerUI()
    {
        this.playerNameText.text = playerName;
        this.PlayerImageMesh.sprite = playerImage;

    }
    public void CreateCardsUI()
    {
        SortCardsAtHand();

        if (gameObject.tag.CompareTo("MainPlayer") == 0)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                //Debug.Log(cards[i].cardNumber + " " + cards[i].cardType + " " + playerNumber);
                GameObject card = Instantiate(cardHandUI, cardsHolderUI.transform);
                card.tag = cards[i].ID;
                card.GetComponent<Image>().sprite = cards[i].cardImage;
                card.AddComponent<CardInfo>().cardInfo = cards[i];
                card.GetComponent<CardInfo>().player = this;
            }
        }
        else
        {
            for (int i = 0; i < cards.Count; i++)
            {
                GameObject card = Instantiate(cardBackUI, cardsHolderUI.transform);
                //Debug.Log(cards[i].cardNumber + " " + cards[i].cardType + " " + playerNumber);
                card.tag = cards[i].ID;
                card.AddComponent<CardInfo>().cardInfo = cards[i];
                card.GetComponent<CardInfo>().player = this;

            }
        }
    }

    public void RefreshCards()
    {
        cards.Clear();

        for (int i = 0; i < cardsHolderUI.transform.childCount; i++)
        {
            for (int j = 0; j < HandManager.instance.cards.Length; j++)
            {
                if (cardsHolderUI.transform.GetChild(i).tag == HandManager.instance.cards[j].ID)
                {
                    cards.Add(HandManager.instance.cards[j]);
                    // Debug.Log(cards[i].cardNumber + " " + cards[i].cardType);
                }
            }
        }
    }


    public void SortCardsAtHand()
    {
        cards.Sort(delegate (Card a, Card b) { return a.cardNumber.CompareTo(b.cardNumber); });
        cards.Sort(delegate (Card a, Card b) { return a.orderType.CompareTo(b.orderType); });
    }

    public void CheckForMelds()
    {
        melds.Clear();

        List<Card> potentialMelds = new List<Card>();
        List<Card> potentialMelds1 = new List<Card>();

        for (int i = 0; i < cards.Count; i++)
        {
            Card beforeC = null;
            Card afterC = null;

            Card card = cards[i];

            if (gameObject.tag == "MainPlayer")
                //Debug.Log(card.cardNumber + " CARD");
                potentialMelds.Add(card);
            potentialMelds1.Add(card);


            for (int j = 1; j < cards.Count; j++)
            {
                if (i + j < cards.Count)
                    afterC = cards[i + j];

                if (afterC != null)
                {
                    if (gameObject.tag == "MainPlayer")
                        //Debug.Log(afterC.cardNumber + " AFTER C");
                        if (!potentialMelds.Contains(afterC) && card.cardNumber + j == afterC.cardNumber && card.cardType.Equals(afterC.cardType))
                        {
                            potentialMelds.Add(afterC);
                        }
                        else if (!potentialMelds1.Contains(afterC) && card.cardNumber == afterC.cardNumber && !card.cardType.Equals(afterC.cardType))
                        {
                            potentialMelds1.Add(afterC);
                        }
                        else
                        {
                            break;
                        }
                }
            }

            for (int j = 1; j < cards.Count; j++)
            {
                if (i - j >= 0)
                    beforeC = cards[i - j];

                if (beforeC != null)
                {
                    if (gameObject.tag == "MainPlayer")
                        //Debug.Log(beforeC.cardNumber + " BEFORE C");
                        if (!potentialMelds.Contains(beforeC) && card.cardNumber - j == beforeC.cardNumber && card.cardType.Equals(beforeC.cardType))
                        {
                            potentialMelds.Add(beforeC);
                        }
                        else if (!potentialMelds1.Contains(beforeC) && card.cardNumber == beforeC.cardNumber && !card.cardType.Equals(beforeC.cardType))
                        {
                            potentialMelds1.Add(beforeC);
                        }
                        else
                        {
                            break;
                        }
                }
            }

            //Debug.Log(potentialMelds1.Count + " COU*UUUUUUUUUUUNR");

            for (int k = 0; k < potentialMelds1.Count; k++)
                //Debug.Log(potentialMelds1[k] + " COU");


                if (!IsAdded(potentialMelds) && potentialMelds.Count > 2)
                {
                    Debug.Log("ADD MELDS");

                    List<Card> temp = new List<Card>(potentialMelds);
                    melds.Add(temp);
                }

            if (!IsAdded(potentialMelds1) && potentialMelds1.Count > 2)
            {
                Debug.Log("ADD MELDS 1");
                List<Card> temp = new List<Card>(potentialMelds1);
                melds.Add(temp);
            }

            Debug.Log(melds.Count + " MELDS & PLAYER NUMBER " + playerNumber);
            potentialMelds.Clear();
            potentialMelds1.Clear();

        }
        if (gameObject.tag == "MainPlayer")
        {
            for (int i = 0; i < melds.Count; i++)
            {
                for (int j = 0; j < melds[i].Count; j++)
                {
                    Debug.Log(melds[i][j] + " MELD SET");
                }
                Debug.Log(" ");
            }
        }

    }

    private bool IsAdded(List<Card> potential)
    {
        bool added = false;

        for (int i = 0; i < melds.Count; i++)
        {
            for (int j = 0; j < potential.Count; j++)
            {
                if (melds[i].Contains(potential[j]))
                    added = true;
                else
                    added = false;
            }
            if (added)
                return true;
        }

        return false;
    }

    public void HighlightMelds()
    {
        for (int i = 0; i < cardsHolderUI.transform.childCount; i++)
        {
            Debug.Log("LOL");
            cardsHolderUI.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);

            for (int k = 0; k < melds.Count; k++)
            {
                for (int j = 0; j < melds[k].Count; j++)
                {
                    if (cardsHolderUI.transform.GetChild(i).tag.CompareTo(melds[k][j].ID.ToString()) == 0)
                    {
                        Debug.Log("HIOGLIGHT");
                        cardsHolderUI.transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    public void CalculateMeldSum()
    {
        int sum = 0;

        for (int i = 0; i < melds.Count; i++)
        {
            for (int j = 0; j < melds[i].Count; j++)
            {
                sum += melds[i][j].cardValue;
            }
        }

        meldSumText.text = sum.ToString();

        if (sum >= 3)
        {
            meldSumGoDown.gameObject.SetActive(true);
        }
        else
        {
            meldSumGoDown.gameObject.SetActive(false);
        }
    }

    public void GoDownWithMelds()
    {
        Debug.LogError(meldSetUI.transform.childCount + " UI");
        Debug.LogError(melds.Count + " COUNT MELDS");
        if (HandManager.instance.players[HandManager.instance.mainPlayerID].playerTurn)
        {

        }

        if (gameObject.tag == "MainPlayer")
        {
            for (int i = 0; i < melds.Count; i++)
            {
                Debug.Log("GOOO DOWNWNWNNWNW");
                GameObject meldSet = Instantiate(meldSetUI, meldGroupsUI.transform);

                for (int j = 0; j < melds[i].Count; j++)
                {
                    for (int k = 0; k < cardsHolderUI.transform.childCount; k++)
                    {
                        if (cardsHolderUI.transform.GetChild(k).gameObject.tag == melds[i][j].ID)
                        {
                            Debug.LogError(cardsHolderUI.transform.GetChild(k).name);
                            Destroy(cardsHolderUI.transform.GetChild(k).gameObject.GetComponent<Draggable>());
                            cardsHolderUI.transform.GetChild(k).GetChild(0).gameObject.SetActive(false);
                            cardsHolderUI.transform.GetChild(k).GetComponent<LayoutElement>().preferredHeight = cardTableUI.GetComponent<LayoutElement>().preferredHeight;
                            cardsHolderUI.transform.GetChild(k).GetComponent<LayoutElement>().preferredWidth = cardTableUI.GetComponent<LayoutElement>().preferredWidth;
                            cardsHolderUI.transform.GetChild(k).GetComponent<RectTransform>().anchoredPosition = cardTableUI.GetComponent<RectTransform>().anchoredPosition;
                            cardsHolderUI.transform.GetChild(k).GetComponent<RectTransform>().anchorMin = cardTableUI.GetComponent<RectTransform>().anchorMin;
                            cardsHolderUI.transform.GetChild(k).GetComponent<RectTransform>().anchorMax = cardTableUI.GetComponent<RectTransform>().anchorMax;
                            cardsHolderUI.transform.GetChild(k).GetComponent<RectTransform>().sizeDelta = cardTableUI.GetComponent<RectTransform>().sizeDelta;
                            cardsHolderUI.transform.GetChild(k).SetParent(meldSet.transform);
                        }
                    }
                }
            }
        }

    }

}

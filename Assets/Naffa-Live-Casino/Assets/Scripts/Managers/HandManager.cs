using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HandManager : MonoBehaviour
{
    public static HandManager instance { get; private set; }

    [SerializeField] private int cardsDealtPerRound;
    [SerializeField] public PlayerHandler[] players;
    [SerializeField] public GameObject[] cardPos;

    public Card[] cards;

    public List<Card> deck { get; set; }
    public int mainPlayerID { get; set; }
    public int whoDeals { get; set; }
    public int whoPlays;
    public float timeToStart;
    private float timeToStartCount;
    public bool isRoundStarted { get; set; }
    public List<PlayerHandler> Team1;
    public List<PlayerHandler> Team2;

    [SerializeField] private GameObject pleaseWaitPanel;
    [SerializeField] private AudioSource shuffleSound;

    public Vector2[] playersPos;
    public bool isGameStarted = false;
    [SerializeField] public PlayerHandler crrentPlayerID;

    [SerializeField] public GameObject HardPlay;
    public bool ReadyForHardMode { set; get; }
    [SerializeField] private GameObject ScoreManager;
    [SerializeField] private GameObject OrderManager;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void Start()
    {
        isRoundStarted = false;
        StartCoroutine(WaitForShuffleSound());


    }

    void CreateTeams(PlayerHandler FirstPlayer)
    {

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].tag == "MainPlayer")
            {
                Team1.Add(players[i]);
                players[i].playerTeam = "Team1";

            }
            else if (players[i].tag == FirstPlayer.tag)
            {
                Team1.Add(FirstPlayer);
                players[i].playerTeam = "Team1";

            }
            else
            {
                Team2.Add(players[i]);
                players[i].playerTeam = "Team2";
            }
        }



        PlayerHandler _player1 = Team1[0];
        PlayerHandler _player2 = Team2[0];
        PlayerHandler _player3 = Team1[1];
        PlayerHandler _player4 = Team2[1];

        players = new PlayerHandler[4];
        players[0] = _player1;
        players[1] = _player2;
        players[2] = _player3;
        players[3] = _player4;

        _player1.playerNumber = 0;
        _player2.playerNumber = 1;
        _player3.playerNumber = 2;
        _player4.playerNumber = 3;

        _player1.playerUI.GetComponent<RectTransform>().anchoredPosition = playersPos[0];
        _player2.playerUI.GetComponent<RectTransform>().anchoredPosition = playersPos[1];
        _player3.playerUI.GetComponent<RectTransform>().anchoredPosition = playersPos[2];
        _player4.playerUI.GetComponent<RectTransform>().anchoredPosition = playersPos[3];

    }
    public void ChoosePartner(PlayerHandler player)
    {
        CreateTeams(player);
        ResultPanel.instance.CreateResults();
        isGameStarted = true;
        PartnerSystem.instance.RemovePartnersListners();
        PanelSystem.instance.hidePanel("PartnerPanel");
    }

    public void PlayerHard()
    {
        if (ReadyForHardMode)
        {
            ReadyForHardMode = false;
        }
        else
        {
            ReadyForHardMode = true;
        }
    }
    void Update()
    {
        if (RoundController.instance.RoundNumber != 0 && crrentPlayerID.tag == "MainPlayer" && crrentPlayerID.playerTurn)
        {
            HardPlay.SetActive(true);
            if (ReadyForHardMode)
            {
                HardPlay.GetComponent<Image>().color = Color.yellow;
            }
            else
            {
                HardPlay.GetComponent<Image>().color = Color.white;
            }
        }
        else
        {
            HardPlay.GetComponent<Image>().color = Color.white;
            HardPlay.SetActive(false);
        }
        if (isGameStarted)
        {
            foreach (PlayerHandler player in players)
            {
                if (player.playerTurn)
                {
                    crrentPlayerID = player;
                }
            }
            if (isRoundStarted == false)
            {
                timeToStartCount += Time.deltaTime;

            }
            if (timeToStartCount >= timeToStart && isRoundStarted == false)
            {
                shuffleSound.volume = 0.2f;
                shuffleSound.clip = SoundSystem.Instance.ShuffleSound;
                //if shuffle sound is not playing

                shuffleSound.Play();
                isRoundStarted = true;
                timeToStartCount = 0;
                deck = new List<Card>();


                GenerateDeck();
                ShuffleDeck();
                DealCards();


                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i].gameObject.CompareTag("MainPlayer"))
                    {
                        mainPlayerID = i;
                    }

                    players[i].CreateCardsUI();


                }


                NextPlayerTurn(whoDeals);
            }
            if (!shuffleSound.isPlaying)
            {
                shuffleSound.volume = 1f;
            }
        }
    }
    IEnumerator WaitForShuffleSound()
    {
        pleaseWaitPanel.SetActive(true);
        yield return new WaitForSeconds(2f);
        pleaseWaitPanel.SetActive(false);
        yield return new WaitForSeconds(0.4f);
        PanelSystem.instance.showPanel("PartnerPanel");
        yield return new WaitForSeconds(0.4f);
        PartnerSystem.instance.StartCoroutine(PartnerSystem.instance.Distrbute());
    }
    public string GetPlayerTeamName(string PlayerTag)
    {
        string teamName = "";
        foreach (PlayerHandler player in players)
        {
            if (player.gameObject.CompareTag(PlayerTag))
            {
                teamName = player.playerTeam;
            }
        }
        return teamName;
    }
    public PlayerHandler GetPlayer(string PlayerTag)
    {
        PlayerHandler player = null;
        foreach (PlayerHandler player1 in players)
        {
            if (player1.gameObject.CompareTag(PlayerTag))
            {
                player = player1;
            }
        }
        return player;
    }
    void ClearChildren(Transform t)
    {
        foreach (Transform ch in t)
        {
            Destroy(ch.gameObject);
        }
    }
    public void ResetRoundItems()
    {
        isRoundStarted = false;
        foreach (PlayerHandler tPlayer in players)
        {
            ClearChildren(tPlayer.cardsHolderUI.transform);
            tPlayer.cards.RemoveAll(item => item != null);
            tPlayer.orderNumber = 0;
            tPlayer.PlayerOrder.text = "0";
            //delete all children
            tPlayer.orderNumber = 0;
            tPlayer.playerTurn = false;
            tPlayer.turnStart = false;
            tPlayer.score = 0;
            tPlayer.LoopTimes = 0;


        }


    }

    public void GenerateDeck()
    {
        foreach (Card card in cards)
            deck.Add(card);
    }

    public void ShuffleDeck()
    {
        System.Random rand = new System.Random();

        for (int i = 0; i < cards.Length; i++)
        {
            int r = i + rand.Next(deck.Count - i);

            //swap method between elements for randomness
            Card temp = deck[r];
            deck[r] = deck[i];
            deck[i] = temp;
        }
    }

    public void DealCards()
    {
        if (RoundController.instance.TarnebRound != 0)
        {
            int _firstPlayer = RoundController.instance.FirstPlayerNumber + 1;
            if (_firstPlayer >= players.Length)
            {
                _firstPlayer = 0;
            }
            whoDeals = _firstPlayer;
            RoundController.instance.FirstPlayerNumber = whoDeals;

        }
        else
        {
            whoDeals = Random.Range(0, players.Length);
            RoundController.instance.FirstPlayerNumber = whoDeals;
        }
        int _cardsDealtPerRound = 0;

        while (_cardsDealtPerRound < cardsDealtPerRound)
        {
            //first 13 cards are dealt to the main player
            if (_cardsDealtPerRound < 13)
            {
                players[0].cards.Add(deck[_cardsDealtPerRound]);
                _cardsDealtPerRound++;
            }

            else if (_cardsDealtPerRound < 26)
            {
                players[1].cards.Add(deck[_cardsDealtPerRound]);
                _cardsDealtPerRound++;
            }
            else if (_cardsDealtPerRound < 39)
            {
                players[2].cards.Add(deck[_cardsDealtPerRound]);
                _cardsDealtPerRound++;
            }
            else if (_cardsDealtPerRound < 52)
            {
                players[3].cards.Add(deck[_cardsDealtPerRound]);
                _cardsDealtPerRound++;
            }
            else
            {
                Debug.Log("No more cards to deal");
            }
        }
        int i = 0;
        foreach (PlayerHandler pl in players)
        {
            i++;
        }

    }


    public void NextPlayerTurn(int who)
    {

        if (who == 0)
        {
            players[who].playerTurn = true;
            whoPlays = 1;
        }
        else if (who == 1)
        {
            players[who].playerTurn = true;
            whoPlays = 2;
        }
        else if (who == 2)
        {
            players[who].playerTurn = true;
            whoPlays = 3;
        }
        else if (who == 3)
        {
            players[who].playerTurn = true;
            whoPlays = 0;
        }
        else
        {
            Debug.Log("Error in NextPlayerTurn");
        }
    }
    public void PlayCard(GameObject card)
    {

        card.GetComponent<Image>().sprite = card.GetComponent<CardInfo>().cardInfo.cardImage;
        //search in array of players for the player who played the card
        card.transform.SetParent(cardPos[crrentPlayerID.playerNumber].transform);

        card.gameObject.transform.localScale = Vector3.one;
        card.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.5f, 0.5f);
        card.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        card.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        card.gameObject.transform.localPosition = Vector3.zero;

        GetComponent<RoundController>().playerPlayed++;
        crrentPlayerID.GetComponent<TimerController>().EndPlayerTurn();

        if (!crrentPlayerID.CompareTag("MainPlayer"))
        {
            //animation on scale
            card.GetComponent<Animator>().enabled = true;
            card.GetComponent<RectTransform>().sizeDelta = cardPos[crrentPlayerID.playerNumber].GetComponent<RectTransform>().sizeDelta;

            card.transform.localRotation = Quaternion.Euler(0, 0, 0);
            card.GetComponent<AudioSource>().Play();

        }
        else
        {
            card.GetComponent<RectTransform>().sizeDelta = cardPos[crrentPlayerID.playerNumber].GetComponent<RectTransform>().sizeDelta;

            if (ReadyForHardMode)
            {
                if (card.GetComponent<CardInfo>().cardInfo.cardType.ToString() == RoundController.instance.GetTypeName(RoundController.instance.TypeNumber))
                {
                    card.GetComponent<AudioSource>().Play();
                    ReadyForHardMode = false;

                }
                else
                {

                    ReadyForHardMode = false;
                    SoundSystem.Instance.GetComponent<AudioSource>().clip = SoundSystem.Instance.TableHard;
                    SoundSystem.Instance.GetComponent<AudioSource>().Play();
                }
            }
            else
            {
                card.GetComponent<AudioSource>().Play();
            }
        }

    }
    public void CheckTableCards()
    {
        Debug.Log("Checking table cards");
    }
    IEnumerator DelayAction(float delay, System.Action action)
    {
        yield return new WaitForSeconds(delay);
        action();
    }
}

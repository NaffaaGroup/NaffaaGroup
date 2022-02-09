using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Tarneb41.Scripts
{

    public class RoundController : MonoBehaviour
    {
        [SerializeField] public int RoundNumber;
        [SerializeField] GameObject OrderPanel;
        [SerializeField] GameObject OrderType;
        public int TableOrder;
        public int TypeNumber;
        public int TarnebRound;
        public int FirstPlayerNumber;
        public PlayerHandler playerOrdered;
        bool allOrder = false;
        public GameObject PlayAgainPanel;
        public int orderCount = 0;
        public int playerPlayed = 0;

        [SerializeField] private bool resetRound = false;
        public int oldPlayerWin = 0;
        PlayerHandler[] orderPlayerTeam;
        public PlayerHandler oldPlayerWinOBj;
        public int Team1Score = 0;
        public int Team2Score = 0;
        public static RoundController instance { get; private set; }


        [Header("Round Settings")]
        [SerializeField] int MaxWinScore = 41;
        [SerializeField] int MaxLoseScore = -26;
        [SerializeField] int CaboodWithoutOrder = 16;
        [SerializeField] int CaboodWithOrder = 26;

        void Awake()
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
            RoundNumber = 0;

        }

        public void ChooseType(int type)
        {
            TypeNumber = type;
            playerOrdered = HandManager.instance.crrentPlayerID;
            orderPlayerTeam = GetTeam(CheckCurrentPlayerTeam(playerOrdered));
            playerOrdered.playerOrderIndex = PlayerHandler.PlayerOrderIndex.first;
            foreach (PlayerHandler player in HandManager.instance.players)
            {
                if (player == playerOrdered)
                {
                    player.PlayerOrder.transform.parent.gameObject.SetActive(true);
                    player.playerOrderIndex = PlayerHandler.PlayerOrderIndex.first;
                }
                else
                {
                    player.PlayerOrder.transform.parent.gameObject.SetActive(false);
                    player.playerOrderIndex = PlayerHandler.PlayerOrderIndex.second;
                }
            }
            //type 1 mean spade , 2 mean diamond , 3 mean heart , 4 mean club
            OrderType.SetActive(false);
            RoundNumber++;
            TypesAssets.Instance.Show(playerOrdered.playerNumber, TypeNumber - 1, playerOrdered.orderNumber);
            TypesAssets.Instance.ChangeText(TableOrder.ToString(), playerOrdered.playerNumber);
            TypesAssets.Instance.ShowType(type - 1, playerOrdered.playerNumber);
            // playerOrdered.playerUI.transform.GetChild(8).GetChild(type - 1).gameObject.SetActive(true);
            SoundSystem.Instance.GetComponent<AudioSource>().clip = SoundSystem.Instance.CardType[type - 1];
            SoundSystem.Instance.GetComponent<AudioSource>().Play();
            if (playerOrdered.playerType.ToString() == "Bot")
            {
                playerOrdered.GetComponent<TimerController>().seconds = playerOrdered.GetComponent<TimerController>().secondsForAI;
            }
            else
            {
                playerOrdered.GetComponent<TimerController>().seconds = playerOrdered.GetComponent<TimerController>().secondsForPlayers;
            }
        }
        PlayerHandler whoPlay()
        {
            foreach (PlayerHandler playerT in HandManager.instance.players)
            {
                if (playerT.playerTurn)
                {
                    return playerT;
                }
            }
            return null;
        }
        CardInfo GetFirstPlayedCard()
        {
            foreach (Transform g in GameObject.Find("TableZone").transform)
            {
                if (g.childCount > 0)
                {

                    if (g.GetChild(0).GetComponent<CardInfo>().player.playerOrderIndex.ToString() == "first")
                    {
                        return g.GetChild(0).gameObject.GetComponent<CardInfo>();
                    }
                }
            }
            return null;
        }
        public void StartNewRound()
        {
            RoundNumber++;
            playerPlayed = 0;

            oldPlayerWin = WhoWin().playerNumber;
            oldPlayerWinOBj = WhoWin();
            WhoWin().GetComponent<PlayerHandler>().score += 1;
            foreach (Transform child in GameObject.Find("TableZone").transform)
            {
                if (child.childCount > 0)
                {
                    GameObject ch = child.GetChild(0).gameObject;

                    ch.transform.SetParent(GameObject.Find("TrashZone").transform);
                    Destroy(ch, 1f);
                }
            }

            foreach (PlayerHandler player in HandManager.instance.players)
            {
                player.PlayerOrder.transform.parent.gameObject.SetActive(true);
                player.PlayerOrder.transform.parent.GetComponent<Image>().color = Color.white;

                player.isOrder = false;
                player.playerOrderIndex = PlayerHandler.PlayerOrderIndex.second;
                player.playerTurn = false;
                if (player.gameObject.tag == "MainPlayer")
                {
                    foreach (Transform card in player.cardsHolderUI.transform)
                    {
                        card.GetComponent<Draggable>().CanDrag = false;
                        card.GetComponent<Image>().color = Color.white;

                    }
                }
            }
            //choose player who play first

            HandManager.instance.whoPlays = oldPlayerWinOBj.playerNumber;
            oldPlayerWinOBj.playerTurn = true;
            oldPlayerWinOBj.playerOrderIndex = PlayerHandler.PlayerOrderIndex.first;

        }
        void FixedUpdate()
        {

            if (HandManager.instance.isRoundStarted)
            {


                if (resetRound)
                {
                    StartNewRound();
                    resetRound = false;
                }

                if (RoundNumber == 0)
                {
                    int playedPlayer = 0;
                    foreach (PlayerHandler pl in HandManager.instance.players)
                    {
                        TypesAssets.Instance.ShowCard(pl.playerNumber);
                        if (pl.isOrder)
                        {
                            playedPlayer++;
                        }

                        foreach (Draggable card in HandManager.instance.players[HandManager.instance.mainPlayerID]
                        .cardsHolderUI.GetComponentsInChildren<Draggable>())
                        {
                            card.GetComponent<Draggable>().CanDrag = false;
                            card.GetComponent<Image>().color = Color.white;
                        }
                    }
                    if (playedPlayer == 4)
                    {
                        PlayAgain();
                    }
                    try
                    {
                        if (HandManager.instance.crrentPlayerID.playerType.ToString() != "Bot")
                        {

                            if (HandManager.instance.crrentPlayerID.turnStart || HandManager.instance.crrentPlayerID.isOrder)
                            {
                                OrderPanel.transform.GetChild(0).GetComponent<OrderButton>().Order(0);
                                OrderPanel.SetActive(false);
                                OrderType.SetActive(false);

                            }
                            else
                            {
                                if (HandManager.instance.crrentPlayerID.PlayerOrder.text == TableOrder.ToString() && TableOrder != 0 && orderCount >= 3)
                                {
                                    OrderPanel.SetActive(false);
                                    OrderType.SetActive(true);
                                }
                                else if (HandManager.instance.crrentPlayerID.PlayerOrder.text == "Pass")
                                {
                                    OrderPanel.transform.GetChild(0).GetComponent<OrderButton>().Order(0);
                                    OrderPanel.SetActive(false);
                                }
                                else
                                {
                                    OrderPanel.SetActive(true);
                                }
                            }
                        }

                        if (HandManager.instance.crrentPlayerID.turnStart || HandManager.instance.crrentPlayerID.isOrder)
                        {

                            //HANDLE IF PLAYER HAS SOME ERRORS .. 
                            if (HandManager.instance.crrentPlayerID.playerType.ToString() == "Bot")
                            {
                                if (HandManager.instance.crrentPlayerID.
                                GetComponent<TimerController>().seconds <= HandManager.instance.crrentPlayerID.
                                GetComponent<TimerController>().secondsForAI - 3f)
                                {
                                    //get random card
                                    GameObject randomCard = HandManager.instance.crrentPlayerID.cardsHolderUI.transform.GetChild(UnityEngine.Random.Range(0, HandManager.instance.crrentPlayerID.cardsHolderUI.transform.childCount)).gameObject;
                                    HandManager.instance.PlayCard(randomCard);
                                }
                            }
                            OrderPanel.SetActive(false);
                            if (HandManager.instance.crrentPlayerID.playerType.ToString() != "Bot")
                            {
                                OrderPanel.transform.GetChild(0).GetComponent<OrderButton>().Order(0);
                            }
                            //check if bot order pass our turn start
                            if (HandManager.instance.crrentPlayerID.turnStart && !HandManager.instance.crrentPlayerID.isOrder)
                            {

                                int spadeCount = 0;
                                int heartCount = 0;
                                int clubCount = 0;
                                int diamondCount = 0;
                                //get higer card type to order
                                foreach (Transform card in HandManager.instance.crrentPlayerID.cardsHolderUI.transform)
                                {
                                    if (card.GetComponent<CardInfo>().cardInfo.cardType.ToString() == "Spade")
                                    {
                                        spadeCount++;
                                    }
                                    if (card.GetComponent<CardInfo>().cardInfo.cardType.ToString() == "Heart")
                                    {
                                        heartCount++;
                                    }
                                    if (card.GetComponent<CardInfo>().cardInfo.cardType.ToString() == "Club")
                                    {
                                        clubCount++;
                                    }
                                    if (card.GetComponent<CardInfo>().cardInfo.cardType.ToString() == "Diamond")
                                    {
                                        diamondCount++;
                                    }
                                }
                                //check if bot order higher than table order


                                //check if all player order pass
                                if (orderCount >= 3 && HandManager.instance.crrentPlayerID.PlayerOrder.text != "pass")
                                {
                                    //this mean its last bot
                                    allOrder = true;
                                    if (spadeCount >= 6)
                                    {
                                        HandManager.instance.crrentPlayerID.PlayerOrder.text = (spadeCount + 1).ToString();
                                        TableOrder = spadeCount + 1;
                                        ChooseType(1);
                                    }
                                    else if (heartCount >= 6)
                                    {
                                        HandManager.instance.crrentPlayerID.PlayerOrder.text = (heartCount + 1).ToString();
                                        TableOrder = heartCount + 1;
                                        ChooseType(3);
                                    }
                                    else if (clubCount >= 6)
                                    {
                                        HandManager.instance.crrentPlayerID.PlayerOrder.text = (clubCount + 1).ToString();
                                        TableOrder = clubCount + 1;
                                        ChooseType(4);
                                    }
                                    else if (diamondCount >= 6)
                                    {
                                        HandManager.instance.crrentPlayerID.PlayerOrder.text = (diamondCount + 1).ToString();
                                        TableOrder = diamondCount + 1;
                                        ChooseType(2);
                                    }
                                    else if (HandManager.instance.crrentPlayerID.orderNumber < TableOrder)
                                    {
                                        OrderPanel.transform.GetChild(0).GetComponent<OrderButton>().Order(0);

                                    }
                                    //if bot didnt have cards to order and bot is last bot to play
                                    else
                                    {
                                        OrderPanel.transform.GetChild(0).GetComponent<OrderButton>().Order(0);
                                    }
                                }
                                else
                                {
                                    //bot order based on type number
                                    if (!HandManager.instance.crrentPlayerID.isOrder && HandManager.instance.crrentPlayerID.PlayerOrder.text != "pass")
                                    {
                                        if (spadeCount >= 6)
                                        {
                                            if (spadeCount + 1 > TableOrder)
                                            {
                                                OrderPanel.transform.GetChild(0).GetComponent<OrderButton>().Order(spadeCount + 1);
                                            }
                                            else
                                            {
                                                OrderPanel.transform.GetChild(0).GetComponent<OrderButton>().Order(0);
                                            }
                                        }
                                        else if (heartCount >= 6)
                                        {
                                            if (heartCount + 1 > TableOrder)
                                            {
                                                OrderPanel.transform.GetChild(1).GetComponent<OrderButton>().Order(heartCount + 1);
                                            }
                                            else
                                            {
                                                OrderPanel.transform.GetChild(1).GetComponent<OrderButton>().Order(0);
                                            }
                                        }
                                        else if (clubCount >= 6)
                                        {
                                            if (clubCount + 1 > TableOrder)
                                            {
                                                OrderPanel.transform.GetChild(2).GetComponent<OrderButton>().Order(clubCount + 1);
                                            }
                                            else
                                            {
                                                OrderPanel.transform.GetChild(2).GetComponent<OrderButton>().Order(0);
                                            }
                                        }
                                        else if (diamondCount >= 6)
                                        {
                                            if (diamondCount + 1 > TableOrder)
                                            {
                                                OrderPanel.transform.GetChild(3).GetComponent<OrderButton>().Order(diamondCount + 1);
                                            }
                                            else
                                            {
                                                OrderPanel.transform.GetChild(3).GetComponent<OrderButton>().Order(0);
                                            }
                                        }
                                        else if (HandManager.instance.crrentPlayerID.orderNumber < TableOrder)
                                        {
                                            OrderPanel.transform.GetChild(0).GetComponent<OrderButton>().Order(0);
                                        }
                                        else
                                        {
                                            OrderPanel.transform.GetChild(0).GetComponent<OrderButton>().Order(0);
                                        }
                                    }
                                    else
                                    {
                                        OrderPanel.transform.GetChild(0).GetComponent<OrderButton>().Order(0);
                                    }
                                }


                            }
                            else
                            {
                                OrderPanel.transform.GetChild(0).GetComponent<OrderButton>().Order(0);
                            }
                        }


                        if (orderCount >= 4 && TableOrder == 0)
                        {
                            PlayAgain();
                        }
                    }
                    catch
                    {

                    }
                }
                else
                {
                    foreach (PlayerHandler player in HandManager.instance.players)
                    {
                        if (player == playerOrdered)
                        {
                            player.PlayerOrder.transform.parent.gameObject.SetActive(true);
                        }
                        else
                        {
                            player.PlayerOrder.transform.parent.gameObject.SetActive(false);
                        }
                    }
                    if (RoundNumber == 1)
                    {
                        if (playerPlayed == 0)
                        {
                            oldPlayerWinOBj = HandManager.instance.crrentPlayerID;
                        }
                    }
                    int TotalOrderTeam = 0;
                    int OtherTeamOrder = 0;
                    foreach (PlayerHandler tPlayer in orderPlayerTeam)
                    {
                        TotalOrderTeam += tPlayer.score;
                    }
                    OtherTeamOrder = RoundNumber - TotalOrderTeam - 1;

                    foreach (PlayerHandler tPlayer in orderPlayerTeam)
                    {

                        if (TotalOrderTeam >= TableOrder)
                        {
                            CurrentOrder.Instance.getScoreParent(tPlayer.playerNumber).GetComponent<Image>().color = Color.green;
                        }
                        else if (OtherTeamOrder > 13 - TableOrder)
                        {
                            CurrentOrder.Instance.getScoreParent(tPlayer.playerNumber).GetComponent<Image>().color = Color.red;
                        }
                        else
                        {
                            CurrentOrder.Instance.getScoreParent(tPlayer.playerNumber).GetComponent<Image>().color = Color.grey;
                        }

                    }

                    PlayerHandler currentPlayer = HandManager.instance.crrentPlayerID;
                    string OrderTypeName = GetTypeName(TypeNumber);
                    foreach (Draggable card in HandManager.instance.crrentPlayerID.cardsHolderUI.GetComponentsInChildren<Draggable>())
                    {
                        card.GetComponent<Draggable>().CanDrag = true;
                        card.GetComponent<Image>().color = Color.white;

                    }
                    if (playerPlayed >= 4)
                    {
                        StartNewRound(); ;
                    }
                    if (currentPlayer.playerType.ToString() == "Bot")
                    {
                        if (currentPlayer.turnStart)
                        {
                            string TypeName = "";
                            if (currentPlayer.playerOrderIndex.ToString() == "first")
                            {

                                int RandomCard = UnityEngine.Random.Range(0, HandManager.instance.crrentPlayerID.cardsHolderUI.transform.childCount);
                                HandManager.instance.PlayCard(currentPlayer.cardsHolderUI.transform.GetChild(RandomCard).gameObject);
                            }
                            else
                            {

                                TypeName = GetFirstPlayedCard().cardInfo.cardType.ToString();
                                int m = 0;

                                foreach (Transform child in currentPlayer.cardsHolderUI.transform)
                                {
                                    //get card type count

                                    if (child.GetComponent<CardInfo>().cardInfo.cardType.ToString() == TypeName)
                                    {
                                        m++;
                                    }
                                }

                                bool breakLoop = false;
                                //loop in player card
                                foreach (Transform child in currentPlayer.cardsHolderUI.transform)
                                {
                                    HandManager.instance.crrentPlayerID.LoopTimes++;
                                    CardInfo Card = child.GetComponent<CardInfo>();
                                    breakLoop = CheckCardTable(Card, TypeName, m, HandManager.instance.crrentPlayerID.LoopTimes, OrderTypeName);
                                    if (HandManager.instance.crrentPlayerID.LoopTimes > 24)
                                    {
                                        Debug.LogWarning("LoopTimes is bigger than 24");
                                        HandManager.instance.PlayCard(child.gameObject);
                                        breakLoop = true;
                                        break;
                                    }
                                    if (breakLoop)
                                    {
                                        breakLoop = false;
                                        break;
                                    }


                                }
                            }


                        }
                    }
                    else
                    {
                        if (HandManager.instance.players[HandManager.instance.mainPlayerID].ReadyToDrop && HandManager.instance.players[HandManager.instance.mainPlayerID].playerTurn)
                        {
                            if (HandManager.instance.crrentPlayerID.GetComponent<TimerController>().ReadyToStart)
                            {

                                foreach (Transform child in currentPlayer.cardsHolderUI.transform)
                                {
                                    //get get Ready To drag Cart

                                    if (currentPlayer.playerOrderIndex.ToString() == "first")
                                    {
                                        if (child.GetComponent<Draggable>().ReadyToDrag)
                                        {
                                            HandManager.instance.PlayCard(child.gameObject);
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (child.GetComponent<Draggable>().ReadyToDrag)
                                        {
                                            if (CheckIfAllowed(child.GetComponent<CardInfo>()))
                                            {
                                                HandManager.instance.PlayCard(child.gameObject);
                                            }
                                            else
                                            {
                                                child.GetComponent<Draggable>().CanDrag = false;
                                                break;
                                            }
                                        }
                                    }
                                }

                            }
                        }

                        if (currentPlayer.GetComponent<TimerController>().seconds <= 1.5f)
                        {
                            //play random card because player is afk
                            if (currentPlayer.playerOrderIndex.ToString() == "first")
                            {
                                int RandomCard = UnityEngine.Random.Range(0, HandManager.instance.crrentPlayerID.cardsHolderUI.transform.childCount);
                                HandManager.instance.PlayCard(currentPlayer.cardsHolderUI.transform.GetChild(RandomCard).gameObject);
                            }
                            else
                            {
                                string TypeName = GetFirstPlayedCard().cardInfo.cardType.ToString();
                                int m = 0;

                                foreach (Transform child in currentPlayer.cardsHolderUI.transform)
                                {
                                    //get card type count

                                    if (child.GetComponent<CardInfo>().cardInfo.cardType.ToString() == TypeName)
                                    {
                                        m++;
                                    }
                                }

                                bool breakLoop = false;
                                //loop in player card



                                foreach (Transform child in currentPlayer.cardsHolderUI.transform)
                                {
                                    if (child.GetComponent<Draggable>().ReadyToDrag)
                                    {
                                        HandManager.instance.PlayCard(child.gameObject);
                                        breakLoop = true;
                                        break;
                                    }
                                }
                                if (!breakLoop)
                                {
                                    foreach (Transform child in currentPlayer.cardsHolderUI.transform)
                                    {
                                        HandManager.instance.crrentPlayerID.LoopTimes++;
                                        CardInfo Card = child.GetComponent<CardInfo>();
                                        breakLoop = CheckCardTable(Card, TypeName, m, HandManager.instance.crrentPlayerID.LoopTimes, OrderTypeName);

                                        if (HandManager.instance.crrentPlayerID.LoopTimes > 24)
                                        {
                                            Debug.LogWarning("LoopTimes is bigger than 24");
                                            HandManager.instance.PlayCard(child.gameObject);
                                            breakLoop = true;
                                            break;
                                        }
                                        if (breakLoop)
                                        {
                                            breakLoop = false;
                                            break;
                                        }

                                    }
                                }
                            }
                            Debug.LogWarning(currentPlayer.playerName + "Player is afk");
                        }
                    }

                    if (currentPlayer.playerOrderIndex.ToString() != "first")
                    {
                        int SameTypeCount = 0;
                        //check how many cards of same played type
                        try
                        {
                            foreach (Transform cardComp in currentPlayer.cardsHolderUI.transform)
                            {
                                if (cardComp.GetComponent<CardInfo>().cardInfo.cardType.ToString() == GetFirstPlayedCard().cardInfo.cardType.ToString())
                                {
                                    SameTypeCount++;
                                }
                            }

                            if (SameTypeCount > 0)
                            {

                                foreach (Transform cardComp in currentPlayer.cardsHolderUI.transform)
                                {
                                    GameObject cardCoObj = cardComp.gameObject;
                                    string cardType = GetFirstPlayedCard().cardInfo.cardType.ToString();
                                    if (cardCoObj.GetComponent<CardInfo>().cardInfo.cardType.ToString() != cardType)
                                    {
                                        cardCoObj.GetComponent<Draggable>().CanDrag = false;
                                        cardCoObj.GetComponent<Image>().color = Color.gray;

                                    }

                                    else
                                    {
                                        cardCoObj.GetComponent<Draggable>().CanDrag = true;
                                        cardCoObj.GetComponent<Image>().color = Color.white;
                                    }
                                }

                            }
                            else
                            {
                                foreach (Transform cardComp in currentPlayer.cardsHolderUI.transform)
                                {
                                    GameObject cardCoObj = cardComp.gameObject;
                                    cardCoObj.GetComponent<Draggable>().CanDrag = true;
                                    cardCoObj.GetComponent<Image>().color = Color.white;
                                }

                            }
                        }
                        catch
                        {

                        }
                    }


                    if (RoundNumber >= 14)
                    {
                        // PlayAgainPanel.SetActive(true);
                        TotalOrderTeam = 0;
                        OtherTeamOrder = 0;
                        TarnebRound++;
                        foreach (PlayerHandler tPlayer in orderPlayerTeam)
                        {
                            TotalOrderTeam += tPlayer.score;
                        }
                        OtherTeamOrder = RoundNumber - TotalOrderTeam - 1;

                        int _team1TempScore = 0;
                        int _team2TempScore = 0;
                        if (CheckCurrentPlayerTeam(orderPlayerTeam[0]) == "Team1")
                        {
                            if (TotalOrderTeam >= TableOrder)
                            {
                                if (Team1Score == 13)
                                {
                                    if (TotalOrderTeam == 13)
                                    {
                                        Team1Score += CaboodWithOrder;
                                        _team1TempScore = CaboodWithOrder;
                                        _team2TempScore = 0;
                                    }
                                    else
                                    {
                                        Team1Score += CaboodWithoutOrder;
                                        _team1TempScore = CaboodWithoutOrder;
                                        _team2TempScore = 0;
                                    }
                                }
                                else
                                {
                                    Team1Score += TotalOrderTeam;
                                    _team1TempScore = TotalOrderTeam;
                                    _team2TempScore = 0;
                                }

                            }
                            else
                            {
                                Team1Score -= TableOrder;
                                _team1TempScore = -TableOrder;
                                if (Team2Score == 13)
                                {
                                    Team2Score += CaboodWithoutOrder;
                                    _team2TempScore = CaboodWithoutOrder;

                                }
                                else
                                {
                                    Team2Score += OtherTeamOrder;
                                    _team2TempScore = OtherTeamOrder;
                                }
                            }
                            if (Team1Score >= MaxWinScore)
                            {
                                PlayAgainPanel.SetActive(true);
                                // GameObject ob = PlayAgainPanel.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().gameObject;
                                // Instantiate(ob).gameObject.GetComponent<UnityEngine.UI.Text>().text="You win"
                            }
                            else if (Team1Score <= MaxLoseScore)
                            {
                                PlayAgainPanel.SetActive(true);
                            }



                        }
                        else
                        {

                            if (TotalOrderTeam >= TableOrder)
                            {
                                if (TotalOrderTeam == 13)
                                {
                                    if (Team2Score == 13)
                                    {
                                        Team2Score += CaboodWithOrder;
                                        _team2TempScore = CaboodWithOrder;
                                        _team1TempScore = 0;
                                    }
                                    else
                                    {
                                        Team2Score += CaboodWithoutOrder;
                                        _team2TempScore = CaboodWithoutOrder;
                                        _team1TempScore = 0;
                                    }
                                }
                                else
                                {
                                    Team2Score += TotalOrderTeam;
                                    _team2TempScore = TotalOrderTeam;
                                    _team1TempScore = 0;
                                }
                            }
                            else
                            {
                                if (Team2Score == 13)
                                {
                                    Team2Score += CaboodWithoutOrder;
                                    _team2TempScore = CaboodWithoutOrder;
                                    _team1TempScore = 0;
                                }
                                else
                                {
                                    Team2Score += OtherTeamOrder;
                                    _team2TempScore = OtherTeamOrder;
                                    _team1TempScore = 0;
                                }
                            }
                            if (Team2Score >= MaxWinScore)
                            {
                                PlayAgainPanel.SetActive(true);
                            }
                            else if (Team2Score <= MaxLoseScore)
                            {
                                PlayAgainPanel.SetActive(true);
                            }
                        }
                        ResultPanel.instance.NewHistoryResults(TarnebRound, _team1TempScore, _team2TempScore);

                        PlayAgain();
                    }

                }

            }

        }
        bool CheckIfAllowed(CardInfo card)
        {
            string TypeN = "";
            foreach (Transform ch in GameObject.Find("TableZone").transform)
            {
                if (ch.childCount > 0)
                {
                    if (ch.GetChild(0).GetComponent<CardInfo>().player.playerOrderIndex.ToString() == "first")
                    {
                        TypeN = ch.GetChild(0).GetComponent<CardInfo>().cardInfo.cardType.ToString();
                    }
                }
            }
            bool doseHave = false;
            //check if player has TypeN card
            foreach (Transform ch in card.player.cardsHolderUI.transform)
            {
                if (ch.GetComponent<CardInfo>().cardInfo.cardType.ToString() == TypeN)
                {
                    doseHave = true;
                }
            }
            if (card.cardInfo.cardType.ToString() == TypeN || !doseHave)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public void PlayAgain()
        {
            HandManager.instance.ResetRoundItems();
            foreach (Transform ch in OrderPanel.transform)
            {
                ch.GetComponent<Button>().interactable = true;
                ch.GetComponent<Image>().color = ch.GetComponent<OrderButton>().buttonColor;
                ch.GetChild(0).GetComponent<Text>().color = Color.white;
            }
            foreach (PlayerHandler player in HandManager.instance.players)
            {
                CurrentOrder.Instance.getScoreParent(player.playerNumber).GetComponent<Image>().color = Color.gray;
                TypesAssets.Instance.Hide(player.playerNumber);
                player.isOrder = false;
            }
            foreach (Transform player in HandManager.instance.players[HandManager.instance.mainPlayerID].cardsHolderUI.transform)
            {
                player.GetComponent<Draggable>().CanDrag = false;
                player.GetComponent<Image>().color = Color.white;
            }
            orderCount = 0;
            TableOrder = 0;
            playerPlayed = 0;
            oldPlayerWin = 0;
            RoundNumber = 0;
            TypeNumber = 0;
            oldPlayerWinOBj = null;
            playerOrdered = null;

        }
        public PlayerHandler WhoWin()
        {
            PlayerHandler winner = null;
            int max = 0;
            string OrderTypeName = GetTypeName(TypeNumber);
            int typeNumbers = 0;
            foreach (Transform child in GameObject.Find("TableZone").transform)
            {
                if (child.childCount > 0)
                {
                    if (child.GetChild(0).GetComponent<CardInfo>().cardInfo.cardType.ToString() == OrderTypeName)
                    {
                        typeNumbers++;
                    }
                }
            }
            if (typeNumbers == 0)
            {
                foreach (Transform child in GameObject.Find("TableZone").transform)
                {
                    if (child.childCount > 0)
                    {
                        if (child.GetChild(0).GetComponent<CardInfo>().cardInfo.cardType.ToString() == GetFirstPlayedCard().cardInfo.cardType.ToString())
                        {
                            if (child.GetChild(0).GetComponent<CardInfo>().cardInfo.cardNumber > max)
                            {
                                max = child.GetChild(0).GetComponent<CardInfo>().cardInfo.cardNumber;
                                winner = child.GetChild(0).GetComponent<CardInfo>().player;
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (Transform child in GameObject.Find("TableZone").transform)
                {
                    if (child.childCount > 0)
                    {
                        if (child.GetChild(0).GetComponent<CardInfo>().cardInfo.cardType.ToString() == OrderTypeName)
                        {
                            if (child.GetChild(0).GetComponent<CardInfo>().cardInfo.cardNumber > max)
                            {
                                max = child.GetChild(0).GetComponent<CardInfo>().cardInfo.cardNumber;
                                winner = child.GetChild(0).GetComponent<CardInfo>().player;
                            }
                        }
                    }
                }
            }
            if (winner == null)
            {
                winner = HandManager.instance.crrentPlayerID;
            }
            return winner;
        }
        public string CheckCurrentPlayerTeam(PlayerHandler PlayerToCheck)
        {
            try
            {
                foreach (PlayerHandler player in HandManager.instance.Team1)
                {
                    if (player.CompareTag(PlayerToCheck.tag))
                    {
                        return "Team1";
                    }
                }
                foreach (PlayerHandler player in HandManager.instance.Team2)
                {
                    if (player.CompareTag(PlayerToCheck.tag))
                    {
                        return "Team2";
                    }
                }
            }
            catch
            {
                return "";

            }
            return "";
        }
        public string CheckCurrentPlayerTeamFromGameObject(GameObject PlayerToCheck)
        {
            try
            {
                foreach (PlayerHandler player in HandManager.instance.Team1)
                {
                    if (player.CompareTag(PlayerToCheck.tag))
                    {
                        return "Team1";
                    }
                }
                foreach (PlayerHandler player in HandManager.instance.Team2)
                {
                    if (player.CompareTag(PlayerToCheck.tag))
                    {
                        return "Team2";
                    }
                }
            }
            catch
            {
                return "";

            }
            return "";
        }
        PlayerHandler[] GetTeam(string TeamName)
        {
            if (TeamName == "Team1")
            {
                return HandManager.instance.Team1.ToArray();
            }
            else
            {
                return HandManager.instance.Team2.ToArray();
            }
        }
        bool SearchInTableCards(string TypeName)
        {
            foreach (Transform child in GameObject.Find("TableZone").transform)
            {
                if (child.childCount > 0)
                {
                    if (child.GetChild(0).GetComponent<CardInfo>().cardInfo.cardType.ToString() == TypeName)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        CardInfo GetPlayerCard(string mathType, string _type, PlayerHandler _player)
        {

            int higherCard = 0;
            int lowerCard = 14;
            CardInfo returnedCard = null;
            foreach (Transform card in _player.cardsHolderUI.transform)
            {
                if (card.GetComponent<CardInfo>().cardInfo.cardType.ToString() == _type)
                {
                    switch (mathType)
                    {
                        case "Higher":
                            if (card.GetComponent<CardInfo>().cardInfo.cardNumber > higherCard)
                            {
                                higherCard = card.GetComponent<CardInfo>().cardInfo.cardNumber;
                                returnedCard = card.GetComponent<CardInfo>();
                            }
                            break;
                        case "Lower":
                            if (card.GetComponent<CardInfo>().cardInfo.cardNumber < lowerCard)
                            {
                                lowerCard = card.GetComponent<CardInfo>().cardInfo.cardNumber;
                                returnedCard = card.GetComponent<CardInfo>();
                            }
                            break;
                    }
                }
            }
            return returnedCard;
        }

        CardInfo CheckTableCards(string mathType, string _type)
        {
            int higherCard = 0;
            int lowerCard = 14;
            CardInfo returnedCard = null;
            foreach (Transform child in GameObject.Find("TableZone").transform)
            {
                if (child.childCount > 0)
                {
                    switch (mathType)
                    {
                        case "Higher":
                            if (child.GetChild(0).GetComponent<CardInfo>().cardInfo.cardType.ToString() == _type)
                            {
                                if (child.GetChild(0).GetComponent<CardInfo>().cardInfo.cardNumber > higherCard)
                                {
                                    higherCard = child.GetChild(0).GetComponent<CardInfo>().cardInfo.cardNumber;
                                    returnedCard = child.GetChild(0).GetComponent<CardInfo>();
                                }
                            }
                            break;
                        case "Lower":
                            if (child.GetChild(0).GetComponent<CardInfo>().cardInfo.cardType.ToString() == _type)
                            {
                                if (child.GetChild(0).GetComponent<CardInfo>().cardInfo.cardNumber < lowerCard)
                                {
                                    lowerCard = child.GetChild(0).GetComponent<CardInfo>().cardInfo.cardNumber;
                                    returnedCard = child.GetChild(0).GetComponent<CardInfo>();
                                }
                            }
                            break;
                    }
                }
                else
                {
                    continue;
                }
            }
            return returnedCard;
        }
        bool CheckCardTable(CardInfo cardToCheck, string TypeName, int CardTypeCount, int LoopTimes, string OrderTypeName)
        {
            int currentPlayerCards = HandManager.instance.crrentPlayerID.cardsHolderUI.transform.childCount;
            int indexOfLoop = 0;
            int HigherCard = 0;
            foreach (Transform child in GameObject.Find("TableZone").transform)
            {
                if (child.childCount == 0)
                {

                }
                else
                {
                    //get higher card in table of same type 
                    if (child.GetChild(0).GetComponent<CardInfo>().cardInfo.cardType.ToString() == TypeName
                    && child.GetChild(0).GetComponent<CardInfo>().cardInfo.cardNumber > HigherCard)
                    {
                        HigherCard = child.GetChild(0).GetComponent<CardInfo>().cardInfo.cardNumber;
                    }
                }
            }

            foreach (Transform child in GameObject.Find("TableZone").transform)
            {

                indexOfLoop++;
                if (child.childCount == 0)
                {
                    //
                }
                else
                {
                    Card card = child.GetChild(0).GetComponent<CardInfo>().cardInfo;
                    //if he has the same played card
                    if (cardToCheck.cardInfo.cardType.ToString() == TypeName)
                    {
                        //if player has higher card then play highest 
                        if (cardToCheck.cardInfo.cardNumber
                         == GetPlayerCard("Higher", TypeName, HandManager.instance.crrentPlayerID).cardInfo.cardNumber
                        && cardToCheck.cardInfo.cardNumber > CheckTableCards("Higher", TypeName).cardInfo.cardNumber)
                        {
                            HandManager.instance.PlayCard(cardToCheck.gameObject);
                            return true;
                        }
                        else
                        {
                            //if player has card of same type and number is lower than higher card
                            if (LoopTimes >= HandManager.instance.crrentPlayerID.cardsHolderUI.transform.childCount)
                            {
                                HandManager.instance.PlayCard(cardToCheck.gameObject);
                                return true;
                            }
                            return false;
                        }
                    }
                    else
                    {
                        //play lower card if first player is same type
                        //here bot dosnet have any card of same player card
                        if (CardTypeCount == 0 && cardToCheck.cardInfo.cardType.ToString() == OrderTypeName)
                        {
                            //first : check if there otherOrderType card in table
                            if (SearchInTableCards(OrderTypeName))
                            {
                                //play higher orderType card 
                                if (cardToCheck.cardInfo.cardNumber > CheckTableCards("Higher", OrderTypeName).cardInfo.cardNumber)
                                {
                                    HandManager.instance.PlayCard(cardToCheck.gameObject);
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                //if there is no other orderType card in table
                                //play lower orderType card
                                //if higher player is in your team
                                if (cardToCheck.player.playerTeam == CheckTableCards("Higher", TypeName).player.playerTeam)
                                {
                                    return false;
                                }

                                if (cardToCheck.cardInfo.cardNumber == GetPlayerCard("Lower", OrderTypeName, cardToCheck.player).cardInfo.cardNumber)
                                {
                                    HandManager.instance.PlayCard(cardToCheck.gameObject);
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                        else if (CardTypeCount == 0 && LoopTimes >= HandManager.instance.crrentPlayerID.cardsHolderUI.transform.childCount && CheckCurrentPlayerTeam(HandManager.instance.crrentPlayerID)
                     != CheckCurrentPlayerTeam(GetFirstPlayedCard().player.GetComponent<PlayerHandler>()))
                        {
                            int LowerCard = 14;
                            foreach (Transform child2 in HandManager.instance.crrentPlayerID.cardsHolderUI.transform)
                            {
                                if (child2.transform.GetComponent<CardInfo>().cardInfo.cardNumber < LowerCard)
                                {
                                    LowerCard = child2.transform.GetComponent<CardInfo>().cardInfo.cardNumber;
                                }
                            }
                            if (cardToCheck.cardInfo.cardType.ToString() == OrderTypeName)
                            {
                                HandManager.instance.PlayCard(cardToCheck.gameObject);
                                return true;
                            }
                            else if (cardToCheck.cardInfo.cardNumber == LowerCard)
                            {
                                HandManager.instance.PlayCard(cardToCheck.gameObject);
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        //if player has tarneb but the played player is partner
                        else if (CardTypeCount == 0 && LoopTimes >= HandManager.instance.crrentPlayerID.cardsHolderUI.transform.childCount && CheckCurrentPlayerTeam(HandManager.instance.crrentPlayerID)
                     == CheckCurrentPlayerTeam(GetFirstPlayedCard().player.GetComponent<PlayerHandler>()))
                        {
                            int LowerCard = 14;
                            foreach (Transform child2 in HandManager.instance.crrentPlayerID.cardsHolderUI.transform)
                            {
                                if (child2.transform.GetComponent<CardInfo>().cardInfo.cardNumber < LowerCard)
                                {
                                    LowerCard = child2.transform.GetComponent<CardInfo>().cardInfo.cardNumber;
                                }
                            }
                            if (cardToCheck.cardInfo.cardNumber == LowerCard)
                            {
                                HandManager.instance.PlayCard(cardToCheck.gameObject);
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;

                        }
                    }

                }
            }
            return false;
        }
        public string GetTypeName(int TypeNumber)
        {
            switch (TypeNumber)
            {
                case 1:
                    return "Spade";
                case 2:
                    return "Diamond";
                case 3:
                    return "Heart";
                case 4:
                    return "Club";
                default:
                    return "";
            }
        }
        IEnumerator WaitForPlayer(Action act)
        {
            yield return new WaitForSeconds(1);
            act();
        }
    }
}
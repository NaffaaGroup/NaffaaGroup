using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{
    [SerializeField] public int secondsForPlayers;
    [SerializeField] public int secondsForAI;
    public Slider slider;


    public float seconds;

    private void Start()
    {
        if (GetComponent<PlayerHandler>().playerType.ToString() == "Bot")
        {
            seconds = secondsForAI;
            slider.value = seconds;
            slider.maxValue = secondsForAI;
        }
        else
        {
            seconds = secondsForPlayers;
            slider.value = seconds;
            slider.maxValue = secondsForPlayers;

        }
    }

    private void Update()
    {
        if (GetComponent<PlayerHandler>().playerTurn && seconds > 0)
        {
            seconds -= Time.deltaTime;
        }

        if (seconds < 1f)
        {
            EndPlayerTurn();
            
        }
        if (GetComponent<PlayerHandler>().playerType.ToString() == "Bot")
        {
            if (seconds < secondsForAI - 2f)
            {
                HandManager.instance.crrentPlayerID.turnStart = true;
            }
            slider.value = seconds;
            slider.maxValue = secondsForAI;

        }
        else
        {
            if (seconds < 1.2f)
            {
                HandManager.instance.crrentPlayerID.turnStart = true;
            }
            slider.value = seconds;
            slider.maxValue = secondsForPlayers;
        }

    }

    public void EndPlayerTurn()
    {
        HandManager.instance.crrentPlayerID.turnStart = false;
        HandManager.instance.crrentPlayerID.LoopTimes = 0;

        Debug.Log("HELLOOO");
        GetComponent<PlayerHandler>().playerTurn = false;
        seconds = secondsForPlayers;
        if (GetComponent<PlayerHandler>().playerType.ToString() == "Bot")
        {
            seconds = secondsForAI;

        }
        else
        {

            seconds = secondsForPlayers;

        }
        HandManager.instance.crrentPlayerID.playerTurn = false;
        if (HandManager.instance.crrentPlayerID.playerNumber == 3)
        {
            HandManager.instance.NextPlayerTurn(0);
        }
        else
        {
            HandManager.instance.NextPlayerTurn(HandManager.instance.crrentPlayerID.playerNumber + 1);
        }
    }


}

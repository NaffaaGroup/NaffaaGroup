using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
namespace Tarneb41.Scripts
{
    public class PlayAgainPanel : MonoBehaviour
    {
        private HandManager handManager;
        private RoundController roundManager;

        public GameObject Team1;
        public GameObject Team2;

        public Text[] playersNames;

        public Text Team1Score;
        public Text Team2Score;

        public Text RoundNumber;
        public Text PlayedTime;
        public Text Scores;

        float time;
        public void Start()
        {
            handManager = HandManager.instance;
            roundManager = RoundController.instance;
        }
        public void PlayAgain()
        {
            Time.timeScale=1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        }
        public void Update()
        {
            handManager.isGameStarted = false;
            time =handManager.timerC;
            foreach(PlayerHandler player in handManager.players)
            {
                playersNames[player.playerNumber].text = player.playerName;
            }
            Team1Score.text = roundManager.Team1Score.ToString();
            Team2Score.text = roundManager.Team2Score.ToString();
            RoundNumber.text = "Total Rounds : "+roundManager.TarnebRound.ToString();
            PlayedTime.text = "Played Time : "+(time/60).ToString("0.00");
            Scores.text ="My Score : "+((time/60)*7*handManager.players[handManager.mainPlayerID].score).ToString("0");
            if(roundManager.Team1Score>roundManager.Team2Score){
                Team1.GetComponent<Image>().color = Color.green;
                Team1Score.color = Color.green;
                Team2Score.color = Color.red;
                Team2.GetComponent<Image>().color = Color.red;
            }else if(roundManager.Team1Score<roundManager.Team2Score){
                Team2.GetComponent<Image>().color = Color.green;
                Team2Score.color = Color.green;
                Team1Score.color = Color.red;
                Team1.GetComponent<Image>().color = Color.red;
            }else{
                Team1.GetComponent<Image>().color = Color.yellow;
                Team2.GetComponent<Image>().color = Color.yellow;
                Team1Score.color = Color.yellow;
                Team2Score.color = Color.yellow;
            }
            Time.timeScale = 0;
        }

    }
}
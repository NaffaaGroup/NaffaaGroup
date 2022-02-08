using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Tarneb41.Scripts
{
    public class ResultPanel : MonoBehaviour
    {
        public static ResultPanel instance { get; private set; }
        public GameObject Players;
        public GameObject[] PlayersArray;
        public List<GameObject> team1Players;
        public List<GameObject> team2Players;
        public Text Team1Score;
        public Text Team2Score;
        public GameObject Round;
        public GameObject RoundsContent;

        [SerializeField]
        void Start()
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
        public void CreateResults()
        {
            foreach (Transform playerObj in Players.transform)
            {

                foreach (GameObject player in PlayersArray)
                {
                    if (HandManager.instance.GetPlayerTeamName(player.tag) == playerObj.name)
                    {
                        player.transform.SetParent(playerObj);
                        player.GetComponent<Text>().text = HandManager.instance.GetPlayer(player.tag).playerName;
                    }
                }

            }



        }
        public void NewHistoryResults(int RoundNumber, int Team1Score, int Team2Score)
        {
            GameObject round = Instantiate(Round, RoundsContent.transform);
            Scores scores = round.GetComponent<Scores>();
            scores.Team1Score.text = Team1Score.ToString();
            scores.Team2Score.text = Team2Score.ToString();
            scores.RoundNumber.text = RoundNumber.ToString();

        }
        public void ClearAllResults()
        {
            foreach (Transform child in RoundsContent.transform)
            {
                Destroy(child.gameObject);
            }
        }
        // Update is called once per frame
        void Update()
        {
            //if team1 score is greater than team2 score
            if (int.Parse(Team1Score.text) > int.Parse(Team2Score.text))
            {
                Team1Score.transform.parent.GetComponents<Image>()[0].color = Color.green;
                Team2Score.transform.parent.GetComponents<Image>()[0].color = Color.red;
            }
            else if (int.Parse(Team1Score.text) < int.Parse(Team2Score.text))
            {
                Team1Score.transform.parent.GetComponents<Image>()[0].color = Color.red;
                Team2Score.transform.parent.GetComponents<Image>()[0].color = Color.green;
            }
            else
            {
                Team1Score.transform.parent.GetComponents<Image>()[0].color = Color.grey;
                Team2Score.transform.parent.GetComponents<Image>()[0].color = Color.gray;
            }

        }
    }
}
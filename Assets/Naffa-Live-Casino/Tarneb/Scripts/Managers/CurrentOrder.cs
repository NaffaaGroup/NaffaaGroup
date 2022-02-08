using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentOrder : MonoBehaviour
{

    public static CurrentOrder Instance;
    public Text[] Scores;

    public Text[] TopScores;
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }    
    }

    public void SetPlayerScore(int score, int playerNumber)
    {
        Scores[playerNumber].text = score.ToString();
    }
    public GameObject getScoreParent(int playerNumber)
    {
        return Scores[playerNumber].transform.parent.gameObject;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TarneebManager : MonoBehaviour
{
    #region Private Variables

    public static string[] _suites = { "C", "D", "H", "S" };
    public static string[] _values = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
    
    public List<string> _deck;

    #endregion

    #region Unity Functions

    // Start is called before the first frame update
    void Start()
    {
        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #endregion

    #region Supporting Functions

    /// <summary>
    /// Starts the game
    /// </summary>
    public void StartGame()
    {
        _deck = GenerateDeck();
        ShuffleCards(_deck);
    }

    /// <summary>
    /// Generates the card deck
    /// </summary>
    /// <returns>The card deck</returns>
    private static List<string> GenerateDeck()
    {
        List<string> newDeck = new List<string>();

        foreach(string s in _suites)
        {
            foreach(string v in _values)
            {
                newDeck.Add(s + v);
            }
        }

        return newDeck;
    }

    /// <summary>
    /// Shuffles the cards
    /// </summary>
    /// <param name="list">The list of cards</param>
    private void ShuffleCards(List<string> list)
    {
        System.Random rand = new System.Random();
        int n = list.Count;

        while (n > 1)
        {
            int k = rand.Next(n);
            n--;
            string temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
    }

    #endregion
}

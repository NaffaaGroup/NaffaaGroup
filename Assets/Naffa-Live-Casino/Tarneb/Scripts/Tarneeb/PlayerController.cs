using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Private Variables

    private Card[] _cards;
    private int _requestGuess;
    private int _winsCount;

    [SerializeField] private Animator _anim;

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the requested guess of the player
    /// </summary>
    public int RequestedGuess
    {
        get { return _requestGuess; }
    }

    /// <summary>
    /// Gets the number of wins by the player in each turn
    /// </summary>
    public int WinCount
    {
        get { return _winsCount; }
    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        _cards = new Card[13];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

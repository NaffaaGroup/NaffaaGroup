using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The card type
/// </summary>
public enum CardType
{
    Club,
    Diamond,
    Heart,
    Spade,
    Joker
}

[CreateAssetMenu(fileName ="Card", menuName ="Scriptable Objects/Card")]
public class Card : ScriptableObject
{
    public int cardNumber;
    public CardType cardType;
    public Sprite cardImage;
    public int cardValue;
    public int orderType;
    public string ID;
}

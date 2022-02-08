using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TypesAssets : MonoBehaviour
{
    public static TypesAssets Instance;

    // Start is called before the first frame update
    [Header("0 = Spade, 1 = Diamond, 2 = Heart, 3 = Club")]
    public Sprite[] Types;
    private Image img;
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

    // Update is called once per frame
    public void Hide(int PlayerNumber)
    {
        transform.GetChild(PlayerNumber).GetComponent<Image>().enabled = false;
        transform.GetChild(PlayerNumber).GetChild(0).GetComponent<Image>().sprite = null;
        transform.GetChild(PlayerNumber).GetChild(1).GetComponent<Text>().text = null;
    }
    public void Show(int PlayerNumber, int Type, int Value)
    {
        transform.GetChild(PlayerNumber).GetComponent<Image>().enabled = true;
        transform.GetChild(PlayerNumber).GetChild(0).GetComponent<Image>().sprite = Types[Type];
        transform.GetChild(PlayerNumber).GetChild(1).GetComponent<Text>().text = Value.ToString();
    }
    public void ShowCard(int PlayerNumber)
    {
        transform.GetChild(PlayerNumber).GetComponent<Image>().enabled = true;
    }
    public void ChangeText(string score, int PlayerNumber)
    {
        transform.GetChild(PlayerNumber).GetChild(1).GetComponent<Text>().text = score.ToString();
    }
    public void ShowType(int type, int PlayerNumber)
    {
        transform.GetChild(PlayerNumber).GetChild(0).gameObject.SetActive(true);
        img = transform.GetChild(PlayerNumber).GetChild(0).GetComponent<Image>();
        img.sprite = Types[type];
    }
    public Text getScoreObject(int playerNumber)
    {
        return transform.GetChild(playerNumber).GetChild(1).gameObject.GetComponent<Text>();
    }
    void Update()
    {
        //
    }
}

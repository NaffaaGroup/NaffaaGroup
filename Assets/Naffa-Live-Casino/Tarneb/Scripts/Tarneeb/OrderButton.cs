using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Tarneb41.Scripts{

public class OrderButton : MonoBehaviour
{
    //button text
    [SerializeField] private int buttonNumber;
    public Color buttonColor;
    // Start is called before the first frame update
    void Start()
    {
        //get first child
        Transform child = transform.GetChild(0);
        this.gameObject.GetComponent<Image>().color = buttonColor;
        child.gameObject.GetComponent<Text>().color = Color.white;

        if (buttonNumber == 0)
        {
            child.gameObject.GetComponent<Text>().text = "Pass";
            child.gameObject.GetComponent<Text>().fontSize = 16;
        }
        else
        {
            child.gameObject.GetComponent<Text>().text = buttonNumber.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Order(int order)
    {
        //loop in all button has OrderButton script
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            if (transform.parent.GetChild(i).GetComponent<OrderButton>().buttonNumber <= order && transform.parent.GetChild(i).GetComponent<OrderButton>().buttonNumber != 0)
            {
                transform.parent.GetChild(i).GetComponent<Button>().interactable = false;
                transform.parent.GetChild(i).GetComponent<Image>().color = new Color32(163, 163, 163, 255);
                Debug.Log("Button number: " + transform.parent.GetChild(i).GetComponent<OrderButton>().buttonNumber);
            }
        }


        if (order == 0)
        {
            if (HandManager.instance.crrentPlayerID.PlayerOrder.text != "Pass")
            {
                RoundController.instance.orderCount++;
            }
            TypesAssets.Instance.ChangeText("Pass", HandManager.instance.crrentPlayerID.playerNumber);
            if (HandManager.instance.crrentPlayerID.isOrder == false)
            {
                SoundSystem.Instance.GetComponent<AudioSource>().clip = SoundSystem.Instance.Numbersclips[0];
                SoundSystem.Instance.GetComponent<AudioSource>().Play();
            }
            HandManager.instance.crrentPlayerID.isOrder = true;
        }
        else
        {
            TypesAssets.Instance.ChangeText(order.ToString(), HandManager.instance.crrentPlayerID.playerNumber);
            RoundController.instance.TableOrder = order;

        }

        if (HandManager.instance.crrentPlayerID.isOrder == false)
        {
            SoundSystem.Instance.GetComponent<AudioSource>().clip = SoundSystem.Instance.Numbersclips[order - 1];
            SoundSystem.Instance.GetComponent<AudioSource>().Play();
        }
        HandManager.instance.crrentPlayerID.orderNumber = order;
        HandManager.instance.crrentPlayerID.GetComponent<TimerController>().EndPlayerTurn();

        HideModel();

    }
    public void HideModel()
    {
        transform.parent.gameObject.SetActive(false);
    }
}
}
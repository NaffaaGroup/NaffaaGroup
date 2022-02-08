using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartnerSystem : MonoBehaviour
{
    // Start is called before the first frame update
    public static PartnerSystem instance { get; private set; }
    public GameObject PlayersContainer;
    public GameObject ParnterContainer;
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
        
    }
    public IEnumerator Distrbute()
    {
        yield return new WaitForSeconds(1f);
        foreach (PlayerHandler player in HandManager.instance.players)
        {
            if (player.tag != "MainPlayer")
            {
                GameObject _newPartner = Instantiate(ParnterContainer, PlayersContainer.transform);
                //add HandManager.isntance.ChoosePartner to button onclick
                _newPartner.GetComponent<Button>().onClick.AddListener(delegate { HandManager.instance.ChoosePartner(player); });
                _newPartner.GetComponent<PartnerData>().PlayerName.text = player.playerName;
                _newPartner.GetComponent<PartnerData>().PlayerImage.sprite =player.playerImage;
            }
        }
    }
    public void RemovePartnersListners()
    {
        foreach (Transform _button in PlayersContainer.transform)
        {
         Destroy(_button.GetComponent<Button>());
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}

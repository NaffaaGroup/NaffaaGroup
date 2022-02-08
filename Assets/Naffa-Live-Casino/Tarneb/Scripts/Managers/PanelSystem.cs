using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelSystem : MonoBehaviour
{
    public static PanelSystem instance { get; private set; }

    public GameObject ResultPanel;
    public GameObject PartnerPanel;
    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void showPanel(string panelName)
    {
        switch (panelName)
        {
            case "ResultPanel":
                ResultPanel.GetComponents<Animator>()[0].SetBool("isShow", true);
                break;
            case "PartnerPanel":
                PartnerPanel.GetComponents<Animator>()[0].SetBool("TurnOn", true);
                break;
        }
    }
    public void hidePanel(string panelName)
    {
        switch (panelName)
        {
            case "ResultPanel":
                ResultPanel.GetComponents<Animator>()[0].SetBool("isShow", false);
                break;
            case "PartnerPanel":
                PartnerPanel.GetComponents<Animator>()[0].SetBool("TurnOn", false);
                break;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public struct BotData
{
    public string name;
    public enum gender { male, female }
    public gender genderName;
    public Sprite image;
}
public class NamesSystem : MonoBehaviour
{

    public static NamesSystem Instance;
    [Header("This Contains the Data of the Bot Players")]

    public BotData[] datas;
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
    public BotData GetRandomData()
    {
        try
        {
            return datas[Random.Range(0, datas.Length)];
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
            return datas[0];
        }
    }
    void Update()
    {


    }
}

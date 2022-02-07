using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSystem : MonoBehaviour
{
    public AudioClip[] Numbersclips;
    public AudioClip[] CardType;
    public AudioClip ShuffleSound;

    public AudioClip TableHard;

    public static SoundSystem Instance { get; private set; }
    void Awake()
    {
       if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

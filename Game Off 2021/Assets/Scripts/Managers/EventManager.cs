using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [Header("References")]
    public static EventManager instance;

    [Header("Event delegates")]
    public bool bruh;
    public delegate void PlayerDead();
    public static event PlayerDead PlayerDied;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CallPlayerDead()
    {
        if (PlayerDied != null) PlayerDied();
    }
}

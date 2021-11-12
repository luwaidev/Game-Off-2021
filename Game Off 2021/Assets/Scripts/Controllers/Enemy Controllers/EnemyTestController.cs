using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTestController : MonoBehaviour, EnemyInterface
{
    [Header("References")]
    [Header("Health Settings")]
    float thing;
    public int health { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        EventManager.PlayerDied += OnPlayerDead;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnPlayerDead()
    {

    }
    public void OnHit(int damage)
    {

    }
}

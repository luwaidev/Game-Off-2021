using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider)
    {
        GameManager.instance.spawnPosition = transform.position;
        GetComponent<Animator>().Play("Respawn Set");
    }
}

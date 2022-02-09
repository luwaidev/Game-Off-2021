using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Player" && GameManager.instance.spawnPosition != (Vector2)transform.position)
        {
            GameManager.instance.spawnPosition = transform.position;
            GetComponent<Animator>().Play("Respawn Set");

            PlayerController.instance.health = HealthController.instance.playerMaxHealth;
        }
    }
}

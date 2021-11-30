using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcicleController : MonoBehaviour
{
    public float icicleSpeed;
    public int damage;

    private void Start()
    {
        GetComponent<Rigidbody2D>().velocity = icicleSpeed * transform.up;
        damage = PlayerController.instance.icicleDamage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.isTrigger && other.tag == "Enemy")
        {
            other.GetComponent<EnemyInterface>().OnHit(damage);
            Destroy(gameObject);
        }
        else if (other.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }


}
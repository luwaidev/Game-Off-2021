using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcicleController : MonoBehaviour
{
    public float icicleSpeed;
    public int damage;

    private void Start()
    {
        GetComponent<Rigidbody2D>().velocity = icicleSpeed * Vector2.right;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.isTrigger && other.tag == "Enemy")
        {
            other.GetComponent<EnemyInterface>().OnHit(damage);
        }
    }


}
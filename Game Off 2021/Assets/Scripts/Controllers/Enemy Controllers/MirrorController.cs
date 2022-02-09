using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorController : MonoBehaviour, EnemyInterface
{

    private Animator anim;
    public bool isAutomatic;

    public float attackTime;
    public float timer;
    public float offset;
    public int health { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (offset >= 0)
        {
            offset -= Time.deltaTime;
        }
        else if (!isAutomatic)
        {
            timer += Time.deltaTime;
            if (timer > attackTime)
            {
                anim.SetTrigger("Attack");
                timer = 0;
            }
        }
    }
    public void OnHit(int damage)
    {

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && isAutomatic)
        {
            anim.SetTrigger("Attack");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeController : MonoBehaviour
{

    public int type;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && !GetComponent<Animator>().GetBool("Collected"))
        {
            GetComponent<AudioSource>().Play();
            GetComponent<Animator>().SetBool("Collected", true);
            UpgradeManager.instance.Upgrade(type);
        }
    }
}

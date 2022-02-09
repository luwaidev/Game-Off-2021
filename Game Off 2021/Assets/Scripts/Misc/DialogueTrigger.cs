using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("References")]
    public Animator anim;
    public GameObject dialogue;
    public bool showing;


    public bool unlockMelee;
    public bool unlockDash;
    public bool unlockIcicle;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (anim != null && showing && !TextDisplay.instance.shown)
        {
            anim.SetTrigger("Hide");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != "Player" || showing) return;

        TextDisplay.instance.currentDialogue = dialogue;
        StartCoroutine(TextDisplay.instance.DisplayText());

        showing = true;
        if (anim != null) anim.SetTrigger("Show");

        TextDisplay.instance.unlockMelee = unlockMelee;
        TextDisplay.instance.unlockDash = unlockDash;
        TextDisplay.instance.unlockIcicle = unlockIcicle;

        PlayerController.instance.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        PlayerController.instance.GetComponent<Animator>().Play("Player " + "Idle " + PlayerController.instance.direction); // Play animation
    }
}

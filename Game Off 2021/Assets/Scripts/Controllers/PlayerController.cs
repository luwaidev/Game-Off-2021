using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public static PlayerController instance;
    [SerializeField] BoxCollider2D attackDirection;
    private Rigidbody2D rb;
    private Animator anim;

    [Header("State")]
    public List<Action> abilities;
    public Vector2 input;
    public Vector2 mousePosition;
    public Vector2 velocity;
    [SerializeField] bool sprinting;

    [Header("Movement Settings")]
    [SerializeField] float movementSpeed;
    [SerializeField] float sprintSpeed;

    // Set References
    private void Awake()
    {
        if (instance != null) instance = this;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        abilities = new List<Action>();
    }

    // Main Loop
    void Update()
    {
        SetState();

    }

    void SetState()
    {
        sprinting = Input.GetKey(KeyCode.LeftShift);
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

    }


    public void PlayerMovement()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        velocity = input * (sprinting ? movementSpeed : sprintSpeed); // Set velocity to regular or sprint speed

        rb.velocity = velocity;
    }

    void SetDirection()
    {
        attackDirection.transform.position = ((Vector2)transform.position - mousePosition).normalized;
    }
    // TODO Figure out how to use animations by just setting states
    void Animations()
    {

    }

    ////////////////////////////////////////////////////////////////
    //                        Abilities                           //
    ////////////////////////////////////////////////////////////////

    public void Dash()
    {

    }
}

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
    private Vector2 input;
    private bool sprinting;
    public Vector2 velocity;

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
        SetDirection();
    }

    void FixedUpdate()
    {
        foreach (Action ability in abilities)
        {
            ability.Invoke();
        }

        rb.velocity = velocity;
    }

    public void OnMove(InputValue value)
    {
        input = value.Get<Vector2>();

        velocity = input * (sprinting ? sprintSpeed : movementSpeed); // Set velocity to regular or sprint speed
    }

    public void OnSprint(InputValue value)
    {
        // sprinting = value.GetValue<bool>();
        print(value.Get<float>());

        velocity = input * (sprinting ? sprintSpeed : movementSpeed); // Set velocity to regular or sprint speed

    }

    void SetDirection()
    {
        var mouse = Mouse.current;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(mouse.position.ReadValue());
        attackDirection.transform.localPosition = (mousePosition - (Vector2)transform.position).normalized;
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

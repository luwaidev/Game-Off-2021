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
    public bool movementLocked;
    public bool dashing;
    public string direction;
    public Vector2 velocity;

    [Header("Movement Settings")]
    [SerializeField] float movementSpeed;

    [Header("Dash Settings")]
    public bool dashed;
    [SerializeField] float dashSpeed;
    [SerializeField] float dashTime;

    // Set References
    private void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        abilities = new List<Action>();
        abilities.Add(Dash);
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

        Animations();

        rb.velocity = velocity;

        ResetValues();
    }

    void ResetValues()
    {
        dashing = false;
    }

    public void OnMove(InputValue value)
    {
        input = value.Get<Vector2>();

        // Set current facing direction
        if (input.magnitude != 0 && Mathf.Abs(input.x) >= Mathf.Abs(input.y))
        {
            direction = (input.x >= 0) ? "Right" : "Left";
        }
        else
        {
            direction = (input.y >= 0) ? "Up" : "Down";
        }

        if (!movementLocked) velocity = input * movementSpeed; // Set velocity to regular or sprint speed
    }

    public void OnSprint(InputValue value)
    {
        // sprinting = value.GetValue<bool>();
        print(value.Get<float>());

        // velocity = input * (sprinting ? sprintSpeed : movementSpeed); // Set velocity to regular or sprint speed

    }

    public void OnDash(InputValue value)
    {
        dashing = dashed = !dashed;
    }
    public void OnFire(InputValue value)
    {

    }

    void SetDirection()
    {
        var mouse = Mouse.current;
        Vector2 mousePositionToPlayer = ((Vector2)Camera.main.ScreenToWorldPoint(mouse.position.ReadValue()) - (Vector2)transform.position).normalized;
        attackDirection.transform.localPosition = mousePositionToPlayer; // Set position 

        attackDirection.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(mousePositionToPlayer.y, mousePositionToPlayer.x)) * Mathf.Rad2Deg;
    }

    void Animations()
    {
        anim.Play("Player " + (rb.velocity.magnitude > 0.01f ? "Walk " : "Idle ") + direction, 0);
    }

    ////////////////////////////////////////////////////////////////
    //                        Abilities                           //
    ////////////////////////////////////////////////////////////////

    public void Dash()
    {
        if (dashing && !movementLocked) StartCoroutine(DashCoroutine());
    }

    IEnumerator DashCoroutine()
    {
        velocity = input * dashSpeed;
        movementLocked = true;
        dashed = true;

        yield return new WaitForSeconds(dashTime);

        velocity = input * movementSpeed;
        dashing = false;
        movementLocked = false;
    }


}

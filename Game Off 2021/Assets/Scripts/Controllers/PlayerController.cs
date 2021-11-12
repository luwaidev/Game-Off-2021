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
    [SerializeField] LayerMask enemyLayer;
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
    [SerializeField] float dashSpeed;
    [SerializeField] float dashTime;

    [Header("Combat Settings")]
    public int meleeDamage;

    [Header("Cutscene Settings")]
    [SerializeField] float loadInWaitTime;
    [SerializeField] float loadInWalkTime;
    [SerializeField] float loadInAfterTime;
    [SerializeField] float loadInMovementSpeed;


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
    //                       Input Calls                          //
    ////////////////////////////////////////////////////////////////

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
        dashing = true;
    }

    public void OnFire(InputValue value)
    {
        // Check melee hit
        RaycastHit2D[] enemiesInRange = Physics2D.BoxCastAll(
            attackDirection.bounds.center,
            attackDirection.bounds.size,
            attackDirection.transform.eulerAngles.z,
            attackDirection.transform.localPosition,
            0.01f, enemyLayer);

        for (int i = 0; i < enemiesInRange.Length; i++)
        {
            enemiesInRange[i].collider.GetComponent<EnemyInterface>().OnHit(meleeDamage);
        }
    }

    ////////////////////////////////////////////////////////////////
    //                     Cutscene Events                        //
    ////////////////////////////////////////////////////////////////

    public IEnumerator OnSceneLoad(Vector2 direction)
    {
        movementLocked = true;
        yield return new WaitForSeconds(loadInWaitTime);
        rb.velocity = direction * loadInMovementSpeed;
        yield return new WaitForSeconds(loadInWalkTime);
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(loadInAfterTime);
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

        yield return new WaitForSeconds(dashTime);

        velocity = input * movementSpeed;
        dashing = false;
        movementLocked = false;
    }


}

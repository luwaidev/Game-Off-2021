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
    private string direction = "Left";
    private string movementDirection = "Left";
    public Vector2 velocity;

    [Header("Movement Settings")]
    [SerializeField] float movementSpeed;
    [SerializeField] float reverseMovementSpeed;

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
        // Invoke active abilities
        foreach (Action ability in abilities)
        {
            ability.Invoke();
        }

        Animations(); // Set player animations

        rb.velocity = velocity; // Set velocity

        ResetValues(); // Reset values
    }

    void ResetValues()
    {
        dashing = false;
    }
    void SetDirection()
    {

        var mouse = Mouse.current; // Get mouse
        Vector2 mousePositionToPlayer = ((Vector2)Camera.main.ScreenToWorldPoint(mouse.position.ReadValue()) - (Vector2)transform.position).normalized; // Get mouse positions
        attackDirection.transform.localPosition = mousePositionToPlayer; // Set position 

        attackDirection.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(mousePositionToPlayer.y, mousePositionToPlayer.x)) * Mathf.Rad2Deg; // Set angle of object

        // Set current facing direction
        if (Mathf.Abs(mousePositionToPlayer.x) >= Mathf.Abs(mousePositionToPlayer.y))
        {
            direction = (mousePositionToPlayer.x >= 0) ? "Right" : "Left";
        }
        else
        {
            direction = (mousePositionToPlayer.y >= 0) ? "Up" : "Down";
        }

    }

    void Animations()
    {
        // Check if walking in opposite direction
        bool oppositeDirection = (movementDirection == "Right" && direction == "Left") || (movementDirection == "Left" && direction == "Right");
        anim.Play("Player " + (rb.velocity.magnitude > 0.01f ? "Walk " : "Idle ") + direction + (oppositeDirection ? "Back" : ""), 0); // Play animation
    }


    ////////////////////////////////////////////////////////////////
    //                       Input Calls                          //
    ////////////////////////////////////////////////////////////////

    public void OnMove(InputValue value)
    {
        input = value.Get<Vector2>(); // Get Vector2 input

        // Set current facing direction
        if (Mathf.Abs(input.x) >= Mathf.Abs(input.y))
        {
            movementDirection = (input.x >= 0) ? "Right" : "Left";
        }
        else
        {
            movementDirection = (input.y >= 0) ? "Up" : "Down";
        }

        // Check if walking in opposite direction
        bool oppositeDirection = (movementDirection == "Right" && direction == "Left") || (movementDirection == "Left" && direction == "Right") ||
                                 (movementDirection == "Up" && direction == "Down") || (movementDirection == "Down" && direction == "Up");
        if (!movementLocked) velocity = input * (oppositeDirection ? reverseMovementSpeed : movementSpeed); // Set velocity to regular or reversing speed
    }

    // Called when the mouse is moved
    public void OnMouseMove(InputValue value)
    {
        // Check if the player is opposite to the mouse
        bool oppositeDirection = (movementDirection == "Right" && direction == "Left") || (movementDirection == "Left" && direction == "Right") ||
                                 (movementDirection == "Up" && direction == "Down") || (movementDirection == "Down" && direction == "Up");
        if (!movementLocked) velocity = input * (oppositeDirection ? reverseMovementSpeed : movementSpeed); // Set velocity to regular or reversing speed
    }

    // Called when the dash button is pressed
    public void OnDash(InputValue value)
    {
        // Indicate dashing
        dashing = true;
    }

    // Called when fire button is pressed
    public void OnFire(InputValue value)
    {
        // Check melee hit
        RaycastHit2D[] enemiesInRange = Physics2D.BoxCastAll(
            attackDirection.bounds.center,
            attackDirection.bounds.size,
            attackDirection.transform.eulerAngles.z,
            attackDirection.transform.localPosition,
            0.01f, enemyLayer);

        // Iterate through enemies and do damage to them
        for (int i = 0; i < enemiesInRange.Length; i++)
        {
            enemiesInRange[i].collider.GetComponent<EnemyInterface>().OnHit(meleeDamage);
        }
    }

    ////////////////////////////////////////////////////////////////
    //                      Cutscene Events                       //
    ////////////////////////////////////////////////////////////////

    // Walk out of darkness 
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

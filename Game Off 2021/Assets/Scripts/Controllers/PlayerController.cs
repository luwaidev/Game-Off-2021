using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;


public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public static PlayerController instance;
    [SerializeField] BoxCollider2D arm;
    [SerializeField] Animator armAnim;
    [SerializeField] SpriteRenderer armSR;
    [SerializeField] LayerMask enemyLayer;
    private BoxCollider2D bc;
    private Rigidbody2D rb;
    private Animator anim;

    [Header("State")]
    public int health;
    public List<Action> abilities;
    private Vector2 input;
    private bool sprinting;
    public bool movementLocked;
    public bool dashed;
    public string direction = "Left";
    public string movementDirection = "Left";
    public Vector2 velocity;
    public Vector2 addedVelocity;

    [Header("Movement Settings")]
    [SerializeField] float movementSpeed;
    [SerializeField] float reverseMovementSpeed;

    [Header("Dash Settings")]
    [SerializeField] float dashSpeed;
    [SerializeField] float dashTime;

    [Header("Combat Settings")]
    public bool attacking;
    [SerializeField] float attackVelocity;
    [SerializeField] float meleeTime;
    private bool attackedAgain;
    public int meleeDamage;
    public float mouseDistance;
    public float stunTime;
    public float knockbackSpeed;

    [Header("Shooting Settings")]
    public Transform firePosition;
    public bool shootIcicle;
    public int icicleDamage;
    public float icicleTime;
    public GameObject icicle;

    [Header("Cutscene Settings")]
    [SerializeField] float loadInWaitTime;
    [SerializeField] float loadInWalkTime;
    [SerializeField] float loadInAfterTime;
    [SerializeField] float loadInMovementSpeed;

    [Header("Audio")]
    [SerializeField] AudioClip[] runningClips;
    [SerializeField] AudioSource runningSource;
    [SerializeField] AudioSource meleeAttackClip;


    // Set References
    private void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        bc = GetComponent<BoxCollider2D>();
        armAnim = arm.gameObject.GetComponent<Animator>();
        armSR = arm.gameObject.GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        abilities = new List<Action>();
        abilities.Add(Dash);
        StartCoroutine(OnSceneLoad());
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

        CheckHit();
        if (!movementLocked) Animations(); // Set player animations

        rb.velocity = velocity + addedVelocity; // Set velocity

        Audio(); // Set audio
        ResetValues();
    }

    void ResetValues()
    {
        shootIcicle = false;
        dashed = false;
    }

    void Audio()
    {
        if (!runningSource.isPlaying && !attacking && velocity != Vector2.zero)
        {
            runningSource.clip = runningClips[Random.Range(0, runningClips.Length)];
            runningSource.Play();
        }
    }
    void SetDirection()
    {

        var mouse = Mouse.current; // Get mouse
        Vector2 mousePositionToPlayer = ((Vector2)Camera.main.ScreenToWorldPoint(mouse.position.ReadValue()) - (Vector2)transform.position).normalized * mouseDistance; // Get mouse positions

        // Set current facing direction
        if (Mathf.Abs(mousePositionToPlayer.x) >= Mathf.Abs(mousePositionToPlayer.y))
        {
            direction = (mousePositionToPlayer.x >= 0) ? "Right" : "Left";
        }
        else
        {
            direction = (mousePositionToPlayer.y >= 0) ? "Up" : "Down";
        }

        if (attacking)
        {
            arm.transform.localPosition = mousePositionToPlayer; // Set position 

            arm.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(mousePositionToPlayer.y, mousePositionToPlayer.x) - Mathf.PI / 2) * Mathf.Rad2Deg; // Set angle of object

            armSR.sortingOrder = (Mathf.Abs(mousePositionToPlayer.y) > Mathf.Abs(mousePositionToPlayer.x) && mousePositionToPlayer.y < 0) ? 1 : -1;
        }
        else
        {
            arm.transform.localPosition = Vector2.zero;
            float angle = (Mathf.Abs(mousePositionToPlayer.x) >= Mathf.Abs(mousePositionToPlayer.y) ? (mousePositionToPlayer.x >= 0 ? -90 : 90) : (mousePositionToPlayer.y >= 0 ? 0 : 180));
            arm.transform.eulerAngles = new Vector3(0, 0, angle); // Set angle of object
        }


    }

    void CheckHit()
    {
        RaycastHit2D hit = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0f, Vector2.down, 0.2f, enemyLayer);
        if (hit && !movementLocked)
        {
            velocity = -hit.collider.transform.position.normalized * knockbackSpeed;

            StartCoroutine(Hit());
        }
    }

    IEnumerator Hit()
    {
        anim.Play("Player Hit " + direction); // Play animation
        movementLocked = true;
        health -= 1;
        yield return new WaitForSeconds(stunTime);
        movementLocked = false;
        velocity = Vector2.zero;

        if (health <= 0) { Dead(); }
    }

    void Animations()
    {
        // Check if walking in opposite direction
        bool oppositeDirection = rb.velocity.magnitude > 0.01f && ((movementDirection == "Right" && direction == "Left") || (movementDirection == "Left" && direction == "Right"));
        anim.Play("Player " + (rb.velocity.magnitude > 0.01f ? "Walk " : "Idle ") + direction + (oppositeDirection ? " Back" : "")); // Play animation
    }

    void Dead()
    {
        movementLocked = true;
        arm.gameObject.SetActive(false);
        anim.SetBool("Dead", true);
        GameManager.instance.Load("Game");
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
        if (!movementLocked) dashed = value.Get<float>() == 1;
    }

    // Called when fire button is pressed
    public void OnFire(InputValue value)
    {
        if (!attacking && !movementLocked) StartCoroutine(MeleeAttackCoroutine());
        else attackedAgain = true;
    }

    // Called when fire button is pressed
    public void OnShoot(InputValue value)
    {
        if (!attacking && !movementLocked) StartCoroutine(MeleeAttackCoroutine());
        else attackedAgain = true;
    }

    ////////////////////////////////////////////////////////////////
    //                      Cutscene Events                       //
    ////////////////////////////////////////////////////////////////

    // Walk out of darkness 
    public IEnumerator OnSceneLoad()
    {
        anim.Play("Player Wake");
        movementLocked = true;
        Camera.main.GetComponent<CameraController>().inCutscene = true;
        arm.gameObject.SetActive(false);
        yield return new WaitForSeconds(loadInWaitTime);
        arm.gameObject.SetActive(true);
        Camera.main.GetComponent<CameraController>().inCutscene = false;
        movementLocked = false;
        attackedAgain = false;
    }

    ////////////////////////////////////////////////////////////////
    //                        Abilities                           //
    ////////////////////////////////////////////////////////////////

    public void OnMeleeAttack()
    {
        // Check melee hit
        RaycastHit2D[] enemiesInRange = Physics2D.BoxCastAll(
            arm.bounds.center,
            arm.bounds.size,
            arm.transform.eulerAngles.z,
            arm.transform.localPosition,
            0.01f, enemyLayer);

        // Iterate through enemies and do damage to them
        for (int i = 0; i < enemiesInRange.Length; i++)
        {
            if (!enemiesInRange[i].collider.isTrigger) enemiesInRange[i].collider.GetComponent<EnemyInterface>().OnHit(meleeDamage);
        }
    }

    IEnumerator MeleeAttackCoroutine()
    {
        attacking = true;
        armAnim.Play("Arm Melee 1");

        movementLocked = true;
        var mouse = Mouse.current; // Get mouse
        Vector2 mousePositionToPlayer = ((Vector2)Camera.main.ScreenToWorldPoint(mouse.position.ReadValue()) - (Vector2)transform.position).normalized * mouseDistance; // Get mouse positions
        velocity = attackVelocity * mousePositionToPlayer;

        yield return new WaitForSeconds(meleeTime / 2);
        meleeAttackClip.Play();
        attackedAgain = false;
        OnMeleeAttack();
        yield return new WaitForSeconds(meleeTime / 2);

        if (attackedAgain)
        {
            armAnim.Play("Arm Melee 2");
            yield return new WaitForSeconds(meleeTime / 2);
            meleeAttackClip.Play();
            attackedAgain = false;
            OnMeleeAttack();
            yield return new WaitForSeconds(meleeTime / 2);
        }


        velocity = Vector2.zero;
        armAnim.Play("Arm Idle");

        attacking = false;
        movementLocked = false;
    }
    public void Shoot()
    {

        if (shootIcicle && !movementLocked) StartCoroutine(ShootCoroutine());
        shootIcicle = false;
    }
    IEnumerator ShootCoroutine()
    {
        attacking = true;
        armAnim.Play("Arm Shoot");

        movementLocked = true;

        Instantiate(icicle, firePosition.position, firePosition.rotation);

        yield return new WaitForSeconds(icicleTime);
        movementLocked = false;
        attacking = false;

    }
    public void Dash()
    {

        if (dashed && !movementLocked) StartCoroutine(DashCoroutine());
    }

    IEnumerator DashCoroutine()
    {
        dashed = false;
        velocity = input * dashSpeed;
        movementLocked = true;

        yield return new WaitForSeconds(dashTime);

        velocity = input * movementSpeed;
        movementLocked = false;
    }


}

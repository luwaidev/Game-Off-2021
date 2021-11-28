using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryController : MonoBehaviour, EnemyInterface
{
    public enum State
    {
        Idle,
        Firing,
        Hit,
        Die,
    }

    [Header("References")]
    private Rigidbody2D rb;
    private Animator anim;
    private List<Transform> nearbyEnemies = new List<Transform>();

    [Header("Health Settings")]
    float shield;
    public int health { get; set; }

    [Header("State Settings")]
    public State state;
    private Vector2 velocity;

    [Header("Attack Settings")]
    public float attackTime;
    public float aggroDistance;
    public float minDistance;
    public GameObject projectile;
    public GameObject target;
    public Sprite dropSprite;

    [Header("Projectile Settings")]
    public float warningTime;
    public float projectileSpeed;
    public Transform firePosition;

    [Header("Hit settings")]
    [SerializeField] float hitTime;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minDistance);
    }

    ////////////////////////////////////////////////////////////////
    //                           States                           //
    ////////////////////////////////////////////////////////////////
    void NextState()
    {
        string methodName = state.ToString() + "State";

        // Get method
        System.Reflection.MethodInfo info =
            GetType().GetMethod(methodName,
                                System.Reflection.BindingFlags.NonPublic |
                                System.Reflection.BindingFlags.Instance);

        StartCoroutine((IEnumerator)info.Invoke(this, null)); // Call the next state
    }
    IEnumerator IdleState()
    {


        while (state == State.Idle)
        {
            if (Vector2.Distance(transform.position, PlayerController.instance.transform.position) < aggroDistance)
            {
                state = State.Firing;
            }
            yield return 0;
        }
        // state = State.Follow;
        NextState();
    }

    IEnumerator FiringState()
    {
        anim.Play("Artillery Fire");

        yield return new WaitForSeconds(0.3f);

        Rigidbody2D p = Instantiate(projectile, firePosition.position, Quaternion.identity).GetComponent<Rigidbody2D>();
        p.velocity = Vector2.up * projectileSpeed;
        yield return new WaitForSeconds(attackTime);

        Vector2 position = PlayerController.instance.transform.position;
        p.transform.position = new Vector2(position.x, p.transform.position.y);
        p.velocity = Vector2.zero;
        Animator t = Instantiate(target, position, Quaternion.identity).GetComponent<Animator>();
        yield return new WaitForSeconds(warningTime);

        p.velocity = Vector2.down * projectileSpeed;
        while (Vector2.Distance(p.position, t.transform.position) > 0.25f) yield return null;
        Destroy(p.gameObject);

        t.Play("Target Hit");
        state = (Vector2.Distance(PlayerController.instance.transform.position, transform.position) < aggroDistance) ?
            State.Firing : State.Idle;


        yield return new WaitForSeconds(warningTime / 2);
        NextState();
    }

    IEnumerator HitState()
    {

        yield return new WaitForSeconds(hitTime);

        state = State.Firing;
        NextState();
    }

    IEnumerator DieState()
    {
        Debug.Log("Die: Enter");
        while (state == State.Die)
        {

            yield return 0;
        }
        Debug.Log("Die: Exit");
    }

    ////////////////////////////////////////////////////////////////
    //                         Functions                          //
    ////////////////////////////////////////////////////////////////
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        NextState();
    }

    private void FixedUpdate()
    {
        rb.velocity = velocity;
    }


    public void OnHit(int damage)
    {
        state = State.Hit;
        health -= damage;
    }

}

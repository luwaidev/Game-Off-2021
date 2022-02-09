using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmController : MonoBehaviour, EnemyInterface
{
    public enum State
    {
        Load,
        Idle,
        Follow,
        Hit,
        Die,
    }

    [Header("References")]
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    public List<Transform> nearbyEnemies = new List<Transform>();
    public int health { get; set; }

    [Header("State Settings")]
    public State state;
    private Vector2 velocity;

    [SerializeField] float movementSpeed;
    [SerializeField] float aggroDistance;

    [Header("Hit settings")]
    [SerializeField] float deathTime;
    [SerializeField] float hitTime;
    [Header("Repel Settings")]
    [SerializeField] float maxRepel;
    [SerializeField] float repelFalloff;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, aggroDistance);
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
    IEnumerator LoadState()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        anim.enabled = false;
        sr.color = new Color(1f, 1f, 1f, 0f);
        GetComponent<BoxCollider2D>().enabled = false;

        for (int i = 0; i < 50; i++)
        {
            sr.color = new Color(1, 1, 1, i / 50f);
            yield return new WaitForSeconds(0.01f);
        }
        state = State.Idle;
        anim.enabled = true;
        GetComponent<BoxCollider2D>().enabled = true;
        NextState();
    }

    IEnumerator IdleState()
    {
        while (state == State.Idle)
        {
            velocity = Vector2.zero;
            if (Vector2.Distance(transform.position, PlayerController.instance.transform.position) < aggroDistance && PlayerController.instance.armAnim.gameObject.activeInHierarchy)
            {
                state = State.Follow;
            }
            yield return null;
        }
        NextState();
    }
    IEnumerator FollowState()
    {
        yield return new WaitForEndOfFrame();
        while (state == State.Follow)
        {
            if (!PlayerController.instance.armAnim.gameObject.activeInHierarchy) state = State.Idle;
            velocity = (PlayerController.instance.transform.position - transform.position).normalized * movementSpeed;
            SetAnimation();

            yield return null;
        }
        NextState();
    }

    IEnumerator DieState()
    {
        velocity = Vector2.zero;
        string direction = Mathf.Abs(rb.velocity.x) > Mathf.Abs(rb.velocity.y) ? " Side" : (rb.velocity.y > 0 ? " Up" : " Down");
        anim.Play("Swarm Death" + direction);

        yield return new WaitForSeconds(deathTime);
        Destroy(gameObject);
    }

    IEnumerator HitState()
    {
        velocity = Vector2.zero;
        transform.localScale = new Vector2(((rb.velocity.x < 0) ? -Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x)), transform.localScale.y);
        GetComponent<Animator>().Play("Swarm Hit");
        yield return new WaitForSeconds(hitTime);
        state = State.Follow;
        NextState();
    }

    ////////////////////////////////////////////////////////////////
    //                         Functions                          //
    ////////////////////////////////////////////////////////////////
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        NextState();
        health = 20;
        velocity = Vector2.zero;
    }

    void SetAnimation()
    {
        string direction = Mathf.Abs(rb.velocity.x) > Mathf.Abs(rb.velocity.y) ? " Side" : (rb.velocity.y > 0 ? " Up" : " Down");
        transform.localScale = new Vector2(((rb.velocity.x < 0 && direction == " Side") ? -Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x)), transform.localScale.y);

        anim.Play("Swarm " + (rb.velocity.magnitude > 0.4f ? "Run" : "Idle") + direction);
    }
    private void FixedUpdate()
    {
        if (nearbyEnemies.Count > 0) AvoidEnemies();
        rb.velocity = velocity;
    }

    void AvoidEnemies()
    {
        int closestEnemyIndex = 0;
        float closestDistance = Vector2.Distance(nearbyEnemies[0].position, transform.position);
        for (int i = 1; i < nearbyEnemies.Count; i++)
        {
            if (closestDistance < Vector2.Distance(nearbyEnemies[i].position, transform.position))
            {
                closestDistance = Vector2.Distance(nearbyEnemies[i].position, transform.position);
                closestEnemyIndex = i;
            }
        }

        Vector2 enemyToSelf = (nearbyEnemies[closestEnemyIndex].position - transform.position) * repelFalloff;
        velocity += new Vector2(Mathf.Sign(enemyToSelf.x) * (Mathf.Pow(Mathf.Abs(enemyToSelf.x), 1f / 3f) + maxRepel),
        Mathf.Sign(enemyToSelf.y) * (-Mathf.Pow(Mathf.Abs(enemyToSelf.y), 1f / 3f) + maxRepel));
    }

    public void OnHit(int damage)
    {
        health -= damage;
        GetComponent<Animator>().Play("Swarm Hit");
        state = State.Hit;
        if (health <= 0) state = State.Die;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            nearbyEnemies.Add(other.gameObject.transform);
        }

    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Enemy" && !nearbyEnemies.Contains(other.gameObject.transform))
        {
            nearbyEnemies.Add(other.gameObject.transform);
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Enemy") nearbyEnemies.Remove(other.gameObject.transform);
    }

}
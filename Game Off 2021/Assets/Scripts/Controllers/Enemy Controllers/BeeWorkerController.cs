using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeWorkerController : MonoBehaviour
{
    public enum State
    {
        Idle,
        Follow,
        Lead,
        Die,
    }

    [Header("References")]
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private List<Transform> nearbyEnemies = new List<Transform>();

    [Header("Health Settings")]
    float shield;
    public int health { get; set; }

    [Header("State Settings")]
    public State state;
    private Vector2 velocity;

    [Header("Follow Player")]
    public bool leader;
    public BeeWorkerController beeAhead;
    public Vector2 targetPosition;


    [Header("Leading Settings")]
    [SerializeField] float playerDistance;
    [SerializeField] float aggroDistance;
    [SerializeField] float movementSpeed;

    [Header("Attack Player")]
    [SerializeField] float attackDistance;
    [SerializeField] float attackDamage;
    [SerializeField] float attackVelocity;

    [Header("Hit settings")]
    [SerializeField] float hitTime;
    [SerializeField] float knockbackSpeed;
    [Header("Repel Settings")]
    [SerializeField] float maxRepel;
    [SerializeField] float repelFalloff;

    private void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroDistance);

        Gizmos.color = Color.gray;
        if (PlayerController.instance != null) Gizmos.DrawWireSphere(PlayerController.instance.transform.position, playerDistance);

        Gizmos.DrawSphere(targetPosition, 1);
    }

    ////////////////////////////////////////////////////////////////
    //                           States                           //
    ////////////////////////////////////////////////////////////////
    void NextState()
    {
        string methodName = state.ToString() + "State";
        print(methodName);

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
            if (leader)
            {
                if (Vector2.Distance(PlayerController.instance.transform.position, transform.position) < aggroDistance)
                {
                    state = State.Lead;
                }
            }
            else if (beeAhead.state != State.Idle)
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
        targetPosition = beeAhead.targetPosition;
        while (state == State.Follow)
        {
            if (Vector2.Distance(targetPosition, transform.position) < 0.1f)
            {
                targetPosition = beeAhead.targetPosition;
            }

            Vector2 target = targetPosition - (Vector2)transform.position;
            velocity = target.normalized * movementSpeed;

            if (beeAhead == null)
            {
                state = State.Lead;
                leader = true;
            }
            yield return null;
        }
        NextState();
    }

    IEnumerator LeadState()
    {
        // Randomise Position
        Vector2 playerPosition = PlayerController.instance.transform.position;
        targetPosition.x = playerPosition.x + Mathf.Sign(Random.Range(-1, 1)) * (-Mathf.Sqrt(-playerDistance * Random.Range(0, playerDistance) + Mathf.Pow(playerDistance, 2f)) + playerDistance);

        targetPosition.y = playerPosition.y + Mathf.Sign(Random.Range(-1, 1)) * (-Mathf.Sqrt(-playerDistance * Random.Range(0, playerDistance) + Mathf.Pow(playerDistance, 2f)) + playerDistance);

        while (state == State.Lead)
        {
            if (Vector2.Distance(targetPosition, transform.position) < 0.1f)
            {
                // Randomise Position
                playerPosition = PlayerController.instance.transform.position;
                targetPosition.x = playerPosition.x + Mathf.Sign(Random.Range(-1, 1)) * (-Mathf.Sqrt(-playerDistance * Random.Range(0, playerDistance) + Mathf.Pow(playerDistance, 2f)) + playerDistance);

                targetPosition.y = playerPosition.y + Mathf.Sign(Random.Range(-1, 1)) * (-Mathf.Sqrt(-playerDistance * Random.Range(0, playerDistance) + Mathf.Pow(playerDistance, 2f)) + playerDistance);

                if (Vector2.Distance(playerPosition, targetPosition) < attackDistance)
                {
                    targetPosition = playerPosition;
                }
            }

            Vector2 target = targetPosition - (Vector2)transform.position;
            velocity = target.normalized * movementSpeed;
            yield return null;
        }
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
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        NextState();
    }

    void SetDirection()
    {
        sr.flipX = velocity.x > 0 ? false : true;
    }
    private void FixedUpdate()
    {
        if (nearbyEnemies.Count > 0) AvoidEnemies();
        SetDirection();
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
        velocity += new Vector2(-Mathf.Pow(enemyToSelf.x, 1f / 3f), -Mathf.Pow(enemyToSelf.x, 1f / 3f)) + Vector2.one * maxRepel;
    }

    public void OnHit(int damage)
    {
        health -= damage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player") state = State.Follow;
        else if (other.tag == "Enemy") nearbyEnemies.Add(other.gameObject.transform);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Enemy") nearbyEnemies.Remove(other.gameObject.transform);
    }
}

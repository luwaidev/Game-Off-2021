using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTestController : MonoBehaviour, EnemyInterface
{
    public enum State
    {
        Patrol,
        Follow,
        Attack,
        Hit,
        Die,
    }

    [Header("References")]
    private Rigidbody2D rb;
    private Animator anim;
    private List<Transform> nearbyEnemies;

    [Header("Health Settings")]
    float shield;
    public int health { get; set; }

    [Header("State Settings")]
    public State state;
    private Vector2 velocity;

    [Header("Patrol Settings")]
    private Vector2 patrolCenter;
    [SerializeField] float patrolMaxDistance;
    [SerializeField] float patrolMargin;
    [SerializeField] float patrolWaitTime;
    [SerializeField] float patrolMovementSpeed;

    [Header("Follow Player")]

    [SerializeField] float disengageDistance;
    [SerializeField] float followMovementSpeed;
    [SerializeField] float followOffset;
    [SerializeField] float followOffsetTime;

    [Header("Repel Settings")]
    [SerializeField] float maxRepel;
    [SerializeField] float repelFalloff;


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
    IEnumerator PatrolState()
    {
        Debug.Log("Patrol: Enter");
        Vector2 patrolPosition = Vector2.zero;
        bool patrolling = false;

        while (state == State.Patrol)
        {
            if (patrolling)
            {
                velocity = (patrolPosition - (Vector2)transform.position).normalized * patrolMovementSpeed;

                patrolling = Vector2.Distance(transform.position, patrolPosition) < patrolMargin;
            }
            else
            {
                patrolPosition.x = patrolCenter.x + Mathf.Sign(Random.Range(-1, 1)) * (-Mathf.Sqrt(Random.Range(0, patrolMaxDistance)) + Mathf.Pow(patrolMaxDistance, 2f));

                patrolPosition.y = patrolCenter.y + Mathf.Sign(Random.Range(-1, 1)) * (-Mathf.Sqrt(Random.Range(0, patrolMaxDistance)) + Mathf.Pow(patrolMaxDistance, 2f));

                yield return new WaitForSeconds(patrolWaitTime);
            }

            yield return 0;
        }
        Debug.Log("Patrol: Exit");
        NextState();
    }

    IEnumerator FollowState()
    {
        Vector2 velocityOffset = Vector2.zero;
        float timeSinceVelocityChange = 0;

        Debug.Log("Follow: Enter");
        while (state == State.Follow)
        {
            // Set velocity offset
            timeSinceVelocityChange += Time.deltaTime;
            if (timeSinceVelocityChange >= followOffsetTime) velocityOffset = new Vector2(
                Random.Range(-followOffsetTime, followOffsetTime),
                Random.Range(-followOffsetTime, followOffsetTime));

            // Set velocity with offset
            velocity = (PlayerController.instance.transform.position - transform.position).normalized * followMovementSpeed;
            velocity += velocityOffset;

            // Check if player is out of range to follow
            float playerDistance = Vector2.Distance(PlayerController.instance.transform.position, transform.position);
            if (playerDistance > disengageDistance) state = State.Patrol;

            yield return 0;
        }
        Debug.Log("Follow: Exit");
        NextState();
    }

    IEnumerator AttackState()
    {
        Debug.Log("Attack: Enter");
        while (state == State.Attack)
        {
            yield return 0;
        }
        Debug.Log("Attack: Exit");
        NextState();
    }

    IEnumerator HitState()
    {
        Debug.Log("Hit: Enter");
        while (state == State.Hit)
        {
            yield return 0;
        }
        Debug.Log("Hit: Exit");
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
    }

    void Start()
    {
        NextState();
    }

    private void FixedUpdate()
    {
        AvoidEnemies();
        rb.velocity = velocity;
    }

    void AvoidEnemies()
    {
        int closestEnemyIndex = 0;
        float closestDistance = Vector2.Distance(transform.position, nearbyEnemies[0].position);
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
        state = State.Hit;
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

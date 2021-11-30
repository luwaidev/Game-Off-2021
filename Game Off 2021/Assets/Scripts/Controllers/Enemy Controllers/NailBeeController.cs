using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NailBeeController : MonoBehaviour, EnemyInterface
{
    public enum State
    {
        Idle,
        Patrol,
        Follow,
        Attack,
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

    [Header("Patrol Settings")]
    private Vector2 patrolCenter;
    [SerializeField] float patrolMaxDistance;
    [SerializeField] float patrolMargin;
    [SerializeField] float patrolWaitTime;
    [SerializeField] float patrolMovementSpeed;

    [Header("Follow Player")]
    [SerializeField] float followDistance;
    [SerializeField] float disengageDistance;
    [SerializeField] float followMovementSpeed;

    [Header("Attack Player")]
    [SerializeField] float attackDistance;
    public static float attackDamage;
    [SerializeField] float attackTime;

    [Header("Hit settings")]
    [SerializeField] float hitTime;
    [SerializeField] float knockbackSpeed;
    [Header("Repel Settings")]
    [SerializeField] float maxRepel;
    [SerializeField] float repelFalloff;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, patrolMaxDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, followDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
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


            if (Vector2.Distance(PlayerController.instance.transform.position, transform.position) < followDistance) state = State.Follow;
            yield return 0;

        }
        NextState();
    }

    IEnumerator PatrolState()
    {

        Vector2 patrolPosition = Vector2.zero;
        bool patrolling = false;

        while (state == State.Patrol)
        {
            if (patrolling)
            {
                velocity = (patrolPosition - (Vector2)transform.position).normalized * patrolMovementSpeed;

                patrolling = !(Vector2.Distance(transform.position, patrolPosition) < patrolMargin);
            }
            else
            {
                patrolPosition.x = patrolCenter.x + Mathf.Sign(Random.Range(-1, 1)) * (-Mathf.Sqrt(-patrolMaxDistance * Random.Range(0, patrolMaxDistance) + Mathf.Pow(patrolMaxDistance, 2f)) + patrolMaxDistance);

                patrolPosition.y = patrolCenter.y + Mathf.Sign(Random.Range(-1, 1)) * (-Mathf.Sqrt(-patrolMaxDistance * Random.Range(0, patrolMaxDistance) + Mathf.Pow(patrolMaxDistance, 2f)) + patrolMaxDistance);

                velocity = Vector2.zero;
                yield return new WaitForSeconds(patrolWaitTime);

                patrolling = true;
            }

            if (Vector2.Distance(PlayerController.instance.transform.position, transform.position) < followDistance) state = State.Follow;

            anim.Play("Nail Walk " + Direction());
            yield return 0;

        }
        NextState();
    }

    IEnumerator FollowState()
    {

        while (state == State.Follow)
        {
            Vector2 playerPosition = PlayerController.instance.transform.position;
            float playerDistance = Vector2.Distance(playerPosition, transform.position);
            if (playerDistance > disengageDistance) state = State.Patrol;
            else if (playerDistance > attackDistance)
            {
                velocity = (playerPosition - (Vector2)transform.position).normalized * followMovementSpeed;
            }
            else if (playerDistance < attackDistance)
            {
                float xDistance = playerPosition.x - transform.position.x;
                float yDistance = playerPosition.y - transform.position.y;

                velocity.x = (Mathf.Abs(xDistance) > Mathf.Abs(yDistance)) ? followMovementSpeed * Mathf.Sign(xDistance) : 0;
                velocity.y = (Mathf.Abs(yDistance) > Mathf.Abs(xDistance)) ? followMovementSpeed * Mathf.Sign(yDistance) : 0;

                if (xDistance < 0.2f || yDistance < 0.2f)
                {
                    state = State.Attack;
                }
            }

            anim.Play("Nail Walk " + Direction());
            yield return 0;
        }
        NextState();


    }

    // void ForYushi()
    // {
    //     string cards = "ABCDEFGH";
    //     string chosenLetters = "";
    //     char[][] grid = new char[4][4];
    //     for (int i = 0; i < grid.length; i++)
    //     {
    //         for (int j = 0; j < grid[i].length; j++)
    //         {
    //             int randomCard = (int)(Math.random() * 8);
    //             while (chosenLetters.count(cards.charAt(randomCard)) > 2)
    //             {
    //                 randomCard = (int)(Math.random() * 8);
    //             }
    //             grid[i][j] = cards.charAt(randomCard);
    //             // System.out.println(randomCard);
    //         }
    //     }
    //     for (char[] x : grid)
    //     {
    //         for (char y : x)
    //         {
    //             System.out.print(y + " ");
    //         }
    //         System.out.println();
    //     }
    // }    

    IEnumerator AttackState()
    {
        anim.Play("Nail Attack " + Direction());
        velocity = Vector2.zero;
        yield return new WaitForSeconds(attackTime);
        anim.Play("Nail Idle " + Direction());

        yield return new WaitForSeconds(attackTime / 2);
        state = State.Follow;
        NextState();
    }

    IEnumerator HitState()
    {
        anim.Play("Nail Hit " + Direction());
        Vector2 playerDirection = PlayerController.instance.transform.position - transform.position;
        velocity = -playerDirection * knockbackSpeed;

        yield return new WaitForSeconds(hitTime);

        state = State.Follow;
        NextState();
    }

    IEnumerator DieState()
    {
        anim.Play("Nail Die " + Direction());
        yield return new WaitForSeconds(0.8f);
        Destroy(gameObject);
    }

    ////////////////////////////////////////////////////////////////
    //                         Functions                          //
    ////////////////////////////////////////////////////////////////
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    string Direction()
    {
        Vector2 playerPosition = PlayerController.instance.transform.position;
        float xDistance = playerPosition.x - transform.position.x;
        float yDistance = playerPosition.y - transform.position.y;

        if (Mathf.Abs(xDistance) > Mathf.Abs(yDistance))
        {
            return (xDistance > 0) ? "Right" : "Left";
        }
        else
        {
            return (yDistance > 0) ? "Up" : "Down";
        }
    }
    void Start()
    {
        NextState();
        patrolCenter = transform.position;
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
        velocity += new Vector2(-Mathf.Pow(enemyToSelf.x, 1f / 3f), -Mathf.Pow(enemyToSelf.x, 1f / 3f)) + Vector2.one * maxRepel;
    }

    public void OnHit(int damage)
    {
        health -= damage;
        state = health <= 0 ? State.Die : State.Hit;
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

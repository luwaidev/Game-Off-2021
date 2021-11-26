using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeWorkerController : MonoBehaviour
{
    public enum State
    {
        Follow,
        Lead,
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

    [Header("Follow Player")]
    public bool leader;
    private BeeWorkerController beeAhead;
    [SerializeField] float followDistance;
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
        print(methodName);

        // Get method
        System.Reflection.MethodInfo info =
            GetType().GetMethod(methodName,
                                System.Reflection.BindingFlags.NonPublic |
                                System.Reflection.BindingFlags.Instance);

        StartCoroutine((IEnumerator)info.Invoke(this, null)); // Call the next state
    }

    IEnumerator FollowState()
    {
        targetPosition = beeAhead.targetPosition;
        Debug.Log("Follow: Enter");
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
        Debug.Log("Follow: Exit");
        NextState();
    }

    IEnumerator LeadState()
    {

        Debug.Log("Follow: Enter");
        while (state == State.Lead)
        {
            if (Vector2.Distance(targetPosition, transform.position) < 0.1f)
            {
                Vector2 playerPosition = PlayerController.instance.transform.position;
                targetPosition.x = playerPosition.x + Mathf.Sign(Random.Range(-1, 1)) * (-Mathf.Sqrt(-playerDistance * Random.Range(0, playerDistance) + Mathf.Pow(playerDistance, 2f)) + playerDistance);

                targetPosition.y = playerPosition.y + Mathf.Sign(Random.Range(-1, 1)) * (-Mathf.Sqrt(-playerDistance * Random.Range(0, playerDistance) + Mathf.Pow(playerDistance, 2f)) + playerDistance);
            }

            Vector2 target = targetPosition - (Vector2)transform.position;
            velocity = target.normalized * movementSpeed;
            yield return null;
        }
        Debug.Log("Follow: Exit");
        NextState();
    }

    IEnumerator HitState()
    {
        Debug.Log("Hit: Enter");


        Vector2 playerDirection = PlayerController.instance.transform.position - transform.position;
        velocity = -playerDirection * knockbackSpeed;

        yield return new WaitForSeconds(hitTime);

        state = State.Follow;
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

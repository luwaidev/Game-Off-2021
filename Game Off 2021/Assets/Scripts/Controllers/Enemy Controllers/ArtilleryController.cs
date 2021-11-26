using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryController : MonoBehaviour
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

    [Header("Projectile Settings")]
    public float warningTime;
    public float projectileSpeed;

    [Header("Hit settings")]
    [SerializeField] float hitTime;
    [SerializeField] float knockbackSpeed;
    [Header("Repel Settings")]
    [SerializeField] float maxRepel;
    [SerializeField] float repelFalloff;

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
        Debug.Log("Hit: Enter");


        Vector2 playerDirection = PlayerController.instance.transform.position - transform.position;
        velocity = -playerDirection * knockbackSpeed;

        yield return new WaitForSeconds(hitTime);

        // state = State.Follow;
        Debug.Log("Hit: Exit");
        NextState();
    }

    IEnumerator FiringState()
    {


        yield return new WaitForSeconds(attackTime);

        Vector2 position = PlayerController.instance.transform.position;
        Instantiate(target, position, Quaternion.identity);
        yield return new WaitForSeconds(warningTime);

        Transform p = Instantiate(target, position + Vector2.up * 6, Quaternion.identity).transform;

        while (Vector2.Distance(p.position, transform.position) < 0.5f) yield return null;

        state = (Vector2.Distance(PlayerController.instance.transform.position, transform.position) < aggroDistance) ?
            State.Firing : State.Idle;
        NextState();
    }

    IEnumerator HitState()
    {
        Debug.Log("Hit: Enter");


        Vector2 playerDirection = PlayerController.instance.transform.position - transform.position;
        velocity = -playerDirection * knockbackSpeed;

        yield return new WaitForSeconds(hitTime);

        // state = State.Follow;
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

}

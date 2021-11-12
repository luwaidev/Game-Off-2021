using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTestController : MonoBehaviour, EnemyInterface
{
    public enum State
    {
        Patrol,
        Follow,
        Orbit,
        Attack,
        Hit,
        Die,
    }

    [Header("References")]

    [Header("Health Settings")]
    float shield;
    public int health { get; set; }

    [Header("State Settings")]
    public State state;
    private Vector2 velocity;

    [Header("Patrol Settings")]
    private bool patrolling;
    private Vector2 patrolCenter;
    private Vector2 patrolPosition;
    private float patrolMaxDistance; // Real max distance will be sqrt(patrolMaxDistance)

    [SerializeField] float patrolMargin;
    [SerializeField] float patrolWaitTime;
    [SerializeField] float patrolMovementSpeed;


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
        while (state == State.Patrol)
        {
            if (patrolling)
            {
                velocity = (patrolPosition - (Vector2)transform.position).normalized * patrolMovementSpeed;

                patrolling = Vector2.Distance(transform.position, patrolPosition) < patrolMargin;
            }
            else
            {
                patrolPosition.x = Mathf.Sign(Random.Range(-1, 1)) * (-Mathf.Sqrt(Random.Range(0, patrolMaxDistance)) + (patrolMaxDistance * patrolMaxDistance));

                patrolPosition.y = Mathf.Sign(Random.Range(-1, 1)) * (-Mathf.Sqrt(Random.Range(0, patrolMaxDistance)) + (patrolMaxDistance * patrolMaxDistance));

                yield return new WaitForSeconds(patrolWaitTime);
            }

            yield return 0;
        }
        Debug.Log("Patrol: Exit");
        NextState();
    }

    IEnumerator FollowState()
    {
        Debug.Log("Follow: Enter");
        while (state == State.Follow)
        {

            yield return 0;
        }
        Debug.Log("Follow: Exit");
        NextState();
    }
    IEnumerator OrbitState()
    {
        Debug.Log("Orbit: Enter");
        while (state == State.Orbit)
        {
            yield return 0;
        }
        Debug.Log("Orbit: Exit");
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

    void Start()
    {
        NextState();
    }


    public void OnHit(int damage)
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoneyBossController : MonoBehaviour, EnemyInterface
{
    public enum State
    {
        Idle,
        Smash,
        Shoot,
        Pop,
        Hit,
        Die,
    }

    [Header("References")]
    private Rigidbody2D rb;
    private Animator anim;
    private List<Transform> nearbyEnemies = new List<Transform>();
    public GameObject block;
    public GameObject healthBar;

    [Header("State Settings")]
    public State state;
    public int health { get; set; }
    public Vector2 maxPosition;
    public Vector2 minPosition;

    [Header("Attack Settings")]
    public float aggroDistance;
    public float recoveryTime;
    public float smashDistance;
    public float smashPrepTime;
    public float smashAfterTime;
    public GameObject smashCollider;

    public float shootDistance;
    public float shootPrepTime;
    public float shootAfterTime;
    public Transform shootingPositions;
    public GameObject projectile;

    public float popPrepTime;
    public float popAfterTime;

    [Header("Hit settings")]
    [SerializeField] float hitTime;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(maxPosition, new Vector3(maxPosition.x, minPosition.y));
        Gizmos.DrawLine(maxPosition, new Vector3(minPosition.x, maxPosition.y));
        Gizmos.DrawLine(minPosition, new Vector3(maxPosition.x, minPosition.y));
        Gizmos.DrawLine(minPosition, new Vector3(minPosition.x, maxPosition.y));

        Gizmos.color = Color.red;
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

    IEnumerator IdleState()
    {


        while (state == State.Idle)
        {

            if (Vector2.Distance(transform.position, PlayerController.instance.transform.position) < aggroDistance)
            {
                state = SetRandomState();
                block.SetActive(true);
                healthBar.transform.parent.GetComponent<Animator>().SetBool("Active", true);
            }

            // transform.position = LockPosition(new Vector2(Random.Range(minPosition.x, maxPosition.x), Random.Range(minPosition.y, maxPosition.y)));
            // yield return new WaitForSeconds(5);
            yield return null;

        }
        NextState();
    }

    IEnumerator SmashState()
    {
        for (int i = 0; i < 2; i++)
        {
            GetComponent<BoxCollider2D>().enabled = true;
            anim.Play("HBoss Smash");
            Vector2 position = Vector2.zero;
            bool playerOnRightSide = PlayerController.instance.transform.position.x > (maxPosition.x + minPosition.x) / 2;
            position.x = PlayerController.instance.transform.position.x + (playerOnRightSide ? -smashDistance : smashDistance);
            position.y = PlayerController.instance.transform.position.y;

            transform.position = LockPosition(position);
            print(LockPosition(position));
            yield return new WaitForSeconds(smashPrepTime - 0.5f);

            playerOnRightSide = PlayerController.instance.transform.position.x > (maxPosition.x + minPosition.x) / 2;
            transform.localScale = new Vector3(playerOnRightSide ? 1 : -1, 1, 1);

            yield return new WaitForSeconds(0.5f);
            smashCollider.SetActive(true);
            yield return new WaitForSeconds(smashAfterTime / 2);
            GetComponent<BoxCollider2D>().enabled = false;
            smashCollider.SetActive(false);
            yield return new WaitForSeconds(smashAfterTime / 2);
            transform.localScale = new Vector3(1, 1, 1);


            anim.Play("HBoss Hidden");
        }

        yield return new WaitForSeconds(recoveryTime);
        state = SetRandomState();
        NextState();
    }

    IEnumerator ShootState()
    {
        for (int j = 0; j < 2; j++)
        {
            anim.Play("HBoss Shoot");
            Vector2 position = Vector2.zero;
            bool playerOnRightSide = PlayerController.instance.transform.position.x > (maxPosition.x + minPosition.x) / 2;
            position.x = PlayerController.instance.transform.position.x + (playerOnRightSide ? shootDistance : -shootDistance);
            bool playerAbove = PlayerController.instance.transform.position.y > (maxPosition.y + minPosition.y) / 2;
            position.y = PlayerController.instance.transform.position.y + (playerAbove ? shootDistance : -shootDistance);

            GetComponent<BoxCollider2D>().enabled = true;
            transform.position = LockPosition(position);
            yield return new WaitForSeconds(shootPrepTime);
            for (int i = 0; i < shootingPositions.childCount; i++)
            {
                Instantiate(projectile, shootingPositions.GetChild(i).position, shootingPositions.GetChild(i).rotation);
            }
            yield return new WaitForSeconds(shootAfterTime);
            GetComponent<BoxCollider2D>().enabled = false;

            anim.Play("HBoss Hidden");
        }


        yield return new WaitForSeconds(recoveryTime);

        state = SetRandomState();
        NextState();
    }

    IEnumerator PopState()
    {
        for (int j = 0; j < 5; j++)
        {
            anim.Play("HBoss Pop");
            transform.position = LockPosition(PlayerController.instance.transform.position);

            yield return new WaitForSeconds(popPrepTime);
            GetComponent<BoxCollider2D>().enabled = true;
            yield return new WaitForSeconds(popAfterTime);
            GetComponent<BoxCollider2D>().enabled = false;
            anim.Play("HBoss Hidden");
        }


        yield return new WaitForSeconds(recoveryTime);

        state = SetRandomState();
        NextState();
    }


    IEnumerator HitState()
    {

        yield return new WaitForSeconds(hitTime);

        NextState();
    }

    IEnumerator DieState()
    {
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


    void Start()
    {
        NextState();
        health = 120;
    }

    public void OnHit(int damage)
    {
        health -= damage;
        state = health <= 0 ? State.Die : State.Hit;
        healthBar.transform.localScale = new Vector3(9.4f * ((float)health / (float)120), 0.75f, 1);
        print((health / 120));
        healthBar.GetComponent<Animator>().Play("HBar Hit");
    }
    State SetRandomState()
    {
        switch (Random.Range(0, 3))
        {
            case 0:
                return State.Smash;
            case 1:
                return State.Shoot;
            case 2:
                return State.Pop;
            default:
                return State.Smash;
        }
    }
    Vector2 LockPosition(Vector2 position)
    {
        position.y = Mathf.Round((position.y - minPosition.y) * 1.063829f) / 1.063829f + minPosition.y;
        if (Mathf.Round(position.y) % 2 == 0)
        {
            position.x = Mathf.Round((position.x - minPosition.x) * 0.88888888f) / 0.88888888f + minPosition.x;
        }
        else position.x = Mathf.Floor((position.x - minPosition.x) * 0.88888888f) / 0.88888888f + minPosition.x + 0.5625f;
        return position;
    }


}

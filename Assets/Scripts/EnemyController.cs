using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private enum State { Idle, Pathing, ChaseTarget, Attacking, Eating };
    private State state;

    public float chaseRadius = 10f;
    public float attackDistance = 2f;
    public float idleTime = 3f;
    private float timer = 0f;

    private float wayPointSearchRadius = 15f;
    private int currentWayPoint = 0;
    Vector3 direction;
    Quaternion lookRotation;

    public List<GameObject> pathWayPoints = new List<GameObject>();
    private bool searchedWayPoints = false;

    Transform target;
    NavMeshAgent agent;
    Animator animator;

    private float targetSpeed;
    private float currentSpeed;
    float speedSmoothTime = 0.2f;
    float speedSmoothVelocity;
    float extraRotationSpeed = 5;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponentInChildren<Animator>();
        state = State.Pathing;
        targetSpeed = 1f;
    }

    void Update()
    {
        if (!searchedWayPoints)
        {
            SearchWayPoints();
            searchedWayPoints = true;
        }

        float distance = 100f;

        if (target != null)
        {
            distance = Vector3.Distance(target.position, transform.position);
        }


        if (distance <= chaseRadius && distance > attackDistance)
        {
            state = State.ChaseTarget;
        }

        if (distance <= attackDistance)
        {
            state = State.Attacking;
        }

        if (state == State.ChaseTarget && distance > chaseRadius)
        {
            agent.isStopped = true;
            state = State.Idle;
        }

        if (distance < attackDistance)
        {
            animator.SetBool("isAttacking", false);
            state = State.Eating;
        }

        switch (state)
        {
            case State.Idle:
                Idle();
                break;
            case State.Pathing:
                Pathing();
                break;
            case State.ChaseTarget:
                ChaseTarget();
                break;
            case State.Attacking:
                Attacking();
                break;
            case State.Eating:
                Eating();
                break;
        }

        if (distance <= attackDistance)
        {
            FaceTarget(PlayerCharacterController.Instance.playerPosition);
        }

        extraRotation();
    }

    // Search waypoints at the start of the game
    void SearchWayPoints()
    {
        GameObject[] foundWayPoints = GameObject.FindGameObjectsWithTag("WayPoint");

        foreach (GameObject wayPoint in foundWayPoints)
        {
            if (Vector3.Distance(wayPoint.transform.position, transform.position) < wayPointSearchRadius)
            {
                if (wayPoint.name == "WayPoint")
                    pathWayPoints.Add(wayPoint);
            }
        }

        foreach (GameObject wayPoint in foundWayPoints)
        {
            if (Vector3.Distance(wayPoint.transform.position, transform.position) < wayPointSearchRadius)
            {
                if (wayPoint.name == "WayPoint1")
                    pathWayPoints.Add(wayPoint);
            }
        }

        foreach (GameObject wayPoint in foundWayPoints)
        {
            if (Vector3.Distance(wayPoint.transform.position, transform.position) < wayPointSearchRadius)
            {
                if (wayPoint.name == "WayPoint2")
                    pathWayPoints.Add(wayPoint);
            }
        }

        foreach (GameObject wayPoint in foundWayPoints)
        {
            if (Vector3.Distance(wayPoint.transform.position, transform.position) < wayPointSearchRadius)
            {
                if (wayPoint.name == "WayPoint3")
                    pathWayPoints.Add(wayPoint);
            }
        }
    }

    void Idle()
    {
        animator.SetBool("isAttacking", false);

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Walking"))
        {
            targetSpeed = 0;
            currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);
            agent.speed = currentSpeed;

            float animationSpeedPercent = currentSpeed / 4f;
            animator.SetFloat("speedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);

            timer += Time.deltaTime;
            if (timer > idleTime)
            {
                timer = 0;
                state = State.Pathing;
                agent.isStopped = false;
            }
        }
    }

    void Pathing()
    {
        animator.SetBool("isAttacking", false);
        agent.isStopped = false;

        direction = (pathWayPoints[currentWayPoint].transform.position - transform.position).normalized;
        lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 6f);

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Walking"))
        {
            targetSpeed = 1f;
            currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);
            agent.speed = currentSpeed;

            float animationSpeedPercent = currentSpeed / 4f;
            animator.SetFloat("speedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);


            agent.SetDestination(pathWayPoints[currentWayPoint].transform.position);

            if (Vector3.Distance(transform.position, pathWayPoints[currentWayPoint].transform.position) < agent.stoppingDistance)
            {
                currentWayPoint++;

                if (currentWayPoint >= pathWayPoints.Count)
                {
                    currentWayPoint = 0;
                }
            }
        }
    }

    void ChaseTarget()
    {
        agent.isStopped = false;
        animator.SetBool("isAttacking", false);

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Walking"))
        {
            targetSpeed = 4f;
            currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);
            agent.speed = currentSpeed;

            float animationSpeedPercent = currentSpeed / 4f;
            animator.SetFloat("speedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);


            animator.SetFloat("speedPercent", 1f);
            timer = 0;
            agent.SetDestination(target.position);
            agent.isStopped = false;
        }
    }

    void Attacking()
    {
        FaceTarget(PlayerCharacterController.Instance.playerPosition);
        animator.SetBool("isAttacking", true);
        agent.velocity = Vector3.zero;
        agent.isStopped = true;
        agent.speed = 0;

        if (Vector3.Distance(transform.position, PlayerCharacterController.Instance.playerPosition) < 1f)
        {
            Debug.Log("Zombie has reached player");
        }
    }

    void FaceTarget(Vector3 position)
    {
        Vector3 direction = (position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        transform.rotation = lookRotation;
    }

    void Eating()
    {
        animator.SetBool("isEating", true);
    }

    void extraRotation()
    {
        Vector3 lookrotation = agent.steeringTarget - transform.position;

        if (lookrotation != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookrotation), extraRotationSpeed * Time.deltaTime);
    }

    public void FindPlayer()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }
}

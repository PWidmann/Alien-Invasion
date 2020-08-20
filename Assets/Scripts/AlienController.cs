using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AlienController : MonoBehaviour
{
    private Animator animator;
    private enum State { Idle, Pathing, ChaseTarget, Attacking };
    private State state;

    public float chaseRadius = 15f;
    public float attackDistance = 8f;
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

    private float targetSpeed;
    private float currentSpeed;
    float speedSmoothTime = 0.2f;
    float speedSmoothVelocity;
    float extraRotationSpeed = 5;

    public Transform rayStart;
    RaycastHit hit;

    private int health = 3;

    public int Health { get => health; set => health = value; }


    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        state = State.Pathing;
        targetSpeed = 2f;
    }

    
    void Update()
    {
        if (isPlayerInSight())
        {
            Debug.DrawRay(rayStart.position, target.position - transform.position, Color.blue);
        }
        else
        {
            Debug.DrawRay(rayStart.position, target.position - transform.position, Color.red);
        }




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

        if (distance <= chaseRadius && distance > attackDistance && PlayerCharacterController.Instance.Alive)
        {
            state = State.ChaseTarget;
            Debug.Log("Changed State to ChaseTarget");
        }

        if (distance <= attackDistance && PlayerCharacterController.Instance.Alive)
        {
            state = State.Attacking;
            Debug.Log("Changed State to Attacking");
        }

        if (state == State.ChaseTarget && distance > chaseRadius)
        {
            agent.isStopped = true;
            state = State.Idle;
            Debug.Log("Changed State to Idle");
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
        }

        if (health > 0)
        {
            if (distance <= attackDistance)
            {
                FaceTarget(PlayerCharacterController.Instance.playerPosition);
            }

            extraRotation();
        }
        else
        {
            agent.speed = 0;
            state = State.Idle;
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

                Debug.Log("Changed state to pathing");
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
            targetSpeed = 4f;
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

        //if direct lign of sight to player
        if (isPlayerInSight())
        {
            animator.SetBool("isAttacking", true);
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
            agent.speed = 0;
            
        }
        else
        {
            state = State.ChaseTarget;
        }
    }

    void FaceTarget(Vector3 position)
    {
        if (health > 0)
        {
            Vector3 direction = (position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

            transform.rotation = lookRotation;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health > 0)
        {
            animator.SetTrigger("gotHit");
        }
        else
        {
            GetComponent<CharacterController>().enabled = false;
            animator.enabled = false;
        }
    }

    void extraRotation()
    {
        Vector3 lookrotation = agent.steeringTarget - transform.position;

        if (lookrotation != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookrotation), extraRotationSpeed * Time.deltaTime);
    }

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

    bool isPlayerInSight()
    {
        bool hasSeenPlayer = false;

        if (Physics.Raycast(rayStart.position, target.transform.position - transform.position, out hit, 100.0f))
        {
            if (hit.transform.tag == "Player")
            {
                hasSeenPlayer = true;
            }
        }

        return hasSeenPlayer;
    }
}

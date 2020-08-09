using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
    
    
    public float walkSpeed = 2;
    public float runSpeed = 4;
    private float currentSpeed = 0;
    public float gravity = -10;
    public Vector3 velocity;
    float velocityY;

    public float turnSmoothTime = 0.1f;
    public float turnSmoothVelocity;
    public float speedSmoothTime = 0.1f;
    public Vector3 targetRotation;
    float speedSmoothVelocity;
    public float turnSpeed = 300f;

    public bool hasSeenPlayer = false;

    CharacterController controller;
    Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        currentSpeed = 0;
    }

    
    void Update()
    {
        Move();

        if (Input.GetKeyDown(KeyCode.F))
        {
            hasSeenPlayer = !hasSeenPlayer;
        }
    }

    void Move()
    {
        //Gravity
        velocityY += Time.deltaTime * gravity;

        if (controller.isGrounded)
        {
            velocityY = 0;
        }

        velocity = transform.forward * currentSpeed + Vector3.up * velocityY;

        if (hasSeenPlayer)
        {
            Vector3 targetDirection = PlayerCharacterController.Instance.playerPosition - transform.position;
            targetDirection.y = 0;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetDirection), Time.deltaTime * turnSpeed);

            currentSpeed = 2;
        }
        else
        {
            currentSpeed = 0;
        }

        controller.Move(velocity * Time.deltaTime);
    }
}

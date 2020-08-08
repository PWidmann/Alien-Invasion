using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float moveSpeed = 4f;
    private float gravity = -10f;

    private bool rotateTowardsMouse;
    private float maxSpeed;

    public float currentSpeed;
    public Vector3 velocity;
    float animationSpeedPercent;
    float velocityY;

    float targetRotation;
    float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public Vector3 inputDir;
    bool isAiming = false;
    public Vector3 aimMovement;

    float speedSmoothTime = 0.1f;
    float speedSmoothVelocity;

    Animator animator;
    CharacterController characterController;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        maxSpeed = moveSpeed;
    }

    private void Update()
    {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        inputDir = input.normalized;
        

        // Move in the direction we are aiming;
        Move(inputDir);

        
        // animator
        animationSpeedPercent = currentSpeed / maxSpeed;
        animator.SetFloat("speedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);

        

        // Mouse aiming
        MouseAimingActive(input);
    }

    void Move(Vector3 inputDir)
    {
        //Gravity
        velocityY += Time.deltaTime * gravity;

        // Smooth runspeed
        float targetSpeed = maxSpeed * inputDir.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);


        inputDir = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * inputDir;


        if (isAiming)
        {
            aimMovement = (transform.TransformDirection(inputDir) * currentSpeed).normalized;
            aimMovement = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * aimMovement;

            animator.SetFloat("aimMovementX", aimMovement.x);
            animator.SetFloat("aimMovementY", aimMovement.z);
        }
        

        velocity = inputDir.normalized * currentSpeed + Vector3.up * velocityY;

        characterController.Move(velocity * Time.deltaTime);

        // Grounded
        if (characterController.isGrounded)
        {
            velocityY = 0;
        }
    }

    void RotateTowardMovementVector(Vector3 movementVector)
    {

        //Player rotation
        if (inputDir != Vector3.zero)
        {
            targetRotation = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
        }
    }

    

    void MouseAimingActive(Vector3 movementVector)
    {
        //Right mouse button
        if (Input.GetMouseButton(1))
        {
            isAiming = true;
            rotateTowardsMouse = true;
            animator.SetBool("isAiming", true);
            maxSpeed = 4f;
        }
        else
        {
            isAiming = false;
            rotateTowardsMouse = false;
            animator.SetBool("isAiming", false);
            maxSpeed = 8f;
        }

        if (!rotateTowardsMouse)
            RotateTowardMovementVector(movementVector);
        else
            RotateTowardMouseVector();
    }

    void RotateTowardMouseVector()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance: 300f))
        {
            var target = hitInfo.point;

            var targetDir = transform.position - target;
            target.y = transform.position.y;

            targetRotation = Mathf.Atan2(targetDir.x, targetDir.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation + 132f, ref turnSmoothVelocity, turnSmoothTime);
        }
    }
}

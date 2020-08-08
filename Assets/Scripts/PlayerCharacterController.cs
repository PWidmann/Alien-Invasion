using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacterController: MonoBehaviour
{

    //Movement
    [Header("Movement")]
    public Vector2 inputDir;
    public Vector3 velocity;
    float runSpeed = 8;
    float gravity = -10;
    float turnSmoothTime = 0.2f;
    public float turnSmoothVelocity;
    float speedSmoothTime = 0.1f;
    float speedSmoothVelocity;

    float currentSpeed;
    float velocityY;
    private float targetRotation;

    private static bool isAiming = false;

    //References
    Animator animator;
    Transform cameraT;
    CharacterController controller;
    public float angle;
    float animationSpeedPercent;

    public static bool IsAiming { get => isAiming; set => isAiming = value; }

    void Start()
    {
        // References
        animator = GetComponent<Animator>();
        cameraT = Camera.main.transform;
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        //Movement
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        inputDir = input.normalized;
        Move(inputDir);

        // animator
        animationSpeedPercent = currentSpeed / runSpeed;
        animator.SetFloat("speedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);

        if (Input.GetMouseButton(1))
        {
            IsAiming = true;
            runSpeed = 3f;
            animator.SetBool("isAiming", true);
            
        }
        else
        {
            IsAiming = false;
            runSpeed = 8f;
            animator.SetBool("isAiming", false);
        }

        if (isAiming  && WeaponHandler.Instance.pistolActive && Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("PistolShoot");
            SoundManager.instance.PlaySound(0);
        }
    }

    void Move(Vector2 inputDir)
    {
        
        //Gravity
        velocityY += Time.deltaTime * gravity;

        if(IsAiming)
            AimToMouse();

        //Player Rotation
        if (inputDir != Vector2.zero && !IsAiming)
        {
            // Turn the player in movement direction when not aiming
            targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
        }

        // Smooth runspeed 
        float targetSpeed = runSpeed * inputDir.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        if (IsAiming)
        {
            // Aiming movement
            Vector3 targetDirection = new Vector3(inputDir.x, 0, inputDir.y);
            targetDirection = Quaternion.Euler(0, 45f, 0) * targetDirection; // Rotate 45° because of isometric camera

            // Move the controller
            velocity = targetDirection * currentSpeed + Vector3.up * velocityY;
            controller.Move(velocity * Time.deltaTime);
            
        }
        else
        { 
            velocity = transform.forward * currentSpeed + Vector3.up * velocityY;
            controller.Move(velocity * Time.deltaTime);
        }

        

        void AimToMouse()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance: 300f))
            {
                Vector3 target = hitInfo.point;
                target.y = transform.position.y;
                Vector3 targetDir = (target - transform.position).normalized;

                Debug.DrawRay(transform.position + new Vector3(0, 1, 0), targetDir);
                transform.LookAt(target);


                // match target aiming vector to unrotated animation vector
                Vector3 newInput = new Vector3(inputDir.x, 0, inputDir.y);
                angle = transform.eulerAngles.y - 45f;
                Vector3 animationVector = (Quaternion.Euler(0, -angle, 0) * newInput.normalized);

                animator.SetFloat("aimMovementX", animationVector.x, 0.1f, Time.deltaTime);
                animator.SetFloat("aimMovementY", animationVector.z, 0.1f, Time.deltaTime);
            }
        }
        

        //Reset jumping
        if (controller.isGrounded)
        {
            velocityY = 0;
        }
    }
}

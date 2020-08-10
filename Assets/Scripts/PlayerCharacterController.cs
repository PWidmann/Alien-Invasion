using System.Collections.Generic;
using System.Runtime.Remoting.Lifetime;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacterController: MonoBehaviour
{
    public static PlayerCharacterController Instance;
    //Movement
    [Header("Movement")]
    Vector3 playerPosition;
    Vector2 inputDir;
    Vector3 velocity;
    float runSpeed = 8;
    float gravity = -10;
    float turnSmoothTime = 0.2f;
    float turnSmoothVelocity;
    float speedSmoothTime = 0.1f;
    float speedSmoothVelocity;
    float currentSpeed;
    float velocityY;
    private float targetRotation;

    public GameObject pistolEndingPosition;

    //References
    Animator animator;
    Transform cameraT;
    RaycastHit hitInfo;
    CharacterController controller;
    float angle;
    float animationSpeedPercent;


    // Weapons
    float pistolShootTimer = 0.5f;
    bool pistolReady = true;

    //Aiming
    private static bool isAiming = false;
    Transform chest; // For rotating aiming animation
    Vector3 aimAnimOffset = new Vector3(15, 35, 15); // Best compromise of far and near target aiming rotation
    Quaternion aimRotation;

    public static bool IsAiming { get => isAiming; set => isAiming = value; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    void Start()
    {
        // References
        animator = GetComponent<Animator>();
        cameraT = Camera.main.transform;
        controller = GetComponent<CharacterController>();

        // For aiming animation
        chest = animator.GetBoneTransform(HumanBodyBones.Chest);
    }

    void Update()
    {
        Movement();
        Aiming();
        PistolShooting();
    }

    private void LateUpdate()
    {
        // point both arms to shooting target while aiming.
        if (isAiming && WeaponHandler.Instance.pistolActive)
        {

            chest.LookAt(hitInfo.point);
            aimRotation = chest.rotation * Quaternion.Euler(aimAnimOffset);
            chest.rotation = aimRotation;
        }
    }
    void Movement()
    {
        playerPosition = transform.position;
        //Movement
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        inputDir = input.normalized;
        Move(inputDir);

        // animator
        animationSpeedPercent = currentSpeed / runSpeed;
        animator.SetFloat("speedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);
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
            //ray.origin = pistolEndingPosition.transform.position;

            if (Physics.Raycast(ray, out hitInfo, maxDistance: 300f))
            {
                Vector3 target = hitInfo.point;
                target.y = transform.position.y;
                Vector3 targetDir = (target - transform.position).normalized;

                Debug.DrawRay(transform.position + new Vector3(0, 1, 0), targetDir * 2);
                transform.LookAt(target);


                // match target aiming vector to unrotated animation map vector
                Vector3 newInput = new Vector3(inputDir.x, 0, inputDir.y);
                angle = transform.eulerAngles.y - 45f;
                Vector3 animationVector = (Quaternion.Euler(0, -angle, 0) * newInput.normalized);

                animator.SetFloat("aimMovementX", animationVector.x, 0.1f, Time.deltaTime);
                animator.SetFloat("aimMovementY", animationVector.z, 0.1f, Time.deltaTime);
            }
        }
        

        //Grounded
        if (controller.isGrounded)
        {
            velocityY = 0; // Deactivate gravity
        }
    }

    void Aiming()
    {
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
    }

    void PistolShooting()
    {
        if (!pistolReady)
        {
            pistolShootTimer -= Time.deltaTime;

            if (pistolShootTimer <= 0)
            {
                pistolShootTimer = 0.5f;
                pistolReady = true;
            }
        }

        // Shooting Pistol
        if (isAiming && WeaponHandler.Instance.pistolActive && Input.GetMouseButtonDown(0) && pistolReady)
        {
            animator.SetTrigger("PistolShoot");
            SoundManager.instance.PlaySound(0);
            pistolReady = false;

            //Handling muzzle flash
            WeaponHandler.Instance.muzzleActive = true;
            WeaponHandler.Instance.muzzleFlash.Play();

            //Body Shot
            if (hitInfo.transform.tag == "AlienBody")
            {
                Debug.Log("Body shot");
                hitInfo.transform.GetComponent<AlienController>().TakeDamage(1);
            }

            // Head Shot
            if (hitInfo.collider.transform.tag == "AlienHead")
            {
                Debug.Log("Head shot");
                hitInfo.collider.transform.GetComponent<AlienHead>().alien.GetComponent<AlienController>().TakeDamage(3);
            }

            
        }
    }
}

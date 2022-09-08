using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovment : MonoBehaviour
{
    [Header("Refrences")]
    [SerializeField]
    private GameObject mainCamera;
    [SerializeField]
    private LayerMask walkableLayer;
    [SerializeField]
    private GameObject model;
    private Animator animator;
    private CharacterController controller;

    [Header("Player settings")]
    [SerializeField]
    private float walkSpeed = 10;
    [SerializeField]
    private float sprintSpeed = 20;
    [SerializeField]
    private float jumpHeight = 1;
    [SerializeField]
    private float GroundedRadius = 0.28f; // The radius of the grounded check. Should match the radius of the CharacterController
    [SerializeField]
    private float rotationSpeed = 100f;
    [SerializeField]
    public float gravity = -15.0f; // custom gravity value


    [Header("Sfx")]
    [SerializeField]
    private AudioClip[] walkSounds;
    [SerializeField]
    private AudioClip landSound;
    [SerializeField]
    private float walkingAudioVolume = 0.5f;


    [Header("Local Variables")]
    const float JUMPTIMEOUT = 0.50f;
    const float FALLTIMEOUT = 0.15f;


    private Vector2 input;
    private Vector3 currentMovementInput;
    private bool gettingHorizontalInput = false; // if the player is walking or sprinting 
    private float verticalVelocity;
    private float rotationVelocity;
    private bool onGround = true;
    private float offsetFromGround = -0.14f;
    private float SpeedChangeRate = 10.0f;
    private float rotationSmoothSpeed = 0.1f;
    private float animationBlend;
    private float maxVelocity = 53.0f;
    private float fallTimeoutDelta;
    private float jumpTimeoutDelta;
    private bool jumpInput = false;
    private bool sprintInput = false;
    Quaternion targetRotation;
    private PlayerInput playerInput;




    private void Awake()
    {
        playerInput = new PlayerInput();
        // when the player starts this action 
        // context is current input data
        playerInput.CharacterControls.Move.started += OnMovmentInput;
        // for keyboard
        playerInput.CharacterControls.Move.canceled += OnMovmentInput;
        // for gamepad
        playerInput.CharacterControls.Move.performed += OnMovmentInput;
        playerInput.CharacterControls.Sprint.started += OnSprintInput;
        playerInput.CharacterControls.Sprint.canceled += OnSprintInput;
        playerInput.CharacterControls.Jump.started += OnJumpInput;
        playerInput.CharacterControls.Jump.canceled += OnJumpInput;
    }
    private void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        fallTimeoutDelta = FALLTIMEOUT;
        jumpTimeoutDelta = JUMPTIMEOUT;

    }
    private void Update()
    {

        Move();
        // check if player is jumping and react accordinaly 
        Jump();
        // this method simulates gravity
        Gravity();
    }

    // everything having to do with physics in LateUpdate
    private void LateUpdate()
    {
        CheckOnGround();
    }
    private void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }
    private void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }
    private void OnMovmentInput(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
        currentMovementInput.x = input.x;
        currentMovementInput.z = input.y;
        gettingHorizontalInput = input.x != 0 || input.y != 0;
    }
    private void OnSprintInput(InputAction.CallbackContext context)
    {
        sprintInput = context.ReadValueAsButton();
    }
    private void OnJumpInput(InputAction.CallbackContext context)
    {
        jumpInput = context.ReadValueAsButton();
    }
    private void Move()
    {
        // set speed according to walk or sprint input
        float speed = sprintInput ? sprintSpeed : walkSpeed;
        // if there is no input, set speed to 0
        if (!gettingHorizontalInput)
            speed = 0.0f;

        if (gettingHorizontalInput)
        {
            Vector3 direction = new Vector3(currentMovementInput.x, 0f, currentMovementInput.y).normalized;
            float targetRotation = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y; // convert to degrees
            // rotate the player to the direction the camera is facing, controlled by the mouse
            // do so gradually
            float smoothRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmoothSpeed);
            transform.rotation = Quaternion.Euler(0f, smoothRotation, 0f);
            // turn from rotation to direction 
            Vector3 cameraDirection = Quaternion.Euler(0f, targetRotation, 0f)* Vector3.forward;
            // actually move the player 
            controller.Move(cameraDirection.normalized * (speed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f));

        }
        // handle walking animation
        animationBlend = Mathf.Lerp(animationBlend, speed, Time.deltaTime * SpeedChangeRate);
        if (animationBlend < 0.01f)
            animationBlend = 0f;
        animator.SetFloat("Speed", animationBlend);
        animator.SetFloat("MotionSpeed", 1);
    }

    private void CheckOnGround()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - offsetFromGround, transform.position.z);
        onGround = Physics.CheckSphere(spherePosition, GroundedRadius, walkableLayer, QueryTriggerInteraction.Ignore);
        // update animator
        animator.SetBool("Grounded", onGround);
    }
    private void Jump()
    {
        if (onGround)
        {
            animator.SetBool("Jump", false);
            animator.SetBool("FreeFall", false);
            // reset the fall timeout timer
            fallTimeoutDelta = FALLTIMEOUT;
            // stop  velocity dropping 
            if (verticalVelocity < 0.0f)
            {
                verticalVelocity = -1f;
            }
            // measure timeout
            if (jumpTimeoutDelta >= 0.0f)
            {
                jumpTimeoutDelta -= Time.deltaTime;
            }
            if (jumpInput && jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                animator.SetBool("Jump", true);
            }
        }
        else
        {
            // reset the jump timeout timer
            jumpTimeoutDelta = JUMPTIMEOUT;
            // measure timeout
            if (fallTimeoutDelta >= 0.0f)
            {
                fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                animator.SetBool("FreeFall", true);
            }
            // if we are not on the ground, do not jump
            jumpInput = false;
        }
    }

    private void Gravity()
    {
        // act as gravity
        if (verticalVelocity < maxVelocity)
        {
            verticalVelocity += gravity * Time.deltaTime; //multiply by delta time twice to linearly speed up over time
        }
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
                int ran = Random.Range(0, walkSounds.Length);
                AudioSource.PlayClipAtPoint(walkSounds[ran], transform.TransformPoint(controller.center), walkingAudioVolume);
    }

    private void OnLand(AnimationEvent animationEvent)
    {
            AudioSource.PlayClipAtPoint(landSound, transform.TransformPoint(controller.center), walkingAudioVolume);
    }
}
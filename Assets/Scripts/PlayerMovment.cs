


using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;

public class PlayerMovment : MonoBehaviour
{
    [Header("Refrences")]
    [SerializeField]
    private GameObject mainCamera;
    [SerializeField]
    private PlayerInput playerInput;
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
    private AudioClip[] walkignSounds;

    [Header("Local Variables")]
    const float JUMPTIMEOUT = 0.50f;
    const float FALLTIMEOUT = 0.15f;

    private Vector2 input;
    private Vector3 currentMovementInput;
    private bool gettingHorizontalInput = false; // if the player is walking or sprinting 
    private float verticalVelocity;
    private bool onGround = true;
    private float offsetFromGround = -0.14f;
    private float speedOffset = 0.1f;
    [Tooltip("Acceleration and deceleration")]
    private float SpeedChangeRate = 10.0f;
    private float animationBlend;
    private float maxVelocity = 53.0f;
    private float fallTimeoutDelta;
    private float jumpTimeoutDelta;
    private bool jumpInput = false;
    private bool sprintInput = false;
    Quaternion targetRotation;




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
    private void CheckOnGround()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - offsetFromGround, transform.position.z);
        onGround = Physics.CheckSphere(spherePosition, GroundedRadius, walkableLayer, QueryTriggerInteraction.Ignore);
        // update animator
        animator.SetBool("Grounded", onGround);
    }
    private void Move()
    {
        // set speed according to walk or sprint input
        float speed = sprintInput ? sprintSpeed : walkSpeed;
        // if there is no input, set speed to 0
        if (!gettingHorizontalInput)
            speed = 0.0f;

        // handle rotation 
        if (gettingHorizontalInput)
        {
            Vector3 lookAt = new Vector3(currentMovementInput.x, 0.0f, currentMovementInput.z);
            Quaternion currentRotation = transform.rotation;

            targetRotation = Quaternion.LookRotation(lookAt);
            // rotate the player to makeit face the current position
            transform.rotation = Quaternion.RotateTowards(currentRotation, targetRotation, rotationSpeed);
        }

        // no matter where we are facing, that is now the forward direction
        Vector3 targetDirection = transform.rotation     * Vector3.forward;
        // actually move the player
        
       //controller.Move(currentMovementInput * (speed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f));
        controller.Move(targetDirection.normalized * (speed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f));

        animationBlend = Mathf.Lerp(animationBlend, speed, Time.deltaTime * SpeedChangeRate);
        if (animationBlend < 0.01f)
            animationBlend = 0f;
        // update the animator 
        animator.SetFloat("Speed", animationBlend);
        animator.SetFloat("MotionSpeed", 1);
    }
    private void HandleRotation()
    {
        Vector3 lookAt = new Vector3(currentMovementInput.x, 0.0f, currentMovementInput.z);
        Quaternion currentRotation = transform.rotation;
        if (gettingHorizontalInput)
        {
            Quaternion target = Quaternion.LookRotation(lookAt);
            transform.rotation = Quaternion.RotateTowards(currentRotation, target, rotationSpeed);
        }
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
                Debug.Log("freefalling");
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
}
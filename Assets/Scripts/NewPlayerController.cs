using UnityEngine;
using Unity.Netcode;
using Unity.Cinemachine;

public class NewPlayerController : NetworkBehaviour
{
    // Setup a reference to the character controller
    private CharacterController characterController;

    // Setup a reference to the input system
    private PlayerControls playerControls;

    // Rotation variable on the Y-axis
    float rotationY;

    // Movement & rotation speed variables
    [SerializeField] private float moveSpeed = 5.0f;
    //[SerializeField] private float rotationSpeed = 5.0f;

    // Gravity
    [SerializeField] private float gravity = -9.18f;

    // Cameras
    [SerializeField] private Camera mainCamera; // Main camera
    [SerializeField] private CinemachineCamera tpCamera; // Third person camera
    [SerializeField] private Transform cameraTransform; // The third person camera transform for camera-relative movement

    // Input & other speed variables
    private Vector3 verticalVelocity;
    private float currentSpeed;
    private Vector3 playerInput;

    private void Awake()
    {
        // Reference the player controls
        playerControls = new PlayerControls();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Reference the character controller
        characterController = GetComponent<CharacterController>();
    }

    // On enable, enable the player controls
    private void OnEnable()
    {
        playerControls.Enable();
    }

    // On disable, disable the player controls
    private void OnDisable()
    {
        playerControls.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        // Set up a boolean to detect if the player is grounded
        bool isGrounded = characterController.isGrounded;

        // If the player is grounded, set their Y velocity to float -2
        if (isGrounded && verticalVelocity.y < 0) verticalVelocity.y = -2f;

        // If the player is NOT grounded, apply gravity instead
        if (!isGrounded) verticalVelocity.y = gravity * Time.deltaTime;

        // Player functions
        GatherInput();
        CalculateSpeed();
        Movement();
    }

    // Function to obtain the player's input
    private void GatherInput()
    {
        Vector2 input = playerControls.Gameplay.Move.ReadValue<Vector2>();
        playerInput = new Vector3(input.x, 0, input.y);
    }

    // Function to handle the player's direction & movement
    private void Movement()
    {
        // Only run this if the client owns the player character
        if (!IsOwner) return;

        // If the player isn't providing any input, don't do anything
        if (playerInput == Vector3.zero) return;

        // Player looking direction logic
        // Camera variable setup for camera-relative movement
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0;
        camRight.Normalize();

        // Create the direction for the player to look at
        Vector3 movementDirection = new Vector3(playerInput.x, 0f, playerInput.z);

        // Player movement logic
        // If the character controller component is null OR the player is not making any input, return
        if (!characterController) return;

        // Set up movement
        Vector3 moveDirection = camForward * playerInput.z + camRight * playerInput.x;

        // Normalize the movement speed to make it consistent
        moveDirection.Normalize();

        // Set the player's rotation to the direction the player wants to move in
        transform.rotation = Quaternion.LookRotation(moveDirection, Vector3.up);

        // Finalise movement vectors
        Vector3 move = moveDirection * moveSpeed;
        Vector3 finalMove = move * Time.deltaTime + verticalVelocity * Time.deltaTime;

        // Move the player in the desired direction
        characterController.Move(finalMove);
    }

    // Function to handle the player's speed
    private void CalculateSpeed()
    {
        // If no input is being made, set the player's speed to 0. Otherwise, set the player's speed to the value stored in moveSpeed
        if (playerInput.sqrMagnitude < 0.001f)
        {
            currentSpeed = 0;
        }
        else
        {
            currentSpeed = moveSpeed;
        }
    }

    public override void OnNetworkSpawn()
    {
        tpCamera = GetComponentInChildren<CinemachineCamera>();

        // If the client is the owner of the player character and the third person camera exists, disable the main camera and enable the third person camera
        if (IsOwner && tpCamera != null)
        {
        //    mainCamera = Camera.main;
        //    mainCamera.enabled = false;
            tpCamera.enabled = true;
        }
        else if (!IsOwner) // Disable the third persona camera if the client is NOT the owner of the player character
        {
            tpCamera.enabled = false;
        }

        if (!IsOwner)
        {
            tpCamera.gameObject.SetActive(false);
        }

        tpCamera.gameObject.SetActive(true);
    }

    public override void OnNetworkDespawn()
    {
        // Disable the third person camera & enable the main camera
        if (IsOwner && tpCamera != null && mainCamera != null)
        {
            tpCamera.enabled = false;
            mainCamera.enabled = true;
        }
    }
}

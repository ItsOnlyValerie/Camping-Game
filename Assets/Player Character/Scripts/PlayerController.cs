using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;
using Unity.Multiplayer.Center.NetcodeForGameObjectsExample.DistributedAuthority;
using Unity.Netcode.Components;

public class PlayerController : NetworkBehaviour
{

    // Reference the player animator
    Animator animator;

    // Reference the UI Manager
    public UIManager uiManager;

    // Speed variables (and tilt)
    public float turnSpeed = 360;
    public float tiltSpeed = 180;
    public float walkSpeed = 5;
    float tilt = 0;

    // Movement variables (server-authoritative architecture)
    private float serverForward;
    private float serverSide;
    private float serverTurn;

    /// Boolean to communicate with the UI Manager
    public static bool isAlive;

    // Rigidbody
    private Rigidbody rigidBody;

    // Cameras
    [SerializeField] private Camera fpcam; // First person camera
    [SerializeField] private Camera maincam; // Main camera

    // Min & max tilt to stop the camera from rotating infinitely
    public float minTilt = -45f;
    public float maxTilt = 45f;

    // Network variable for the animator's speed parameter
    public NetworkVariable<float> netAnimSpeed = new NetworkVariable<float>();

    // Start is called before the first frame update
    void Start()
    {
        // Restrict the client's cursor to within the game window - CHANGE TO CENTER BEFORE SUBMISSION
        Cursor.lockState = CursorLockMode.Confined;
        // Make the cursor invisible during gameplay - it will only be enabled when necessary, such as interacting with UI
        //Cursor.visible = false;

        // Get the player animator
        animator = GetComponent<Animator>();

        // Get the rigidbody
        rigidBody = GetComponent<Rigidbody>();
        // Prevent the player character from falling over
        rigidBody.freezeRotation = true;

        // Set the networkReady boolean
    }

     // Update is called once per frame
    void Update()
    {
        // If the client is the owner of the player character, allow their inputs to control it - also sort out their animations & movement and transmit data through the network
        if (IsOwner)
        {
            // Keyboard input
            float forward = Input.GetAxisRaw("Vertical");
            float side = Input.GetAxisRaw("Horizontal");
            float turn = Input.GetAxisRaw("Mouse X");

            // Send the player input information to the server
            RelayInputServerRpc(forward, side, turn);

            // Mouse input (camera)
            float mouseY = -Input.GetAxisRaw("Mouse Y") * tiltSpeed * Time.deltaTime;
            float previousTilt = tilt;
            tilt += mouseY;
            tilt = Mathf.Clamp(tilt, minTilt, maxTilt); // Clamp the tilt values so the camera cannot rotate infinitely
            float targetTilt = tilt - previousTilt;
            fpcam.transform.Rotate(targetTilt, 0, 0); // Rotate the first person camera

            // Animation
            float animSpeed = new Vector2(forward, side).magnitude; // variable to set animation speed parameter based on forward & side input
            animator.SetFloat("Speed", animSpeed, 0.1f, Time.deltaTime); // set local animation
        }
        else
        {
            animator.SetFloat("Speed", netAnimSpeed.Value, 0.1f, Time.deltaTime);
        }
    }

    // Server-authoritative movement
    void FixedUpdate()
    {
        // Only the server should be handling this
        if (!IsServer) return;

        Vector3 move = transform.forward * serverForward + transform.right * serverSide;

        // Handle the rigidbody's movement
        rigidBody.MovePosition(rigidBody.position + move * walkSpeed * Time.fixedDeltaTime);

        // Handle the rigidbody's rotation
        rigidBody.MoveRotation(rigidBody.rotation * Quaternion.Euler(0, serverTurn * turnSpeed * Time.fixedDeltaTime, 0));

        // Update the server animation state
        netAnimSpeed.Value = new Vector2(serverForward, serverSide).magnitude;
    }

    // Server RPC for input
    [ServerRpc]
    void RelayInputServerRpc(float forward, float side, float turn)
    {
        serverForward = forward;
        serverSide = side;
        serverTurn = turn;
    }

    public override void OnNetworkSpawn()
    {
        isAlive = true; // The player is connected to, and present in, a session
        fpcam = GetComponentInChildren<Camera>();

        // If the player is in a session, disable the create & join interface and enable the leave & code interface
        uiManager = FindFirstObjectByType<UIManager>(); // find the UI Manager object
        uiManager.SessionJoinUpdateUI();

        if (IsOwner && fpcam != null) // If the client is the owner of the player character & the first person camera exists, disable the main camera, enable the player camera, and hide the character from the player
        {
            maincam = Camera.main;
            maincam.enabled = false;
            fpcam.enabled = true;
            SkinnedMeshRenderer[] renderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer renderer in renderers) renderer.enabled = false;
        }
        else // Disable the first person camera if the client is not the owner of the player character so players do not overlap cameras
        {
            fpcam.enabled = false;
        }
    }

    public override void OnNetworkDespawn()
    {
        isAlive = false;

        // if the player is not in a session, enable the create & join interface and disable the leave & code interface
        uiManager.SessionLeaveUpdateUI();

        if (IsOwner && fpcam != null && maincam != null)
        {
            fpcam.enabled = false;
            maincam.enabled = true;
        }
    }
}

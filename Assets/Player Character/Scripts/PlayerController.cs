using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;
using Unity.Multiplayer.Center.NetcodeForGameObjectsExample.DistributedAuthority;
using Unity.Netcode.Components;
using UnityEngine.UIElements;

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

    // Input variables
    float forward;
    float side;
    float mouseX;
    float mouseY;

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
        //Cursor.lockState = CursorLockMode.Confined;
        // Make the cursor invisible during gameplay - it will only be enabled when necessary, such as interacting with UI
        //Cursor.visible = false;

        // Get the player animator
        animator = GetComponent<Animator>();

        // Get the rigidbody
        rigidBody = GetComponent<Rigidbody>();

        // Prevent the player character from falling over
        rigidBody.freezeRotation = true;
    }

     // Update is called once per frame
    void Update()
    {
        // If the client is the owner of the player character, allow their inputs to control it - also sort out their animations & movement and transmit data through the network
        if (IsOwner)
        {
            // Keyboard input
            forward = Input.GetAxisRaw("Vertical");
            side = Input.GetAxisRaw("Horizontal");

            // Mouse input (camera)
            mouseX = Input.GetAxisRaw("Mouse X") * turnSpeed * Time.deltaTime;
            mouseY = -Input.GetAxisRaw("Mouse Y") * tiltSpeed * Time.deltaTime;
            float previousTilt = tilt;

            // Rotate the player
            transform.Rotate(Vector3.up * mouseX);

            // Rotate the player camemra
            tilt += mouseY;
            tilt = Mathf.Clamp(tilt, minTilt, maxTilt); // Clamp the tilt values so the camera cannot rotate infinitely
            fpcam.transform.localRotation = Quaternion.Euler(tilt, 0f, 0f);

            // Animation
            //float animSpeed = new Vector2(forward, side).magnitude;
            //animator.SetFloat("Speed", animSpeed, 0.1f, Time.deltaTime);
            //netAnimSpeed.Value = animSpeed;
        }
        else
        {
            animator.SetFloat("Speed", netAnimSpeed.Value, 0.1f, Time.deltaTime);
        }
    }

    // Server-authoritative movement
    void FixedUpdate()
    {
        if (!IsOwner) return;

        Vector3 move = transform.forward * forward + transform.right * side;

        Vector3 targetVelocity = move * walkSpeed;
        targetVelocity.y = rigidBody.linearVelocity.y;
        rigidBody.linearVelocity = targetVelocity;
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

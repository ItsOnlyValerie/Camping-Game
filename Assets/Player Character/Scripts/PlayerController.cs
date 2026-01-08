using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;
using Unity.Multiplayer.Center.NetcodeForGameObjectsExample.DistributedAuthority;

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

    public static bool isAlive;

    [SerializeField] private Camera fpcam; // first person camera
    [SerializeField] private Camera maincam; // main camera

    // Min & max tilt to stop the camera from rotating infinitely
    public float minTilt = -45f;
    public float maxTilt = 45f;

    // Network variable for the animator's speed parameter
    public NetworkVariable<float> netAnimSpeed = new NetworkVariable<float>();

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        //Cursor.visible = false;

        animator = GetComponent<Animator>(); // Get the player animator
    }

     // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            float forward = Input.GetAxisRaw("Vertical");
            float side = Input.GetAxisRaw("Horizontal");
            float turn = Input.GetAxisRaw("Mouse X");
            float mouseY = -Input.GetAxisRaw("Mouse Y") * tiltSpeed * Time.deltaTime;
            float previousTilt = tilt;
            tilt += mouseY;
            tilt = Mathf.Clamp(tilt, minTilt, maxTilt);
            float targetTilt = tilt - previousTilt;

            //transform.Translate(new Vector3(0, 0, forward * walkSpeed * Time.deltaTime)));
            //transform.Translate(new Vector3(side * walkSpeed * Time.deltaTime, 0, 0));
            //transform.Rotate(new Vector3(0, turn * turnSpeed * Time.deltaTime, 0));
            RelayInputServerRpc(forward * walkSpeed * Time.deltaTime, side * walkSpeed * Time.deltaTime, turn * turnSpeed * Time.deltaTime); // send movement to server
            float animSpeed = new Vector2(forward, side).magnitude; // variable to set animation speed parameter based on forward & side input
            animator.SetFloat("Speed", animSpeed, 0.1f, Time.deltaTime); // set local animation
            RelayAnimServerRpc(animSpeed); // set server animation
            fpcam.transform.Rotate(targetTilt, 0, 0); // rotate the first person camera
        }
        else
        {
            animator.SetFloat("Speed", netAnimSpeed.Value, 0.1f, Time.deltaTime);
        }
    }

    // server rpc for movement
    [ServerRpc]
    void RelayInputServerRpc(float forward, float side, float turn)
    {
        transform.Translate(new Vector3(0, 0, forward));
        transform.Translate(new Vector3(side, 0, 0));
        transform.Rotate(new Vector3(0, turn));
    }

    // server rpc for animations
    [ServerRpc]
    void RelayAnimServerRpc(float speed)
    {
        animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);
        netAnimSpeed.Value = speed;
    }

    public override void OnNetworkSpawn()
    {
        isAlive = true;
        fpcam = GetComponentInChildren<Camera>();

        // if the player is in a session, disable the create & join interface and enable the leave & code interface
        uiManager = FindFirstObjectByType<UIManager>(); // find the UI Manager object
        uiManager.SessionJoinUpdateUI();

        if (IsOwner && fpcam != null) // if the client is the owner of the player character & the first person camera exists, disable the main camera, enable the player camera, and hide the character from the player
        {
            maincam = Camera.main;
            maincam.enabled = false;
            fpcam.enabled = true;
            SkinnedMeshRenderer[] renderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer renderer in renderers) renderer.enabled = false;
        }
        else // disable the first person camera if the client is not the owner of the player character so players do not overlap cameras
        {
            fpcam.enabled = false;
        }

        /*if (IsServer)
        {
            transform.position = new Vector3(0, 1f, 0);
        }*/
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

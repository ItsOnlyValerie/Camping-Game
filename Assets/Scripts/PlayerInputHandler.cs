using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    // Setup a reference to the player input system
    private PlayerControls playerControls;

    private void Awake()
    {
        // Reference & create the player controls
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

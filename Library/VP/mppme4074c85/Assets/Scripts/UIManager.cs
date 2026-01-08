using UnityEngine;

public class UIManager : MonoBehaviour
{
    // reference the playerController script
    public PlayerController playerController;

    // reference the isAlive variable from the playerController
    bool isAlive = PlayerController.isAlive;

    // Reference the User Interface via the Inspector
    //[SerializeField] GameObject userInterface;
    [SerializeField] GameObject createSession;
    [SerializeField] GameObject joinSession;
    [SerializeField] GameObject leaveSession;
    [SerializeField] GameObject sessionCode;
    [SerializeField] GameObject playerName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // if the user interface is not active on startup, enable it
        //if (!userInterface.activeSelf) userInterface.SetActive(true);

        // create references to the User Interface session buttons
        //createSession = userInterface.transform.Find("Create Session");
        //joinSession = userInterface.transform.Find("Quick Join Session");
        //leaveSession = userInterface.transform.Find("Leave Session");
        //sessionCode = userInterface.transform.Find("Show Session Code");

        // disable the session code & leave session UI whilst not in a session - COME BACK TO THIS
        //sessionCode.gameObject.SetActive(false);
        //leaveSession.gameObject.SetActive(false);

        // Set up the UI once on start (main menu)
        createSession.gameObject.SetActive(true);
        joinSession.gameObject.SetActive(true);
        leaveSession.gameObject.SetActive(false);
        sessionCode.gameObject.SetActive(false);
        playerName.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        // If the player is in a session (networkSpawn), disable the create & join interface and enable the leave & code interface
        /*if (isAlive)
        {
            createSession.gameObject.SetActive(false);
            joinSession.gameObject.SetActive(false);
            sessionCode.gameObject.SetActive(true);
            leaveSession.gameObject.SetActive(true);
        }
        else if (!isAlive) // if the player is not in a session (networkDespawn), enable the create & join interface and disable the leave & code interface
        {
            createSession.gameObject.SetActive(true);
            joinSession.gameObject.SetActive(true);
            sessionCode.gameObject.SetActive(false);
            leaveSession.gameObject.SetActive(false);
        }*/
    }

    public void SessionJoinUpdateUI() // Update the UI to show/hide certain elements when connected to a session
    {
        createSession.gameObject.SetActive(false);
        joinSession.gameObject.SetActive(false);
        sessionCode.gameObject.SetActive(true);
        leaveSession.gameObject.SetActive(true);
        playerName.gameObject.SetActive(false);
    }

    public void SessionLeaveUpdateUI() // Update the UI to show/hide certain elements when not connected to a session (main menu)
    {
        createSession.gameObject.SetActive(true);
        joinSession.gameObject.SetActive(true);
        sessionCode.gameObject.SetActive(false);
        leaveSession.gameObject.SetActive(false);
        playerName.gameObject.SetActive(true);
    }
}

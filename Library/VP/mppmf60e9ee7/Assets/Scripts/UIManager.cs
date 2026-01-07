using UnityEngine;

public class UIController : MonoBehaviour
{
    // reference the playerController script
    public PlayerController playerController;

    // reference the isAlive variable from the playerController
    bool isAlive = PlayerController.isAlive;

    // set up variables to reference the User Interface session buttons
    Transform createSession;
    Transform joinSession;
    Transform leaveSession;
    Transform sessionCode;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // reference the User Interface gameObject
        GameObject userInterface = GameObject.Find("User Interface");

        // if the user interface is not active on startup, enable it
        if (!userInterface.activeSelf) userInterface.SetActive(true);

        // create references to the User Interface session buttons
        createSession = userInterface.transform.Find("Create Session");
        joinSession = userInterface.transform.Find("Quick Join Session");
        leaveSession = userInterface.transform.Find("Leave Session");
        sessionCode = userInterface.transform.Find("Show Session Code");

        // disable the session code & leave session UI whilst not in a session - COME BACK TO THIS
        sessionCode.gameObject.SetActive(false);
        leaveSession.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // if the player is in a session (networkDespawn), disable the create & join interface and enable the leave & code interface
        if (isAlive)
        {
            createSession.gameObject.SetActive(false);
            joinSession.gameObject.SetActive(false);
            sessionCode.gameObject.SetActive(true);
            leaveSession.gameObject.SetActive(true);
        }
        else // if the player is not in a session (networkSpawn), enable the create & join interface and disable the leave & code interface
        {
            createSession.gameObject.SetActive(true);
            joinSession.gameObject.SetActive(true);
            sessionCode.gameObject.SetActive(false);
            leaveSession.gameObject.SetActive(false);
        }
    }
}

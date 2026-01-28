using TMPro;
using UnityEngine;
using Unity.Netcode;

public class UIManager : NetworkBehaviour
{
    // Reference the playerController script
    public PlayerController playerController;

    // Reference the WoodMinigameHandler script
    public WoodMinigameHandler woodMinigameHandler;

    // Reference the User Interface via the Inspector
    //[SerializeField] GameObject userInterface;
    [SerializeField] GameObject createSession;
    [SerializeField] GameObject joinSession;
    [SerializeField] GameObject leaveSession;
    [SerializeField] GameObject sessionCode;
    [SerializeField] GameObject playerName;
    [SerializeField] TextMeshProUGUI woodMinigameText;
    [SerializeField] TextMeshProUGUI playerWoodScore;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Hide the wood minigame UI upon start
        woodMinigameText.gameObject.SetActive(false);
        playerWoodScore.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        WoodMinigameUI();
    }

    // Function for the wood minigame's related UI
    void WoodMinigameUI()
    {
        // If the player is inside the minigame's bounds
        if (woodMinigameHandler.insideBounds)
        {
            // Show the text
            woodMinigameText.gameObject.SetActive(true);

            // If the total number of players inside of the minigame's bounds is (not) equal to the total number of players in the session, set the text accordingly
            if (woodMinigameHandler.totalPlayers != GameManager.instance.playerCount)
            {
                woodMinigameText.text = "Waiting for other player(s)...";
            }
            else
            {
                woodMinigameText.text = $"All players are ready! Press F to begin!";
            }
        }
        // Otherwise, if the minigame has started or the player is not within the minigame's bounds, hide the text
        else if (woodMinigameHandler.minigameStarted || !woodMinigameHandler.insideBounds)
        {
            woodMinigameText.gameObject.SetActive(false);
        }

        // If the minigame has been started, show the player's score UI
        if (woodMinigameHandler.minigameStarted)
        {
            playerWoodScore.gameObject.SetActive(true);
        }
    }

    public void SessionJoinUpdateUI() // Update the UI to show/hide certain elements when connected to a session
    {

    }

    public void SessionLeaveUpdateUI() // Update the UI to show/hide certain elements when not connected to a session (main menu)
    {

    }
}

using TMPro;
using UnityEngine;
using Unity.Netcode;

public class UIManager : NetworkBehaviour
{
    // Reference the playerController script
    public PlayerController playerController;

    // Reference the WoodMinigameHandler script
    public WoodMinigameHandler woodMinigameHandler;

    // Reference the ScoreManager script
    public ScoreManager scoreManager;

    // Integer for the player's score when finding it in the ScoreManager's NetworkList
    int playerScore = 0;

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

        // If the WoodMinigameHandler exists, set the values of the following variables using the assigned functions
        if (woodMinigameHandler != null)
        {
            woodMinigameHandler.playerStartInput.OnValueChanged += PlayerStartInputChanged;
            woodMinigameHandler.minigameStarted.OnValueChanged += MinigameStartedChanged;
            woodMinigameHandler.minigameComplete.OnValueChanged += MinigameCompleteChanged;
        }
    }

    // Update is called once per frame
    void Update()
    {
        WoodMinigameUI();
    }

    // Function to get the winner of the minigame
    private PlayerScore GetWinner()
    {
        // If the ScoreManager does not exist or the scores NetworkList is empty, return default
        if (scoreManager == null || scoreManager.scores.Count == 0)
        {
            return default;
        }

        PlayerScore winner = scoreManager.scores[0];

        // Iterate through the scores NetworkList and find the highest score
        for (int i = 1; i  < scoreManager.scores.Count; i++)
        {
            if (scoreManager.scores[i].score > winner.score)
            {
                winner = scoreManager.scores[i];
            }
        }

        // Return the highest score
        return winner;
    }

    // Function for the wood minigame's related UI
    void WoodMinigameUI()
    {
        // If the minigame has started or the player is not within the minigame's bounds, hide the text
        if (woodMinigameHandler.minigameStarted.Value || woodMinigameHandler.playersInsideBounds.Value == 0)
        {
            woodMinigameText.gameObject.SetActive(false);
        }
        // Else, if the player is inside the minigame's bounds
        else if (woodMinigameHandler.playersInsideBounds.Value > 0)
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
                woodMinigameText.text = "All players are ready! Press F to begin!";
            }
        }

        // If the minigame has been completed, display the winner and hide the score text
        if (woodMinigameHandler.minigameComplete.Value)
        {
            // Hide the player's score text
            playerWoodScore.gameObject.SetActive(false);

            // Show the minigame text
            woodMinigameText.gameObject.SetActive(true);

            // Get the winner (highest score)
            PlayerScore winner = GetWinner();
            string winnerName = $"Player {winner.clientId}";

            // Set the minigame text to active and display the winner
            woodMinigameText.text = $"The winner is {winnerName} with {winner.score} wood!";
        }
        // If the minigame has been started and has not been completed, show the player's score UI
        if (woodMinigameHandler.minigameStarted.Value && !woodMinigameHandler.minigameComplete.Value)
        {
            playerWoodScore.gameObject.SetActive(true);

            // If the ScoreManager exists, search its NetworkList for the player's score and update the UI element accordingly
            if (scoreManager != null)
            {
                ulong localClientId = NetworkManager.Singleton.LocalClientId;

                // Find the player's score in the NetworkList
                
                foreach (var entry in scoreManager.scores)
                {
                    if (entry.clientId == localClientId)
                    {
                        playerScore = entry.score;
                        break;
                    }
                }
            }

            // Update the UI element
            playerWoodScore.text = $"Wood collected: {playerScore}";
        }
    }

    // Functions to handle the minigame UI across all clients
    private void PlayerStartInputChanged(bool oldValue, bool newValue)
    {
        WoodMinigameUI();
    }

    private void MinigameStartedChanged(bool oldValue, bool newValue)
    {
        WoodMinigameUI();
    }

    private void MinigameCompleteChanged(bool oldValue, bool newValue)
    {
        WoodMinigameUI();
    }

    public void SessionJoinUpdateUI() // Update the UI to show/hide certain elements when connected to a session
    {

    }

    public void SessionLeaveUpdateUI() // Update the UI to show/hide certain elements when not connected to a session (main menu)
    {

    }
}

using UnityEngine;
using Unity.Netcode;
using NUnit.Framework;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour
{
    // Setup the instance of the GameManager
    public static GameManager instance;

    // Integer to track how many players are in-game
    public int playerCount { get; private set; }

    // Reference the WoodMinigameHandler script
    public WoodMinigameHandler woodMinigameHandler;

    private void Awake()
    {
        // If a GameManager already exists, destroy it
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Reference the GameManager instance
        instance = this;

        // Don't destroy the GameManager on load as it is initialised in the main menu
        DontDestroyOnLoad(gameObject);
    }

    // Function for incrementing the player count if a player joins
    public void IncrementPlayerCount()
    {
        playerCount++;

        Debug.Log($"Player joined! Player count is now {playerCount}");
    }

    // Function for decrementing the player count if a player disconnects
    public void DecrementPlayerCount()
    {
        playerCount--;

        Debug.Log($"Player left! Player count is now {playerCount}");
    }
}

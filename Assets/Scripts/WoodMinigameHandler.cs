using UnityEngine;
using Unity.Netcode;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.Services.Multiplayer;
using UnityEngine.Rendering.Universal;

public class WoodMinigameHandler : NetworkBehaviour
{
    // Setup for the wood collection minigame
    [Header("Wood Minigame")]
    [SerializeField] GameObject woodPrefab;
    [SerializeField] GameObject WoodMinigameObject;
    public GameObject[] woodSpawnPoints;
    public List<GameObject> woodList = new List<GameObject>();

    // Boolean to communicate with the UI Manager as to whether or not a player is within the minigame's bounds
    public bool insideBounds = false;

    // Boolean to communicate with the GameManager as to whether or not the minigame has been started
    public bool minigameStarted = false;

    // Boolean to communicate with the NewPlayerController to allow input to start the minigame
    public bool playerStartInput = false;

    // Boolean to stop wood from being spawned once it has been spawned once already
    private bool woodSpawned = false;

    // Integer to count how many players are inside the boundaries
    public int totalPlayers = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Assign a reference to self in the GameManager
        GameManager.instance.woodMinigameHandler = this;
    }

    // Update is called once per frame
    void Update()
    {
        // Only the server should be handling this
        if (!IsServer) return;

        // If all players are within the minigame boundaries and the minigame hasn't already been started, set playerStartInput to true to allow the player to start the minigame
        if (totalPlayers == GameManager.instance.playerCount && !minigameStarted && playerStartInput == false)
        {
            playerStartInput = true;
        }

        // If the minigame has been started and the wood has not been spawned yet, instantiate the wood at each spawn point
        if (minigameStarted && !woodSpawned)
        {
            // For each spawn point, instantiate a collectable wood
            foreach (GameObject spawn in woodSpawnPoints)
            {
                GameObject wood = Instantiate(woodPrefab, spawn.transform.position, spawn.transform.rotation);
                wood.GetComponent<NetworkObject>().Spawn(true);
                woodList.Add(wood);
            }

            woodSpawned = true;
        }
    }

    // When a player enters the minigame boundaries, do the following
    private void OnTriggerEnter(Collider other)
    {
        // Only the server should be handling this
       // if (!IsServer) return;

        // If the object that's entered the trigger is not a player, return
        if (!other.CompareTag("Player")) return;

        // Debug log to see if player detection works correctly
        Debug.Log("Player has entered the minigame boundaries!");

        // Update the player count
        totalPlayers += 1;

        // Debug log to see if totalPlayers is being updated correctly
        Debug.Log($"totalPlayers = {totalPlayers}");

        // Set insideBounds to true so the UI Manager knows to display the related text
        insideBounds = true;
    }

    // When a player exits the minigame boundaries, do the following
    private void OnTriggerExit(Collider other)
    {
        // Only the server should be handling this
        //if (!IsServer) return;

        // If the object that's exited the trigger is not a player, return
        if (!other.CompareTag("Player")) return;

        // Debug log to see if player detection works correctly
        Debug.Log("Player has exited the minigame boundaries!");

        // Update the player count
        totalPlayers -= 1;

        // Debug log to see if totalPlayers is being updated correctly
        Debug.Log($"totalPlayers = {totalPlayers}");

        // Set insideBounds to false to hide the related text
        insideBounds = false;
    }
}

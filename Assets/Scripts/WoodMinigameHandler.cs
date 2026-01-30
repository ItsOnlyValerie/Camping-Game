using UnityEngine;
using Unity.Netcode;
using NUnit.Framework;
using System.Collections;
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

    // Network integer to count how many players are inside bounds
    public NetworkVariable<int> playersInsideBounds = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // Boolean to communicate with the GameManager as to whether or not the minigame has been started
    public NetworkVariable<bool> minigameStarted = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // Boolean to determine whether or not the minigame has finished
    public NetworkVariable<bool> minigameComplete = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // Boolean to communicate with the NewPlayerController to allow input to start the minigame
    public NetworkVariable<bool> playerStartInput = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // Boolean to stop wood from being spawned once it has been spawned once already
    private bool woodSpawned = false;

    // Boolean to ensure the restart couroutine is only executed once at any given time
    private bool coroutineStarted = false;

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
        if (totalPlayers == GameManager.instance.playerCount && !minigameStarted.Value && !playerStartInput.Value)
        {
            playerStartInput.Value = true;
        }

        // If the minigame has been started and the wood has not been spawned yet, instantiate the wood at each spawn point
        if (minigameStarted.Value && !woodSpawned)
        {
            // Debug log to see if wood is successfully being spawned
            Debug.Log("Spawning wood...");

            // Set the coroutine variable to false so the coroutine can be started
            coroutineStarted = false;

            // For each spawn point, instantiate a collectable wood
            foreach (GameObject spawn in woodSpawnPoints)
            {
                GameObject wood = Instantiate(woodPrefab, spawn.transform.position, spawn.transform.rotation);
                wood.GetComponent<NetworkObject>().Spawn(true);
                woodList.Add(wood);
            }

            woodSpawned = true;
        }

        // If the minigame has been started and the woodList is empty, declare the minigame as finished
        if (minigameStarted.Value && woodList.Count <= 0)
        {
            minigameComplete.Value = true;
        }

        // If the minigame has been completed, reset it after a brief delay
        if (minigameComplete.Value && !coroutineStarted)
        {
            StartCoroutine(ResetAfterDelay(5.0f));
            coroutineStarted = true;
        }
    }

    // Function to remove a wood prefab from the list if the player collides with it
    public void RemoveFromWoodList(GameObject wood)
    {
        if (woodList.Contains(wood))
        {
            woodList.Remove(wood);
        }
    }

    // Function to reset the minigame upon completion
    public void ResetMinigame()
    {
        // Reset all required variables
        minigameStarted.Value = false;
        minigameComplete.Value = false;
        playerStartInput.Value = false;
        woodSpawned = false;

        // Clear the wood list
        if (woodList != null)
        {
            // Despawn any remaining wood
            foreach (var wood in woodList)
            {
                if (wood != null)
                {
                    NetworkObject netObj = wood.GetComponent<NetworkObject>();
                    if (netObj != null)
                    {
                        netObj.Despawn(true);
                    }
                    else // Fallback in case it is not on the network
                    {
                        Destroy(wood);
                    }
                }
            }

            woodList.Clear();
        }

        // Reset player scores
        // Get the ScoreManager
        ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();

        // If the ScoreManager exists, iterate through its NetworkList and reset player scores
        if (scoreManager != null)
        {
            for (int i = scoreManager.scores.Count - 1; i >= 0; i++)
            {
                var entry = scoreManager.scores[i];
                entry.score = 0;
                scoreManager.scores[i] = entry;
            }
        }

        // Debugging log to indicate that it has been reset
        Debug.Log("Minigame has been reset and is ready to be played again.");
    }

    // Coroutine to restart the minigame after a brief delay
    private IEnumerator ResetAfterDelay(float delayInSeconds)
    {
        yield return new WaitForSeconds(delayInSeconds);

        if (IsServer)
        {
            ResetMinigame();
        }
    }

    // Server Rpc for starting the minigame using player input so that everyone can start it (not just the host player)
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void StartMinigameServerRpc(ulong clientId)
    {
        // Only works if the starting conditions are met
        if (!minigameStarted.Value && playerStartInput.Value && totalPlayers == GameManager.instance.playerCount)
        {
            minigameStarted.Value = true;
            Debug.Log($"Minigame started by client {clientId}");
        }
    }

    // Server Rpc for incrementing playersInsideBounds
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void PlayerEnteredBoundsServerRpc()
    {
        playersInsideBounds.Value += 1;
    }

    // Server Rpc for decrementing playersInsideBounds and ensuring that it never falls below 0
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void PlayerExitedBoundsServerRpc()
    {
        playersInsideBounds.Value -= 1;

        if (playersInsideBounds.Value < 0) playersInsideBounds.Value = 0;
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

        // Increment the bounds network variable (only the owner of the character does this)
        var playerController = other.GetComponent<NewPlayerController>();
        if (playerController.IsOwner) PlayerEnteredBoundsServerRpc();

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

        // Decrement the bounds network variable and use a safeguard to ensure that it doesn't fall below 0 (only the owner of the character does this)
        var playerController = other.GetComponent<NewPlayerController>();
        if (playerController.IsOwner) PlayerExitedBoundsServerRpc();

        if (playersInsideBounds.Value < 0) playersInsideBounds.Value = 0;

        // Update the player count
        totalPlayers -= 1;

        // Debug log to see if totalPlayers is being updated correctly
        Debug.Log($"totalPlayers = {totalPlayers}");

        // Set insideBounds to false to hide the related text
        insideBounds = false;
    }
}
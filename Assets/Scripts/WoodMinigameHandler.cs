using UnityEngine;
using Unity.Netcode;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.Services.Multiplayer;
using UnityEngine.Rendering.Universal;

public class WoodMinigameHandler : NetworkBehaviour
{
    // Boolean to communicate with the UI Manager as to whether or not a player is within the minigame's bounds
    public bool insideBounds = false;

    // Boolean to communicate with the GameManager as to whether or not the minigame has been started
    public bool minigameStarted = false;

    // Integer to count how many players are inside the boundaries
    public int totalPlayers = 0;

    // Set up a reference to the player controls
    private PlayerControls playerControls;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Reference the player controls
        playerControls = new PlayerControls();

        // Assign a reference to self in the GameManager
        GameManager.instance.woodMinigameHandler = this;
    }

    // Update is called once per frame
    void Update()
    {
        // If all players are within the minigame boundaries, then minigame hasn't already been started, and a player hits the interact key, start the minigame
        if (totalPlayers == GameManager.instance.playerCount && playerControls.Gameplay.Interact.IsPressed() && !minigameStarted)
        {
            minigameStarted = true;
        }

       /*if (minigameStarted)
        {
            foreach (GameObject wood in GameManager.instance.woodList)
            {

            }
        }*/
    }

    // On enable, enable the player controls
    private void OnEnable()
    {
        playerControls.Enable();
    }

    // On disable, disable the player controls
    private void OnDisable()
    {
        playerControls.Disable();
    }

    // When a player enters the minigame boundaries, do the following
    private void OnTriggerEnter(Collider other)
    {
        // Only the server should be handling this
        if (!IsServer) return;

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
        if (!IsServer) return;

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

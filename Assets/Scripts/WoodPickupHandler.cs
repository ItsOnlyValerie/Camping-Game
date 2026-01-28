using UnityEngine;
using Unity.Netcode;
using System.Threading;

public class WoodPickupHandler : NetworkBehaviour
{
    float rotationSpeed = 80.0f; // Rotation speed of the wood
    float floatSpeed = 1.5f; // Floating speed of the wood
    float floatHeight = 0.5f; // Maximum height the wood should float above its original position
    float spawnTimer = 10.0f; // Timer for cooldown on wood respawning
    public bool woodCollected = false; // Boolean to detect when a piece of wood has been collected

    Vector3 originalPos; // Original position of the wood

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Store the object's original position
        originalPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate the wood
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Float the wood up & down
        float newY = originalPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight / 2;
        transform.position = new Vector3(originalPos.x, newY, originalPos.z);

    }

    private void OnTriggerEnter(Collider other) // If a player enters the wood's Box Collider, destroy it and update the player's score for this minigame - FULL FUNCTIONALITY TO BE ADDED
    {
        if (!IsServer) return; // Only the server should be handling this - not the client

        // Get the player NetworkObject
        if (!other.TryGetComponent(out NetworkObject playerNetObj)) return;

        // If the other object is not a player, return
        if (!playerNetObj.CompareTag("Player")) return;

        // Get the player's client ID
        ulong playerId = playerNetObj.OwnerClientId;

        // Debug log to test if the client ID is being obtained
        Debug.Log($"Wood has been touched by client {playerId}");

        // Get the PlayerScoreManager component and call the AddScore() function in order to update the player's score
        var playerScore = playerNetObj.GetComponent<PlayerScoreManager>();
        playerScore.AddScore(1);

        // Debug log to see if the player's score is being updated successfully
        Debug.Log($"Client {playerId}'s score is now {playerScore.score.Value}");

        // Despawn the object for everyone on the network
        NetworkObject.Despawn(true);

        // Despawn the wood for everyone
        NetworkObject.Despawn(true);

        // Count down the timer
        //spawnTimer -= Time.deltaTime;

        // Respawn the object for everyone on the network
        //if (spawnTimer <= 0) NetworkObject.Spawn();

        // Reset the timer
        //spawnTimer = 10.0f;
    }
}

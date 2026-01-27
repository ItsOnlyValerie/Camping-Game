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

        if (other.CompareTag("Player")) // If the other object is a player, allow interaction
        {
            NetworkObject.Despawn(); // Despawn the object for everyone on the network
            //spawnTimer -= Time.deltaTime; // Count down the timer
            //if (spawnTimer <= 0) NetworkObject.Spawn(); // Respawn the object for everyone on the network
            //spawnTimer = 10.0f; // Reset the timer
        }
    }
}

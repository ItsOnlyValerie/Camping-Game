using UnityEngine;
using Unity.Netcode;
using System.Threading;

public class WoodPickupHandler : NetworkBehaviour
{
    // Rotation speed of the wood
    float rotationSpeed = 80.0f;

    // Floating speed of the wood
    float floatSpeed = 1.5f;

    // Maximum height the wood should float above its original position
    float floatHeight = 0.5f;

    // Timer for cooldown on wood respawning
    float spawnTimer = 10.0f;

    // Boolean to detect when a piece of wood has been collected
    public bool woodCollected = false;

    // Original position of the wood
    Vector3 originalPos;

    // Reference the WoodMinigameHandler
    private WoodMinigameHandler woodMinigameHandler;

    // Reference the wood's audio source
    public AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Store the object's original position
        originalPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        // Initialise the WoodMinigameHandler reference
        woodMinigameHandler = FindFirstObjectByType<WoodMinigameHandler>();

        // Initialise the audio source
        audioSource = GetComponent<AudioSource>();
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

    // If a player enters the wood's Box Collider, destroy it and update the player's score for this minigame
    private void OnTriggerEnter(Collider other)
    {
        // Only the server should be handling this
        if (!IsServer) return;

        // Get the player NetworkObject
        if (!other.TryGetComponent(out NetworkObject playerNetObj)) return;

        // If the other object is not a player, return
        if (!playerNetObj.CompareTag("Player")) return;

        // Get the player's client ID
        ulong playerId = playerNetObj.OwnerClientId;

        // Debug log to test if the client ID is being obtained
        Debug.Log($"Wood has been touched by client {playerId}");

        // Get the ScoreManager
        ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();

        // Debug log to check if ScoreManager exists
        if (scoreManager == null)
        {
            Debug.LogError("ScoreManager not found!");
            return;
        }

        // Update the player's score using their Id as a reference to their entry in the NetworkList
        scoreManager.AddScore(playerId, 1);

        // Debugging to see if the player's score is being updated successfully
        int playerScore = 0;
        foreach (var entry in scoreManager.scores)
        {
            if (entry.clientId == playerId)
            {
                playerScore = entry.score;
                break;
            }
        }

        Debug.Log($"Client {playerId}'s score is now {playerScore}");

        // Play the pickup SFX from the audio source
        audioSource.Play();

        // Remove the object from the woodList in WoodMinigameHandler
        woodMinigameHandler.RemoveFromWoodList(this.gameObject);

        // Despawn the object for everyone on the network
        if (NetworkObject != null)
        {
            NetworkObject.Despawn(true);
        }
        else // Fallback in case it is not networked
        {
            Destroy(gameObject);
        }
    }
}

using UnityEngine;
using Unity.Netcode;
using NUnit.Framework;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour
{
    // Setup for the wood collection minigame
    [Header("Wood Minigame")]
    [SerializeField] GameObject woodPrefab;
    [SerializeField] GameObject WoodMinigameObject;
    public GameObject[] woodSpawnPoints;
    public List<GameObject> woodList = new List<GameObject>();

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // If the wood minigame has been started, spawn the wood once
        if (woodMinigameHandler.minigameStarted)
        {
            WoodMinigameInit();
        }
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

    // Function for the wood minigame's initial setup
    void WoodMinigameInit()
    {
        // For each spawn point, instantiate a collectable wood
        foreach (GameObject spawn in woodSpawnPoints)
        {
            GameObject wood = Instantiate(woodPrefab, spawn.transform.position, spawn.transform.rotation);
            wood.GetComponent<NetworkObject>().Spawn(true);
            woodList.Add(wood);
        }
    }
}

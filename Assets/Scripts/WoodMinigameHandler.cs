using UnityEngine;
using Unity.Netcode;
using NUnit.Framework;
using System.Collections.Generic;

public class WoodMinigameHandler : NetworkBehaviour
{
    public List<GameObject> woodSpawnList; // Create a list of type GameObject to store the spawn points in
    [SerializeField] private GameObject woodPrefab; // Reference the prefab for the wood
    int totalWood; // Counter for how much wood is currently spawned


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Set totalWood to 0
        totalWood = 0;

        // For each spawn point, instantiate a collectable piece of wood
        foreach (GameObject spawn in woodSpawnList)
        {
            Instantiate(woodPrefab, spawn.transform.position, spawn.transform.rotation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer) // Only the server should be handling this
        {

        }
    }
}

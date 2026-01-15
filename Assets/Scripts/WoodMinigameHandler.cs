using UnityEngine;
using Unity.Netcode;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.Services.Multiplayer;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer) // Only the server should be handling this
        {

        }
    }

   public void woodSpawn()
    {
        Debug.Log("woodSpawn() was called. If the objects have not appeared, something didn't work!");
        // For each spawn point, instantiate a collectable piece of wood
        foreach (GameObject spawn in woodSpawnList)
        {
            GameObject wood = Instantiate(woodPrefab, spawn.transform.position, spawn.transform.rotation);
            wood.GetComponent<NetworkObject>().Spawn(true);
        }
        
    }

    public void woodJoining()
    {
        Debug.Log("woodJoining() was called.");


    }

    public void woodFail()
    {
        Debug.Log("woodFail() was called.");


    }
}

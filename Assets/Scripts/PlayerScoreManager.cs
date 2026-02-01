using UnityEngine;
using Unity.Netcode;

public class PlayerScoreManager : NetworkBehaviour
{
    // Setup an integer network variable for the player's score that can only be written to by the server
    public NetworkVariable<int> score = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Function to update the player's score upon collecting a piece of wood in the wood minigame
    /*(public void AddScore(int amount)
    {
        if (IsServer)
        {
            // Host can apply this immediately
            score.Value += amount;
        }
        else
        {
            AddScoreServerRpc(amount);
        }
    }*/

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void AddScoreServerRpc(int amount)
    {
        score.Value += amount;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer) Debug.Log($"Score write permission = {score.WritePerm}");
    }    
}

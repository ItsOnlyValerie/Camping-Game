using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class PlayerName : NetworkBehaviour
{
    // Set up a variable to store the player's name
    public NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    // Upon spawning, set the player's name
    public override void OnNetworkSpawn()
    {
        playerName.Value = PlayerPrefs.GetString("PlayerName", $"Player {OwnerClientId}");
    }
}

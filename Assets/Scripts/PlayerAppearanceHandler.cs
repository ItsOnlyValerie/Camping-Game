using UnityEngine;
using Unity.Netcode;

public class PlayerAppearanceHandler : NetworkBehaviour
{
    // Set up a reference to the player's renderer
    public Renderer playerRenderer;

    // Network variable for the skin the player selected on the main menu
    private NetworkVariable<PlayerSkin> playerSkinVar = new NetworkVariable<PlayerSkin>(PlayerSkin.Skin1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // List of skins
    private static readonly PlayerSkin[] SkinList = new PlayerSkin[]
    {
        PlayerSkin.Skin1, // Red
        PlayerSkin.Skin2, // White
        PlayerSkin.Skin3, // Blue
        PlayerSkin.Skin4 // Green
    };

    // Upon spawning the player, call the Server Rpc to update the skin option stored in the network variable, as well as other functions to actually apply it
    public override void OnNetworkSpawn()
    {
        // Store any changes in the skin value
        playerSkinVar.OnValueChanged += PlayerSkinChanged;

        // Only the server assigns skins
        if (IsServer)
        {
            ApplyUniqueSkin();
        }

        // Set the current skin locally
        SetSkin(playerSkinVar.Value);
    }

    // Function to set the player's skin
    private void SetSkin(PlayerSkin playerSkin)
    {
        switch (playerSkin)
        {
            case PlayerSkin.Skin1:
                playerRenderer.material.color = Color.red;
                break;

            case PlayerSkin.Skin2:
                playerRenderer.material.color = Color.white;
                break;

            case PlayerSkin.Skin3:
                playerRenderer.material.color = Color.blue;
                break;

            case PlayerSkin.Skin4:
                playerRenderer.material.color = Color.green;
                break;
        }
    }

    // Function to call the ApplySkin() function
    private void PlayerSkinChanged(PlayerSkin oldSkin, PlayerSkin newSkin)
    {
        SetSkin(newSkin);
        Debug.Log($"Skin changed from {oldSkin} to {newSkin}");
    }

    // Server Rpc to automatically assign a new skin based on join order
    //[Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void ApplyUniqueSkin()
    {
        // Create a list of all connected clients
        var connectedClients = NetworkManager.Singleton.ConnectedClientsList;

        // Find the owner's index
        int playerIndex = 0;
        for (int i = 0; i < connectedClients.Count; i++)
        {
            if (connectedClients[i].ClientId == OwnerClientId)
            {
                playerIndex = i;
                break;
            }
        }

        // Assign a skin automatically based on the index/join order with a debug log to indicate which skin was applied to the player
        PlayerSkin assignedSkin = SkinList[playerIndex % SkinList.Length];
        playerSkinVar.Value = assignedSkin;
        Debug.Log($"Assigned skin {assignedSkin} to player.");
    }
}

using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;

public class PlayerNameUI : NetworkBehaviour
{
    // Set up the player name UI
    [SerializeField] private TextMeshProUGUI playerNameText;

    // Reference the player name script
    private PlayerName playerName;

    private void Awake()
    {
        // Initialise the player name script
        playerName = GetComponent<PlayerName>();
    }

    // Upon spawning, update the player's name
    public override void OnNetworkSpawn()
    {
        UpdatePlayerName(playerName.playerName.Value);

        playerName.playerName.OnValueChanged += PlayerNameChanged;
    }

    // Upon despawn/being destroyed, remove the player's name
    private void OnDestroy()
    {
        if (playerName != null)
        {
            playerName.playerName.OnValueChanged -= PlayerNameChanged;
        }
    }

    // Function to update the player's name
    private void UpdatePlayerName(FixedString32Bytes newName)
    {
        playerNameText.text = newName.ToString();
    }

    // Function to call the UpdateName function
    private void PlayerNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName)
    {
        UpdatePlayerName(newName);
    }
}

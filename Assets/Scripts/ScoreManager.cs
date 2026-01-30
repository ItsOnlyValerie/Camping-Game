using UnityEngine;
using Unity.Collections;
using Unity.Netcode;
using System.Collections.Generic;
using System;

// PlayerScore struct for referencing the players and managing their scores
public struct PlayerScore : INetworkSerializable, IEquatable<PlayerScore>
{
    // Variables required to identify the client correctly and increment their score
    public ulong clientId;
    public int score;

    public bool Equals(PlayerScore other)
    {
        return clientId == other.clientId && score == other.score;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(clientId, score);
    }

    // Serialize it on the network
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref score);
    }
}

public class ScoreManager : NetworkBehaviour
{
    // Create a NetworkList based on the PlayerScore struct
    public NetworkList<PlayerScore> scores;

    private void Awake()
    {
        scores = new NetworkList<PlayerScore>(null, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    }

    public void AddScore(ulong clientId, int amount)
    {
        // Only the server should be handling this
        if (!IsServer) return;

        // Boolean to ensure that score is incremented successfully
        bool found = false;

        // Iterate through the entries in the NetworkList and update the player's score if their clientId matches
        for (int i = 0; i < scores.Count; i++)
        {
            if (scores[i].clientId == clientId)
            {
                scores[i] = new PlayerScore { clientId = clientId, score = scores[i].score + amount };

                found = true;
                break;
                /*var entry = scores[i];
                entry.score = amount;
                scores[i] = entry;
                return;*/
            }
        }

        if (!found)
        {
            scores.Add(new PlayerScore { clientId = clientId, score = amount });
        }
        // Add a new score
        //scores.Add(new PlayerScore { clientId = clientId, score = amount });
    }

    // Only for debugging purposes
    public override void OnNetworkSpawn()
    {
        // Only the server should be handling this
        if (!IsServer) return;

        Debug.Log("ScoreManager was spawned on the server.");
    }
}

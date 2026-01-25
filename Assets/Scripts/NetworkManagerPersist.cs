using UnityEngine;

public class NetworkManagerPersist : MonoBehaviour
{
    private void Awake()
    {
        // Ensure the NetworkManager persists between scenes
        DontDestroyOnLoad(gameObject);
    }
}

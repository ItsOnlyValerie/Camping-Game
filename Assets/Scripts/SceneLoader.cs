using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class SceneLoader : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Function to swap to the game world scene
    public void openGameScene()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Game World", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}

using UnityEngine;

public class PlayerSkinSelectcion : MonoBehaviour
{
    // Initialise a variable to store the skin the player selected
    public static PlayerSkin selectedSkin = PlayerSkin.Skin1;

    // Function to select skin 1
    public void Skin1Selected()
    {
        selectedSkin = PlayerSkin.Skin1;
    }

    // Function to select skin 2
    public void Skin2Selected()
    {
        selectedSkin = PlayerSkin.Skin2;
    }

    // Function to select skin 3
    public void Skin3Selected()
    {
        selectedSkin = PlayerSkin.Skin3;
    }

    // Function to select skin 4
    public void Skin4Selected()
    {
        selectedSkin = PlayerSkin.Skin4;
    }
}

using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerNameBillboard : MonoBehaviour
{
    // Variable for the player camera's transform
    private Transform cameraTransform;

    // Make the name face the player camera
    private void LateUpdate()
    {
        if (cameraTransform == null)
        {
            var cameraBrain = FindAnyObjectByType<CinemachineBrain>();

            if (cameraBrain == null || cameraBrain.OutputCamera == null) return;

            cameraTransform = cameraBrain.transform;
        }

        transform.forward = cameraTransform.forward;
    }
}

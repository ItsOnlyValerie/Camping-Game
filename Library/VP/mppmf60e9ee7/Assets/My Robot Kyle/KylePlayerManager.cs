// --------------------------------------------------------------------------------------------------------------------
// Roughly based on Photon Unity Networking Demos
// Adaped for Kingston University students
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;

public class KylePlayerManager : MonoBehaviour
{
    Animator animator;

	void Start () 
	{
	    animator = GetComponent<Animator>();
	}
	        
	void Update () 
	{
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        v = Mathf.Max(v, 0);    // prevent negative speed ("S" key)

        animator.SetFloat("Speed", h*h+v*v);
        animator.SetFloat("Direction", h, 0.25f, Time.deltaTime);
	}
}

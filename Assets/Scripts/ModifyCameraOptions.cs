using UnityEngine;
using System.Collections;

public class ModifyCameraOptions : MonoBehaviour {

    public float newDistance = 0f;
    public Vector3 newRotation = new Vector3();

    private Vector3 savedRotation;
    private float savedDistance;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Camera cam = Camera.main;
            ChasePlayer chase = cam.GetComponent<ChasePlayer>();

            // Save current state
            savedRotation = chase.rotationVector;
            savedDistance = chase.distance;

            // Rotate to new Stuff
            chase.rotationVector = newRotation;
            chase.distance = newDistance;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if( other.gameObject.tag == "Player")
        {
            Camera cam = Camera.main;
            ChasePlayer chase = cam.GetComponent<ChasePlayer>();
            chase.rotationVector = savedRotation;
            chase.distance = savedDistance;

            // Reset
            savedRotation = Vector3.zero;
            savedDistance = 0f;
        }
    }
}

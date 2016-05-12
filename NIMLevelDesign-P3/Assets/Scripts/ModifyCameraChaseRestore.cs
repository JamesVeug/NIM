using UnityEngine;
using System.Collections;

public class ModifyCameraChaseRestore : MonoBehaviour {

    // When we enter this Trigger. What should we change to?
    public Camera cameraToEdit;
    public bool modifyCameraRotation1Enter = false;
    public float newDistance1Enter = 0f;
    public Vector3 newRotation1Enter = new Vector3();
    public bool newChasePlayer1Enter = false;
    public bool newChaseX1Enter = false;
    public bool newChaseY1Enter = false;
    public bool newChaseZ1Enter = false;

    // When we leave exit 1. What angle should we leave?
    private float newDistance1Exit = 0f;
    private Vector3 newRotation1Exit = new Vector3();
    private bool newChasePlayer1Exit = false;
    private bool newChaseX1Exit = false;
    private bool newChaseY1Exit = false;
    private bool newChaseZ1Exit = false;

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
            ChasePlayer chase = cameraToEdit.GetComponent<ChasePlayer>();


            // Save
            newRotation1Exit = chase.rotationVector;
            newDistance1Exit = chase.distance;
            newChasePlayer1Exit = chase.enableChase;
            newDistance1Exit = chase.distance;
            newChaseX1Exit = chase.chaseX;
            newChaseY1Exit = chase.chaseY;
            newChaseZ1Exit = chase.chaseZ;

            // Rotate to new Stuff
            if (modifyCameraRotation1Enter)
            {
                chase.rotationVector = newRotation1Enter;
                chase.distance = newDistance1Enter;
            }
            chase.enableChase = newChasePlayer1Enter;
            chase.chaseX = newChaseX1Enter;
            chase.chaseY = newChaseY1Enter;
            chase.chaseZ = newChaseZ1Enter;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if( other.gameObject.tag == "Player")
        {
            ChasePlayer chase = cameraToEdit.GetComponent<ChasePlayer>();
            chase.rotationVector = newRotation1Exit;
            chase.distance = newDistance1Exit;
            chase.enableChase = newChasePlayer1Exit;
            chase.chaseX = newChaseX1Exit;
            chase.chaseY = newChaseY1Exit;
            chase.chaseZ = newChaseZ1Exit;
        }
    }
    
}

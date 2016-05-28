using UnityEngine;
using System.Collections;

public class CheckpointPlayer : MonoBehaviour {

    public Checkpoints currentCheckpoint;
    public Movement movementScript;
	// Use this for initialization
	void Start () {

        if (currentCheckpoint == null) {
            Debug.Log("No checkpoint provided for player");
        }
	}

    public void setCheckpoint(Checkpoints checkpoint) {
        currentCheckpoint = checkpoint;
        Debug.Log("NEW CHECKPOINT: " + checkpoint.name);
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        //Debug.Log("TRANSFORM Y: " + this.transform.position.y );
        if (this.transform.position.y < -10) {
            transform.position = currentCheckpoint.transform.position;
            movementScript.currentMovementWaypoint = currentCheckpoint.getWaypoint();

        }
	}
}

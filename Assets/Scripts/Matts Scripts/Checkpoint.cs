using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

    public CheckpointPlayer playerScript;
    private MovementWaypoint waypoint;
    // Use this for initialization

    void Start()
    {
		playerScript = FindObjectOfType<CheckpointPlayer> ();
        waypoint = this.GetComponent<MovementWaypoint>();

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerScript.setCheckpoint(this);
        }
        else {
            Debug.Log("NO PLAYER TAG FOUND FOR CHECKPOINT");
        }


    }

    public MovementWaypoint getWaypoint() {
        return waypoint;
    }

}

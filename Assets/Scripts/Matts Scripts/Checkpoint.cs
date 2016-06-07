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
        Debug.Log("WAYPOINT: " + waypoint.ToString());

    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("TEST1");
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

using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

    public AIBuzzer[] buzzerAI;
    private CheckpointPlayer playerScript;
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
            if (buzzerAI != null)
            {
                foreach(AIBuzzer b in buzzerAI)
                    b.spanRange();
            }
            
        }
        else {
            Debug.Log("NO PLAYER TAG FOUND FOR CHECKPOINT");
        }


    }

    public MovementWaypoint getWaypoint() {
        return waypoint;
    }

}

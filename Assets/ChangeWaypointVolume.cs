using UnityEngine;
using System.Collections;

public class ChangeWaypointVolume : MonoBehaviour {

    public static bool drawText = true; // Draw the names of the Waypoints we will change to
    public MovementWaypoint newPreviousWaypoint;

	// Use this for initialization
	void Start () {
	
	}

        void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag.Equals("Player"))
        {
            Movement move = other.gameObject.GetComponent<Movement>();
            move.currentMovementWaypoint = newPreviousWaypoint;
        }
        else
        {
            Debug.Log("Not player '" + other.gameObject.tag + "'");
        }
    }
    
    void OnDrawGizmos()
    {
        if (drawText)
        {
            string text = newPreviousWaypoint == null ? "null" : newPreviousWaypoint.name;
            TextGizmo.Instance.DrawText(Camera.main, transform.position, text);
        }
    }
}

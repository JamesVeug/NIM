using UnityEngine;
using System.Collections;

public class ChangeWaypointVolume : MonoBehaviour {

    public static bool drawText = true; // Draw the names of the Waypoints we will change to
    public MovementWaypoint newPreviousWaypoint;


    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag.Equals("Player"))
        {
            Movement move = other.gameObject.GetComponent<Movement>();
            move.currentMovementWaypoint = newPreviousWaypoint;
        }
    }
    
    void OnDrawGizmos()
    {
        if (drawText)
        {
            string text = newPreviousWaypoint == null ? "null" : newPreviousWaypoint.name;
            //TextGizmo.Instance.DrawText(Camera.main, transform.position, text);
        }
    }
}

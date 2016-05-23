using UnityEngine;
using System.Collections;

public class PhaseVolume : MonoBehaviour
{
    // ONLY FOR DEBUGGING
    public static bool drawLinks = true;
    private Vector3 drawOffset = new Vector3(0f, 0.25f, 0f);

    public MovementWaypoint nextPhaseWaypoint;
    public MovementWaypoint previousPhaseWaypoint;
    

    void OnTriggerEnter(Collider other)
    {
        PhaseJump jump = other.gameObject.GetComponent<PhaseJump>();
        if( jump != null)
        {
            jump.setPhaseVolume(this);
        }
    }

    void OnTriggerExit(Collider other)
    {
        PhaseJump jump = other.gameObject.GetComponent<PhaseJump>();
        if (jump != null && jump.getPhaseVolume() == this )
        {
            jump.setPhaseVolume(null);
        }
    }
    
    // Use this for initialization
    void OnDrawGizmos()
    {

        if (!drawLinks)
        {
            return;
        }
        

        // Next position to phase to
        if (nextPhaseWaypoint != null)
        {
            makeLine(this.transform.position, nextPhaseWaypoint.transform.position, Color.blue, drawOffset);
            //if( nextPhasePoint.phaseLayer == this.phaseLayer) { Debug.LogWarning("The phaselayer for " + this.name + " is the same for it's nextPhasePoint!"); }

        }

        // Previous point to phase to
        if (previousPhaseWaypoint != null)
        {
            makeLine(this.transform.position, previousPhaseWaypoint.transform.position, Color.yellow, -drawOffset);
            //if (previousPhasePoint.phaseLayer == this.phaseLayer) { Debug.LogWarning("The phaselayer for " + this.name + " is the same for it's previousPhasePoint!"); }
        }


        BoxCollider collider = GetComponent<BoxCollider>();
        if(nextPhaseWaypoint != null && collider.bounds.Contains(nextPhaseWaypoint.transform.position))
        {
            Debug.LogWarning("PhaseVolume " + name + " has a nextPhasePoint within itself!");
        }
        if (previousPhaseWaypoint != null && collider.bounds.Contains(previousPhaseWaypoint.transform.position))
        {
            Debug.LogWarning("PhaseVolume " + name + " has a previousPhasePoint within itself!");
        }
    }

    // Draw a visible line that can be seen in the editor and in game
    private void makeLine(Vector3 start, Vector3 end, Color color, Vector3 offset)
    {
        Gizmos.color = color;
        Gizmos.DrawLine(start - offset, end - offset);
    }
}

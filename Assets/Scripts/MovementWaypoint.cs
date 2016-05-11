using UnityEngine;
using System.Collections;

public class MovementWaypoint : MonoBehaviour {

    // ONLY FOR DEBUGGING
    public static bool drawLinks = true;

    public int phaseLayer = 0;

    public MovementWaypoint previous;
    public MovementWaypoint next;

    public MovementWaypoint nextPhasePoint;
    public MovementWaypoint previousPhasePoint;


    // Drawing lines
    private Vector3 drawOffset = new Vector3(0f, 0.25f, 0f);


    // Use this for initialization
    void Start () {
	
        if( !drawLinks)
        {
            return;
        }

        
        // Next point for the character to walk to
        if ( next != null)
        {
            makeLine(this, next, Color.red, drawOffset);
            if (next.phaseLayer != this.phaseLayer) { Debug.LogWarning("The phaselayer for " + this.name + " is NOT same for it's next point!"); }
        }

        // When we walk left, it will walk here
        if (previous != null)
        {
            makeLine(this, previous, Color.green, -drawOffset);
            if (previous.phaseLayer != this.phaseLayer) { Debug.LogWarning("The phaselayer for " + this.name + " is NOT same for it's previous point!"); }
        }

        // Next position to phase to
        if (nextPhasePoint != null)
        {
            makeLine(this, nextPhasePoint, Color.blue, drawOffset);
            if( nextPhasePoint.phaseLayer == this.phaseLayer) { Debug.LogWarning("The phaselayer for " + this.name + " is the same for it's nextPhasePoint!"); }

        }

        // Previous point to phase to
        if (previousPhasePoint != null)
        {
            makeLine(this, previousPhasePoint, Color.yellow, -drawOffset);
            if (previousPhasePoint.phaseLayer == this.phaseLayer) { Debug.LogWarning("The phaselayer for " + this.name + " is the same for it's previousPhasePoint!"); }
        }
    }

    // Draw a visible line that can be seen in the editor and in game
    public void makeLine(MovementWaypoint start, MovementWaypoint end, Color color, Vector3 offset)
    {
        GameObject o = new GameObject();
        LineRenderer line = o.AddComponent<LineRenderer>();
        line.SetWidth(0.1f, 0.1f);
        line.SetPositions(new Vector3[] { start.transform.position - offset, end.transform.position - offset });
        line.material.color = color;
    }
	
	// Update is called once per frame
	void Update () {
    }
}

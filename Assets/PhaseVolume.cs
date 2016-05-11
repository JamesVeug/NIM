using UnityEngine;
using System.Collections;

public class PhaseVolume : MonoBehaviour {

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
}

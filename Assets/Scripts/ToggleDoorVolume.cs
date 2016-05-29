using UnityEngine;
using System.Collections;

public class ToggleDoorVolume : MonoBehaviour {
    public Door door;
    public bool TriggerOnlyOnce = true;
    private bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if( triggered && TriggerOnlyOnce)
        {
            return;
        }
        triggered = true;

        // Flip between them
        door.open = !door.open;
    }
    
}

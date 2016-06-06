using UnityEngine;
using System.Collections;

public class PressureTrigger : MonoBehaviour {

    public PressurePlate plate;
    public bool triggerOnExit = true;
    int count = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnTriggerEnter(Collider other)
    {
        
        count++;
        if (count == 1) {
            plate.setTriggerReact(true);
        }
        
    }

    public void OnTriggerExit(Collider other)
    {

        count--;
        if (count == 0) {
            if (triggerOnExit == true)
            {
                plate.setTriggerReact(false);
            }
            else {
                plate.setTriggerNoReact(false);
            }
            
        }
    }
}

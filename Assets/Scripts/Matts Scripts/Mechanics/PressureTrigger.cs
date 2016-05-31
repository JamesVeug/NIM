using UnityEngine;
using System.Collections;

public class PressureTrigger : MonoBehaviour {

    public PressurePlate plate;
    int count = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("TEST");
        
        count++;
        if (count == 1) {
            plate.setTrigger(true);
        }
        
    }

    public void OnTriggerExit(Collider other)
    {

        count--;
        if (count == 0) {
            plate.setTrigger(false);
        }
    }
}

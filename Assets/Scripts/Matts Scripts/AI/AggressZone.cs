using UnityEngine;
using System.Collections;

public class AggressZone : MonoBehaviour {

    public AIPatrol patrolScript;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnTriggerEnter(Collider other)
    {
       
        if (other.gameObject.tag == "Player")
        {
            patrolScript.setAggression(true);
        }
        else {
            Debug.Log("NO PLAYER TAG FOUND FOR CHECKPOINT");
        }


    }
}

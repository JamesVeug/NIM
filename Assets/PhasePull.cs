using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PhasePull : MonoBehaviour {

    public List<GameObject> inCollider = new List<GameObject>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer == 8)
        {
            inCollider.Add(other.gameObject);
            Debug.Log("Name " + other.gameObject.name);
            Debug.Log("Added " + inCollider.Count);
        }
        
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            inCollider.Remove(other.gameObject);
            Debug.Log("Name " + other.gameObject.name);
            Debug.Log("Removed " + inCollider.Count);
        }
    }
}

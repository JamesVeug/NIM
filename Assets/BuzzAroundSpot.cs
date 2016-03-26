using UnityEngine;
using System.Collections;

public class BuzzAroundSpot : MonoBehaviour {

    // What to chase
    public GameObject whatToChase;

    // Start looking at the object
    public bool startLookingObject = true;

    // Speed to chase
    public float chaseSpeed = 1f;
    public float rotateSpeed = 0.5f;

    public float xMinDistance = 0;
    public float xMaxDistance = 0;

    public float yMinDistance = 0;
    public float yMaxDistance = 0;

    public float zMinDistance = 0;
    public float zMaxDistance = 0;

    public float minDistance = 2;
    public float maxDistance = 10;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        // Make sure we have something to chase
        if (whatToChase == null)
        {
            Debug.LogWarning("No object assigned to chase!");
            return;
        }

        
        Vector3 pPos = whatToChase.transform.position; // WhatToChase's Position
        Vector3 tPos = this.transform.position-new Vector3(minDistance, minDistance, minDistance); // This Objects Position
        float distance = (tPos - pPos).magnitude; // Distance from This to the Object

        // Get rotation to object
        Quaternion targetRotation = Quaternion.LookRotation(whatToChase.transform.position - transform.position);
        float str = Mathf.Min(rotateSpeed * Time.deltaTime * (maxDistance/distance), 1);

        // Get faster if we exceed the distance
        float finalChaseSpeed = chaseSpeed;
        if( distance > maxDistance)
        {
            finalChaseSpeed *= 2f;
            str *= 2f;
        }

        // Move and rotate towards target
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, str);
        transform.Translate(0, 0, finalChaseSpeed);
    }
}

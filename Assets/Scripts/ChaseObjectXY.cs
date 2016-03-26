using UnityEngine;
using System.Collections;

public class ChaseObjectXY : MonoBehaviour
{
    
    // What to chase
    public GameObject whatToChase;

    // Start looking at the object
    public bool startLookingObject = true;

    // Speed to chase
    public float chaseSpeed = 1f;

    public float xDistance = 0;
    public float yDistance = 0;
    public float zDistance = 0;


    // Use this for initialization
    void Start()
    {

        // Check that we have someone to follow
        if (whatToChase == null)
        {
            Debug.LogWarning("No object assigned to chase!");
            return;
        }

        // Look at object on game start
        if (startLookingObject)
        {
            Vector2 pPos = whatToChase.transform.position; // WhatToChase's Position
            Vector2 tPos = this.transform.position; // This Objects Position

            this.transform.Translate(pPos - tPos);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Make sure we have something to chase
        if (whatToChase == null)
        {
            Debug.LogWarning("No object assigned to chase!");
            return;
        }

        Vector3 dPos = new Vector3(xDistance, yDistance, zDistance);
        Vector3 pPos = whatToChase.transform.position; // WhatToChase's Position
        Vector3 tPos = this.transform.position; // This Objects Position

        // Chase Object
        this.transform.Translate((pPos -dPos - tPos) * Time.deltaTime * chaseSpeed);

    }
}

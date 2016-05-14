using UnityEngine;
using System.Collections;
using System;

public class MovingPlatform : MonoBehaviour {

   
    public GameObject pointA;
    public GameObject pointB;
    public float speed;
    public Boolean flippingPlatform;
    private Boolean pointATarget = true;
    private int timer = 0;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        
        Patrol();

    }
    void FixedUpdate() {

        //Debug.Log(timer);
        Debug.Log(transform.rotation.eulerAngles);
        if (flippingPlatform) {
            if (timer == 100)
            {
                //transform.Rotate(1, 0, 0);
                
                this.transform.RotateAround(this.transform.position, transform.right, Time.deltaTime * 90f);
                if (transform.rotation.eulerAngles.x ==180f || transform.rotation.eulerAngles.x == 0f || transform.rotation.eulerAngles.x == 360f)
                {


                   timer = 0;
                }





            }
            else {
                //timer++;
            }
        }

    }

    private void Patrol()
    {
        
        if (pointATarget == true)
        {
           
          

            if (Vector3.Distance(transform.position, pointA.transform.position) < 0.5f)
            {
                Debug.Log("test");
                transform.position = Vector3.MoveTowards(transform.position, pointB.transform.position, speed * Time.deltaTime);
                //transform.MovePosition(transform.position, pointB.transform.position, speed * Time.deltaTime);
                pointATarget = !pointATarget;
            }
            else {
                transform.position = Vector3.MoveTowards(transform.position, pointA.transform.position, speed * Time.deltaTime);
            }
            
            
        }
        else {
            if (Vector3.Distance(transform.position, pointB.transform.position) < 0.5f)
            {
                transform.position = Vector3.MoveTowards(transform.position, pointA.transform.position, speed * Time.deltaTime);
                pointATarget = !pointATarget;
            }
            else {
                transform.position = Vector3.MoveTowards(transform.position, pointB.transform.position, speed * Time.deltaTime);
            }



        }
    }

    public void OnCollisionEnter (Collision col) {

         col.transform.parent = transform;
    }

    public void OnCollisionExit(Collision col)
    {
       col.transform.parent = null;
    }
}

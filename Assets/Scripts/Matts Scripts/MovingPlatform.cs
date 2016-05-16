using UnityEngine;
using System.Collections;
using System;

public class MovingPlatform : MonoBehaviour {

    
    public GameObject pointA;
    public GameObject pointB;
    public float speed;
    public Boolean flippingPlatform;
    public float flipTimer = 1;

    private Boolean pointATarget = true;
    private float timer = 0;
    private Collision collidedObject;
    private Boolean flipping = false;
    


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
       
        Patrol();

        //Debug.Log(timer);
        //Debug.Log(test);
        if (flippingPlatform)
        {
            if (timer >= 100*flipTimer)
            {
                flipping = true;
                if (collidedObject != null) {
                    collidedObject.transform.parent = null;
                }
                transform.Rotate(Vector3.right * 60 * Time.deltaTime);

                float test = (float)Math.Round(transform.rotation.eulerAngles.x);
/*<<<<<<< HEAD
                //Debug.Log(test);
=======
        
>>>>>>> DynamicBridge */
                if (test > 178f && test < 182f || test == 0f || test == 360f)
                {


                    timer = 0;
                    flipping = false;
                }





            }
            else {
                timer++;
            }
        }


    }
  

    private void Patrol()
    {
        
        if (pointATarget == true)
        {
           
          

            if (Vector3.Distance(transform.position, pointA.transform.position) < 0.5f)
            {
/*<<<<<<< HEAD
                //Debug.Log("test");
=======
               
>>>>>>> DynamicBridge */
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
        if (flipping == false && collidedObject == null)
        {
            collidedObject = col;
            col.transform.parent = transform;
        }
        
    }

    public void OnCollisionExit(Collision col)
    {
       col.transform.parent = null;
        collidedObject = null;
    }
}

using UnityEngine;
using System.Collections;
using System;

public class MovingPlatform : MonoBehaviour {

    
    public GameObject pointA;
    public GameObject pointB;
    public float movementSpeed;
   // public Boolean flippingPlatform;
    public Boolean isTrigger;
   // public float flipTimer = 1;

    private Boolean pointATarget = true;
    private float timer = 0;
    private Collision collidedObject;
    private Boolean flipping = false;
    private float speed;
    


    // Use this for initialization
    void Start () {
        if (isTrigger)
        {
            speed = 0;
        }
        else {
            speed = movementSpeed;
        }
}
    
    // Update is called once per frame
    void Update () {
       
        Patrol();

        //Debug.Log(timer);
        //Debug.Log(test);
        //if (flippingPlatform)
        //{
        //    if (timer >= 100*flipTimer)
        //    {
        //        flipping = true;
        //        if (collidedObject != null) {
        //            collidedObject.transform.parent = null;
        //        }
        //        transform.Rotate(Vector3.right * 60 * Time.deltaTime);

        //        float test = (float)Math.Round(transform.rotation.eulerAngles.x);
        //        if (test > 178f && test < 182f || test == 0f || test == 360f)
        //        {


        //            timer = 0;
        //            flipping = false;
        //        }





        //    }
        //    else {
        //        timer++;
        //    }
        //}


    }
  

    private void Patrol()
    {
        
        if (pointATarget == true)
        {
           
          

            if (Vector3.Distance(transform.position, pointA.transform.position) < 0.5f)
            {
				transform.position = Vector3.MoveTowards(transform.position, pointB.transform.position, speed * (Time.deltaTime * Time.timeScale));
                //transform.MovePosition(transform.position, pointB.transform.position, speed * Time.deltaTime);
                pointATarget = !pointATarget;
                if (isTrigger)
                {
                    speed = 0;
                }
            }
            else {
				transform.position = Vector3.MoveTowards(transform.position, pointA.transform.position, speed * (Time.deltaTime * Time.timeScale));
                
            }
            
            
        }
        else {
            if (Vector3.Distance(transform.position, pointB.transform.position) < 0.5f)
            {
				transform.position = Vector3.MoveTowards(transform.position, pointA.transform.position, speed * (Time.deltaTime * Time.timeScale));
                pointATarget = !pointATarget;
                if (isTrigger)
                {
                    speed = 0;
                }
            }
            else {
				transform.position = Vector3.MoveTowards(transform.position, pointB.transform.position, speed * (Time.deltaTime * Time.timeScale));
                
            }




        }
    }


    public void triggerPlatform(Boolean trig)
    {
        if (trig == true)
        {
            pointATarget = true;

            if (Vector3.Distance(transform.position, pointA.transform.position) > 0.5f)
            {
                speed = movementSpeed;
            }
            else {
                speed = 0;
            }
        }
        else {
            pointATarget = false;
            if (Vector3.Distance(transform.position, pointB.transform.position) > 0.5f)
            {
                speed = movementSpeed;
            }
            else {
                speed = 0;
            }

        }
        
    }

    public bool checkFlipping() {
        return flipping;
    }
}
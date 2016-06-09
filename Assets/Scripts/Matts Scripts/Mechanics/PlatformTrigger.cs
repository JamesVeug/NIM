using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlatformTrigger : MonoBehaviour {

   // private Vector3 oldEulerAngles;

    private int index;
	// Use this for initialization
	void Start () {
       // oldEulerAngles = this.transform.rotation.eulerAngles;
        index = 0;
    }
	
	// Update is called once per frame
	void FixedUpdate () {

    }

 

    public void OnTriggerEnter(Collider other)
    {
        other.transform.parent = transform;

    }

    public void OnTriggerExit(Collider other)
    {

        other.transform.parent = null;


    }

    //void OnTriggerStay(Collider other)
    //{
    //    if (oldEulerAngles == this.transform.rotation.eulerAngles)
    //    {
    //        other.transform.parent = transform;
    //    }
    //    else {
           
    //        other.transform.parent = null;
    //        oldEulerAngles = this.transform.rotation.eulerAngles;
    //    }

    //}




}

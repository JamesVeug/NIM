using UnityEngine;
using System.Collections;
using System;

public class Door : MonoBehaviour {

    public Boolean open;
    public GameObject top;
    public GameObject bottom;
    public float speed;

    private Vector3 topClosed;
    private Vector3 botClosed;
    private Vector3 topOpen;
    private Vector3 botOpen;


    // Use this for initialization
    void Start () {
        topClosed = top.transform.localPosition;
        botClosed = bottom.transform.localPosition;
        topOpen = new Vector3(topClosed.x,topClosed.y + top.transform.localScale.y, topClosed.z);
        botOpen= new Vector3(botClosed.x, botClosed.y - bottom.transform.localScale.y, topClosed.z);
       
    }
	
	// Update is called once per frame
	void Update () {
        if (open)
        {
            slowMove(topOpen, top);
            slowMove(botOpen, bottom);
        }
        else {
            slowMove(topClosed, top);
            slowMove(botClosed, bottom);
        }
        
	}

   

    private void slowMove(Vector3 desiredPos, GameObject doorBlock)
    {
        if (Math.Abs(desiredPos.y - doorBlock.transform.localPosition.y) > 0.05f)
        {
            if (desiredPos.y > doorBlock.transform.localPosition.y)
            {
                float slowYInc = doorBlock.transform.localPosition.y + (doorBlock.transform.localScale.y * 0.05f * speed);
                doorBlock.transform.localPosition = new Vector3(doorBlock.transform.localPosition.x, slowYInc, doorBlock.transform.localPosition.z);
            }
            else if (desiredPos.y < doorBlock.transform.localPosition.y)
            {
                float slowYInc = doorBlock.transform.localPosition.y - (doorBlock.transform.localScale.y * 0.05f * speed);
                doorBlock.transform.localPosition = new Vector3(doorBlock.transform.localPosition.x, slowYInc, doorBlock.transform.localPosition.z);
            }
        }
        else {
            doorBlock.transform.localPosition = desiredPos;

        }
    }
    public void triggerDoor() {
        
        open = !open;
    }

   
}

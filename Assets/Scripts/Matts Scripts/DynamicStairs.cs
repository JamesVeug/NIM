using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DynamicStairs : MonoBehaviour {

    public float currentPosition;
    public float speed;
    public Boolean consistentMove;
    public float postion1;
    public float position2;
    public float timer;

    private float counter = 0;


    private List<GameObject> steps;//A static amount of 6 steps are expected

    private float leftX;
    private float rightX;
    private float topY;
    private float botY;

    private float scaleX;
    private float scaleY;
    private float z;
    
	// Use this for initialization
	void Start () {
        steps = new List<GameObject>();
        //Gives us our 6 steps
        foreach (Transform child in this.transform) {
            if (child.tag == "Step") {
                steps.Add(child.gameObject);
            }
        }

        CalcCoord();
	}

    /**
        Sees the stairs as a box, calculates where the corners are
        based off the steps    
    */
    private void CalcCoord()
    {
        leftX = float.MaxValue;
        rightX = float.MinValue;
        topY = float.MinValue;
        botY = float.MaxValue;

        foreach (GameObject step in steps) {
            float x = step.transform.localPosition.x;
            float y = step.transform.localPosition.y;

            if (x < leftX) {
                leftX = x;
            }
           
            if (y < botY) {
                botY = y;
            }

        }
        scaleX = steps[0].transform.localScale.x;
        scaleY = steps[0].transform.localScale.y;
        z = steps[0].transform.localPosition.z;

       
            rightX = leftX + (scaleX * (steps.Count-1));
            topY = botY + (scaleY * (steps.Count-1));
    }

    public void SetPosition(float givenPosition)
    {
        if (givenPosition < 1 || givenPosition > 4) {
            throw new System.ArgumentException("Only accepts a position between 1 and 4");
        }

        
        if (givenPosition == 1) {
            SetPositionHelper(leftX,botY,scaleY, true);
        }
        else if (givenPosition == 2)
        {
            SetPositionHelper(leftX, topY, (scaleY * -1), true);
        }
        else if (givenPosition == 3)
        {
            SetPositionHelper(leftX, botY, scaleY, false);
        }
        else
        {
            SetPositionHelper(leftX, topY, scaleY, false);
        }
    }


   


    public void SetPositionHelper(float xPos, float yPos, float scaleY, Boolean diag)
    {
        foreach (GameObject step in steps)
        {
            slowMove(xPos, yPos, step);
            xPos = xPos + scaleX;
            if (diag == true)
            {
                yPos = yPos + scaleY;
            }

        }

    }

    /**
        Used to slowly move the step in the desired direction(up or down)
    */
    private void slowMove(float xPos, float yPos, GameObject step)
    {
        if (Math.Abs(yPos - step.transform.localPosition.y) > 0.05f)
        {
            if (yPos > step.transform.localPosition.y)
            {
                float slowYInc = step.transform.localPosition.y + (step.transform.localScale.y * 0.05f * speed);
                step.transform.localPosition = new Vector3(xPos, slowYInc, z);
            }
            else if (yPos < step.transform.localPosition.y)
            {
                float slowYInc = step.transform.localPosition.y - (step.transform.localScale.y * 0.05f * speed);
                step.transform.localPosition = new Vector3(xPos, slowYInc, z);
            }
        }
        else {
            step.transform.localPosition = new Vector3(xPos, yPos, z);

        }
    }
    


// Update is called once per frame
void Update () {
        if (consistentMove == true) {
            
            if (counter >= 100 * timer) {
                
                SetPosition(currentPosition);
                counter = 0;
                if (currentPosition != postion1)
                {
                    currentPosition = postion1;

                } else {
                    currentPosition = position2;

                }
            } else {
                counter++;
            }
        } 
            SetPosition(currentPosition);
        
        
	}
}


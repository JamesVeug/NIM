using UnityEngine;
using System.Collections;
using System;

public class PressurePlate : MonoBehaviour
{

    private Vector3 PositionOff;
    private Vector3 PositionOn;

    public GameObject partner;
    public partType partnerType;
    public bool triggered;


    // Use this for initialization
    void Start()
    {
        PositionOff = this.transform.localPosition;
        //moved half the trigger plate down
        PositionOn = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y - (this.transform.position.y / 2), this.transform.localPosition.z);
        CastePartner();

    }

    private void CastePartner()
    {
       
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (triggered)
        {
            slowMove(PositionOn);

        }
        else {
            slowMove(PositionOff);
        }
    }

    private void slowMove(Vector3 desiredPos)
    {
        if (Math.Abs(desiredPos.y - this.transform.localPosition.y) > 0.05f)
        {
            if (desiredPos.y > this.transform.localPosition.y)
            {
                float slowYInc = this.transform.localPosition.y + (this.transform.localScale.y * 0.05f * 1);
                this.transform.localPosition = new Vector3(this.transform.localPosition.x, slowYInc, this.transform.localPosition.z);

            }
            else if (desiredPos.y < this.transform.localPosition.y)
            {
                float slowYInc = this.transform.localPosition.y - (this.transform.localScale.y * 0.05f * 1);
                this.transform.localPosition = new Vector3(this.transform.localPosition.x, slowYInc, this.transform.localPosition.z);
            }
        }
        else {
            this.transform.localPosition = desiredPos;

        }
    }


    public void setTrigger(bool trig)
    {
        triggered = trig;
        triggerReact();
    }

    private void triggerReact()
    {

        if (partnerType == partType.Door)
        {
            Door partnerScript = partner.GetComponent<Door>();
            partnerScript.triggerDoor();
        }
        else if (partnerType == partType.Stairs)
        {
            DynamicStairs partnerScript = partner.GetComponent<DynamicStairs>();
            partnerScript.triggerStairs();
        }
        else {
            if (triggered)
            {
                MovingPlatform partnerScript = partner.GetComponent<MovingPlatform>();
                partnerScript.triggerPlatform(true);
            }
            else {
                MovingPlatform partnerScript = partner.GetComponent<MovingPlatform>();
                partnerScript.triggerPlatform(false);
            }
            
        }
   
    }

    public enum partType{
        Door,Stairs,Platform
    }



}

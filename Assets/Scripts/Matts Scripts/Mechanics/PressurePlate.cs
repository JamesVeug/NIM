using UnityEngine;
using System.Collections;
using System;

public class PressurePlate : MonoBehaviour
{

    private Vector3 PositionOff;
    private Vector3 PositionOn;

    public GameObject[] partners;
    private bool triggered;

    // Use this for initialization
    void Start()
    {
        PositionOff = this.transform.localPosition;
        //moved half the trigger plate down
        PositionOn = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y - (this.transform.position.y / 2), this.transform.localPosition.z);

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
        for (int i = 0; i < partners.Length; i++)
        {
            if (partners[i].GetComponent<Door>() != null)
            {
                Door partnerScript = partners[i].GetComponent<Door>();
                partnerScript.triggerDoor();
            }
            else if (partners[i].GetComponent<DynamicStairs>() != null)
            {
                DynamicStairs partnerScript = partners[i].GetComponent<DynamicStairs>();
                partnerScript.triggerStairs();
            }
            else if (partners[i].GetComponent<MovingPlatform>() != null)
            {
                if (triggered)
                {
                    MovingPlatform partnerScript = partners[i].GetComponent<MovingPlatform>();
                    partnerScript.triggerPlatform(true);
                }
                else {
                    MovingPlatform partnerScript = partners[i].GetComponent<MovingPlatform>();
                    partnerScript.triggerPlatform(false);
                }

            }
            else {
                Debug.Log("Must give a Door, platform or stairs script");
            }
        }

    }

    public enum partType
    {
        Door, Stairs, Platform
    }



}

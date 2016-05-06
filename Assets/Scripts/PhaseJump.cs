﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PhaseJump : MonoBehaviour {

    private Movement playerMovement;
    private bool canPhase = true;
    private bool phaseMenuOpen = false;
    private int phaseDirectionSelected = 0;

    private List<PhaseCondition> conditions = new List<PhaseCondition>();

    public bool copyYOnPhase = false;
    public bool copyJumpedHeightOnPhase = true;
    public bool moveCameraOnPhase = false;

    // Use this for initialization
    void Start () {
        playerMovement = GetComponent<Movement>();
	}
	
    public bool phaseMenuIsOpen()
    {
        return phaseMenuOpen;
    }

    public int getJumpDirection()
    {
        return phaseDirectionSelected;
    }

    void phaseForward()
    {
        MovementWaypoint currentPoint = playerMovement.currentMovementWaypoint;
        if (currentPoint == null)
        {
            Debug.LogError("Player does not have a current Waypoint to phase");
            return;
        }

        // Move to new point
        bool phased = phaseToWayPoint(currentPoint, currentPoint.nextPhasePoint, true);

        // move camera
        if (phased && moveCameraOnPhase)
        {
            ChasePlayer chase = Camera.main.GetComponent<ChasePlayer>();
            if (chase != null) chase.instantlyMoveToPlayer();
        }
    }

    void phaseBack()
    {
        MovementWaypoint currentPoint = playerMovement.currentMovementWaypoint;
        if (currentPoint == null)
        {
            Debug.LogError("Player does not have a current Waypoint to phase");
            return;
        }

        // Move to new point
        bool phased = phaseToWayPoint(currentPoint, currentPoint.previousPhasePoint, false);

        // move camera
        if (phased && moveCameraOnPhase)
        {
            ChasePlayer chase = Camera.main.GetComponent<ChasePlayer>();
            if (chase != null) chase.instantlyMoveToPlayer();
        }
    }

    // Update is called once per frame
    void Update () {

        float phaseJumpMenu = Input.GetAxis("PhaseJumpMenu");
        if( phaseJumpMenu == 0)
        {
            phaseMenuOpen = false;
            if( phaseDirectionSelected == 1)
            {
                phaseForward();
            }
            else if (phaseDirectionSelected == -1)
            {
                phaseBack();
            }

            // Closed the menu and reset
            phaseDirectionSelected = 0;
            return;
        }

        phaseMenuOpen = true;
        float phaseJumpDirection = Input.GetAxis("PhaseJump");
        if (phaseJumpDirection != 0 && canPhase )
        {
            canPhase = false;

            // Phase forward
            if( phaseJumpDirection > 0 )
            {
                phaseDirectionSelected = 1;
            }

            // Phase Backward
            if (phaseJumpDirection < 0 )
            {
                phaseDirectionSelected = -1;
            }
        }
        else if( phaseJumpDirection == 0 && !canPhase)
        {
            // We released the phase button.
            // Allow us to phase again
            phaseDirectionSelected = 0;
            canPhase = true;
        }

    }

    /*
    * A -> current Point
    * B -> next Point
    *
    * #1 A and B do not have a next point
    * #2 A has no next point, But B does
    * #3 A has a next point, But B does NOT
    * #4 A and B have next points and are linked via a phase
    *
    * #5 A's next is not linked to B's next via a phase
    *
    */
    private bool phaseToWayPoint(MovementWaypoint current, MovementWaypoint phasedPoint, bool phaseForward)
    {
        // Make sure we can phase
        if(!canPhaseJump(current,phaseForward))
        {
            return false;
        }

        // Get the points to phase TO
        Vector3 spawnPosition = Vector3.zero;
        MovementWaypoint newPhasePoint = null;
        getPhasePoint(phaseForward, out spawnPosition, out newPhasePoint);

        //Debug.LogWarning("Phasing!");

        // If we are inside any phaseCondition volumes. Call the beforePhase method
        callConditions(true, phaseForward);

        // Move to the phase position
        transform.position = spawnPosition;
        playerMovement.currentMovementWaypoint = newPhasePoint;

        // If we are inside any phaseCondition volumes. Call the afterPhase method
        callConditions(false, phaseForward);
        return true;
    }

    private bool canPhaseJump(MovementWaypoint current, bool phaseForward)
    {
        MovementWaypoint phasedPoint = phaseForward ? current.nextPhasePoint : current.previousPhasePoint;
        if (phasedPoint == null)
        {
            //Debug.LogError("FAILED: 0");
            return false;
        }

        // We can't move forward
        MovementWaypoint nextPoint = current.next;
        if (nextPoint == null)
        {
            return false;
        }

        MovementWaypoint nextPhasePoint = phaseForward ? nextPoint.nextPhasePoint : nextPoint.previousPhasePoint;

        // #1 A and B do not have a next point
        if (nextPhasePoint == null)
        {
            //Debug.LogError("FAILED: 1");
            return false;
        }

        // Check An and Bn ARE connected
        bool areConnected = false;
        MovementWaypoint B = phaseForward ? current.nextPhasePoint : current.previousPhasePoint;
        for (MovementWaypoint C = B.next; C != null; C = C.next)
        {
            if (C == nextPhasePoint) {
                areConnected = true;
                break;
            }
        }
        if( !areConnected )
        {
            // If B and Bn are not connected. Stop
            //Debug.Log("Not Conencted B-BN " + B.name + ", " + nextPhasePoint.name);
            return false;
        }

        // #4 A and B have next points and are linked via a phase
        if (!areLinkedByPhase(nextPoint, nextPhasePoint))
        {
            //Debug.LogError("FAILED: 4");
            return false;
        }

        // We can phase
        return true;
    }

    public void getPhasePoint(bool phaseForward, out Vector3 newPoint, out MovementWaypoint newPhasedPoint)
    {

        MovementWaypoint current = playerMovement.currentMovementWaypoint;
        MovementWaypoint phasedPoint = phaseForward ? current.nextPhasePoint : current.previousPhasePoint;

        Vector3 spawnPosition = Vector3.zero;
        MovementWaypoint spawnPhase = null;

        // Find midpoint between both points
        spawnPosition = getMidPoint(current, phasedPoint, phaseForward);
        spawnPhase = getPreviousWayPoint(current, spawnPosition, phaseForward);

        // Apply the Y according to the largest Y of the points then add the players extra Y
        float extraY = transform.position.y - current.transform.position.y;
        
        // Use the Y of the highest point? (current or next)
        if (copyYOnPhase)
        {
            Vector3 opposite = getMidPoint(phasedPoint, current, phaseForward);
            float maxY = Mathf.Max(opposite.y, spawnPosition.y);
            spawnPosition.y = maxY;
        }

        // If we have jumped before phasing, add that height to the next phase area
        if( copyJumpedHeightOnPhase)
        {
            spawnPosition.y += extraY;
        }

        // Return the points
        newPoint = spawnPosition;
        newPhasedPoint = spawnPhase;
    }

    private MovementWaypoint getPreviousWayPoint(MovementWaypoint A, Vector3 spawnposition, bool phaseForward)
    {

        MovementWaypoint An = A.next;

        float Ad = (A.next.transform.position - A.transform.position).magnitude;
        float At = (transform.position - A.transform.position).magnitude;
        float Ap = At / Ad;
        //Debug.Log("percent " + Ap);


        MovementWaypoint B = phaseForward ? A.nextPhasePoint : A.previousPhasePoint;
        MovementWaypoint Bn = phaseForward ? An.nextPhasePoint : An.previousPhasePoint;

        // Get the distance from B to Bn by traversing the nodes
        float Bd = 0f;
        for( MovementWaypoint b = B; b != Bn; b = b.next)
        {
            Bd += (b.transform.position-b.next.transform.position).magnitude;
        }

        // Find out where we should be in the graph by using Ap
        float Bp = 0f;
        MovementWaypoint previousWayPoint = B;
        while (Bp <= Ap)
        {
            float distance = (previousWayPoint.transform.position - previousWayPoint.next.transform.position).magnitude;
            float percent = distance / Bd;
            Bp += percent;

            if( previousWayPoint.next != Bn )
            {
                previousWayPoint = previousWayPoint.next;
            }
        }

        //Debug.Log("Previous " + previousWayPoint.name);
        return previousWayPoint;
    }

    private Vector3 getMidPoint(MovementWaypoint A, MovementWaypoint B, bool phaseForward)
    {
        MovementWaypoint Bn = phaseForward ? A.next.nextPhasePoint : A.next.previousPhasePoint;

        // Get the distance from A to An
        float Ad = (A.next.transform.position - A.transform.position).magnitude;
        //Debug.Log("Ad " + Ad);

        // Get the distance traveled
        float traveled = (transform.position - A.transform.position).magnitude;
        //Debug.Log("traveled " + traveled);

        // Get percentage
        float traveledPercent = traveled / Ad;
        //Debug.Log("traveledPercent " + traveledPercent);

        // Get point between B and Bn according to %
        Vector3 destination = getPointOnLine(B.transform.position, Bn.transform.position, traveledPercent);

        // Return Y
        return destination;
    }

    private Vector3 getPointOnLine(Vector3 A, Vector3 B, float scale)
    {
        //Subtract the two vector (B - A) to get a vector pointing from A to B.Lets call this AB
        Vector3 AB = B - A;

        //2) Normalize this vector AB. Now it will be one unit in length.
        AB = AB * scale;

        //3) You can now scale this vector to find a point between A and B.so(A + (0.1 * AB)) will be 0.1 units from A.
        return A+AB;
    }

    private void callConditions(bool beforePhase, bool phaseForward)
    {
        foreach (PhaseCondition p in conditions)
        {
            if (beforePhase)
            {
                p.triggerBeforePhase(phaseForward);
            }
            else
            {
                p.triggerAfterPhase(phaseForward);
            }
        }
    }

    public void addPhaseCondition(PhaseCondition c)
    {
        conditions.Add(c);
    }

    public void removePhaseCondition(PhaseCondition c)
    {
        conditions.Remove(c);
    }

    // Get A's next (An)
    // Get B's furthest point according to An's phase (Bn).
    // Get distance from B to Bn. (Bd)
    // Get distance from A to An. (Ad)

    // Get the % of how far we are from A to An
    // Get Waypoint in B that contains %
    private void getDistance(MovementWaypoint current, MovementWaypoint next, bool phaseForward, out MovementWaypoint newWaypoint, out Vector3 newPosition)
    {
        MovementWaypoint A = current;
        MovementWaypoint B = next;

        
        MovementWaypoint An = null;
        MovementWaypoint Bn = null;
        MovementWaypoint graphA = getA(current, next, phaseForward);
        if( graphA == next)
        {
            Bn = graphA.next;
            An = !phaseForward ? Bn.nextPhasePoint : Bn.previousPhasePoint;
        }
        else
        {
            An = graphA.next;
            Bn = phaseForward? An.nextPhasePoint: An.previousPhasePoint;
        }

        Debug.LogWarning("A " + A);
        Debug.LogWarning("B " + B);
        Debug.LogWarning("An " + An);
        Debug.LogWarning("Bn " + Bn);

        // Get the maximum distance we need to travel in B
        float Bd = 0f;
        for(MovementWaypoint i = Bn; i != B; i = i.previous)
        {
            Bd += (i.transform.position - i.previous.transform.position).magnitude;
        }
        Debug.LogWarning("Bd " + Bd);

        // %Distance we traveled in A
        float percent = (A.transform.position - transform.position).magnitude / (An.transform.position - A.transform.position).magnitude;
        Debug.LogWarning("percent  " + percent);

        // Find point in B that we should appear in
        float distance = 0f;
        newWaypoint = B;
        while (newWaypoint != Bn.previous)
        {
            float mag = (newWaypoint.next.transform.position - newWaypoint.transform.position).magnitude;
            float tempDistance = (distance+mag) / Bd;
            float newTravel = distance + (newWaypoint.next.transform.position - newWaypoint.transform.position).magnitude;
            Debug.LogWarning("newTravel " + newTravel);
            Debug.LogWarning("newWaypoint " + newWaypoint);

            // Have we traveled too far?
            if (tempDistance > percent)
            {
                Debug.LogWarning("BREAK:");
                break;
            }

            distance += mag;
            newWaypoint = newWaypoint.next;
        }

        // How far accross should we travel to the next point?
        float remainingDistance = Bd - distance - (A.transform.position - transform.position).magnitude;
        float remainingDistancePercentage = remainingDistance / Bd;
        Debug.LogWarning("remainingDistance" + remainingDistance);
        Debug.LogWarning("remainingDistancePercentage" + remainingDistancePercentage);

        newPosition = getPointOnLine(newWaypoint.transform.position,newWaypoint.next.transform.position, remainingDistancePercentage);
    }

    private MovementWaypoint getA(MovementWaypoint current, MovementWaypoint next, bool phaseForward)
    {
        // B X----X----X
        //   |         |
        //   |         |
        // A X---------X

        MovementWaypoint cnP = phaseForward ? current.next.nextPhasePoint : current.next.previousPhasePoint;
        MovementWaypoint nnP = phaseForward ? next.next.nextPhasePoint : next.next.previousPhasePoint;

        if (cnP != null && cnP.phaseLayer == next.phaseLayer)
        {
            //Debug.LogWarning("~A " + current.name);
            return current;
        }

        //Debug.LogWarning("~A " + next.name);
        return next;
    }

    public void previewPhaseCamera(Camera cam, GameObject o, Vector3 pos, bool phaseForward)
    {

        Camera main = Camera.main;
        if (conditions.Count > 0)
        {
            ChasePlayer mainChase = main.GetComponent<ChasePlayer>();
            ChasePlayer camChase = cam.GetComponent<ChasePlayer>();
            camChase.transform.position = mainChase.transform.position;
            camChase.transform.localEulerAngles = mainChase.transform.localEulerAngles;
        }

        foreach (PhaseCondition p in conditions)
        {
            p.cameraToEdit = cam;
            p.triggerBeforePhase(phaseForward);
        }

        o.transform.position = pos;

        foreach (PhaseCondition p in conditions)
        {
            p.triggerAfterPhase(phaseForward);
            p.cameraToEdit = main;
        }
    }

    private bool areLinkedByPhase(MovementWaypoint A, MovementWaypoint B)
    {
        return (A.nextPhasePoint == B && B.previousPhasePoint == A) || (A.previousPhasePoint == B && B.nextPhasePoint == A);
    }

    public bool canPhaseForward()
    {
        return canPhaseJump(playerMovement.currentMovementWaypoint, true);
    }

    public bool canPhaseBack()
    {
        return canPhaseJump(playerMovement.currentMovementWaypoint, false);
    }
}
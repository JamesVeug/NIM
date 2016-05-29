using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XInputDotNetPure;

public class PhaseJump : MonoBehaviour
{

    private Movement playerMovement;
    private AudioSource audioSource;
    private bool canPhase = true;
    private int phaseDirectionSelected = 0;
    public AnimationCurve scaleCurve;

    private bool phasing = false;
    private Vector3 savedScale = Vector3.zero;
    private float coolDownRemainingTime = 0f;
    private float phaseRemainingTime = 0f;
    private Vector3 phaseFromPosition = Vector3.zero;
    private Vector3 phaseToPosition = Vector3.zero;
    //private MovementWaypoint waypoint = null;

    private List<PhaseCondition> conditions = new List<PhaseCondition>();

    public float phaseCoolDown = 0.5f; // half second
    public float phaseTime = 0.5f; // half a second
    public float vibrationScale = 0.5f;
    public bool copyYOnPhase = false;
    public bool copyJumpedHeightOnPhase = true;
    public bool moveCameraOnPhase = false;

    // The PhaseVolume that we are current in
    private PhaseVolume currentPhaseVolume;

    // SOUNDS
    // Volumes (100 represents 100% volume intensity)
    [Range(min: 0, max: 100)]
    public float[] phaseForwardSoundsVolume;

    [Range(min: 0, max: 100)]
    public float[] phaseBackSoundsVolume;

    // Clips
    public AudioClip[] phaseForwardSounds;
    public AudioClip[] phaseBackSounds;

    // Use this for initialization
    void Start()
    {
        playerMovement = GetComponent<Movement>();
        audioSource = GetComponent<AudioSource>();
        coolDownRemainingTime = phaseCoolDown;
    }

    public int getJumpDirection()
    {
        return phaseDirectionSelected;
    }

    bool phaseForward()
    {
        return phase(true);
    }

    bool phaseBack()
    {
        return phase(false);
    }

    public void ShakeCamera()
    {
        Camera cam = Camera.main;
        ShakeCamera shake = cam.GetComponent<ShakeCamera>();
        if( shake != null)
        {
            shake.DoShake();
        }
    }

    private bool phase(bool phaseForward)
    {
        //Debug.Log("Running");
        MovementWaypoint currentPoint = playerMovement.currentMovementWaypoint;
        if (currentPoint == null)
        {
            Debug.LogError("Player does not have a current Waypoint to phase");
            return false;
        }

        // Get the next phase waypoint to end up on
        MovementWaypoint nextPhasePoint = phaseForward ? currentPoint.nextPhasePoint : currentPoint.previousPhasePoint;
        
        // Move to new point
        bool phased = phaseToWayPoint(currentPoint, nextPhasePoint, phaseForward);

        // Return if we phased or not
        return phased;
    }

    // Update is called once per frame
    void Update()
    {
        coolDownRemainingTime += Time.deltaTime;


        if (phasing)
        {

            float time = phaseRemainingTime / phaseTime;
            transform.position = Vector3.Slerp(phaseFromPosition, phaseToPosition, time);
            phaseRemainingTime += Time.deltaTime;

            // Scale character
            GameObject c = gameObject.transform.FindChild("Model").gameObject;
            float curveScale = scaleCurve.Evaluate(time);
            Vector3 scale = savedScale * curveScale;
            c.transform.localScale = scale;

            // Vibration
            float vibration = (1 - curveScale)*vibrationScale;
            GamePad.SetVibration(PlayerIndex.One, vibration, vibration);

            // TODO: Needs to be Fixed. Sometimes plays more than once!
            if (curveScale > 0.1 && curveScale < 0.3  && time < 1)
            {
                SoundMaster.playRandomSound(phaseBackSounds, phaseBackSoundsVolume, getAudioSource());
            }
            // Finished phasing
            else if (time >= 1)
            {
                c.transform.localScale = savedScale;
                transform.position = phaseToPosition;
                phasing = false;
                canPhase = true;
                phaseDirectionSelected = 0;
                ShakeCamera();

                // Reset cooldown
                coolDownRemainingTime = 0f;

                // Stop vibration
                GamePad.SetVibration(PlayerIndex.One, 0, 0);
            }

            return;
        }
        
        // Determine which way we should phase
        if (phaseDirectionSelected == 1)
        {
            phaseForward();
            return;
        }
        else if (phaseDirectionSelected == -1)
        {
            phaseBack();
            return;
        }

        // phaseMenuOpen = true;
        float phaseJumpDirection = Input.GetAxis("PhaseJump");
        if (Mathf.Abs(phaseJumpDirection) == 1 && canPhase)
        {
            //Debug.Log("Rotation2 " + Camera.main.transform.localEulerAngles);
            //Debug.Log("Position2 " + Camera.main.transform.position);
            //pauseGame();

            // Phase forward
            if (phaseJumpDirection == 1 && canPhaseForward())
            {
                phaseDirectionSelected = 1;
                canPhase = false;
            }

            // Phase Backward
            if (phaseJumpDirection == -1 && canPhaseBack())
            {
                phaseDirectionSelected = -1;
                canPhase = false;
            }
        }
        else if (phaseJumpDirection == 0 && !canPhase)
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
        if (!canPhaseJump(current, phaseForward))
        {
            return false;
        }

        // Get the points to phase TO
        Vector3 spawnPosition = Vector3.zero;
        MovementWaypoint newPhasePoint = null;
        getPhasePoint(phaseForward, out spawnPosition, out newPhasePoint);

        // If we are inside any phaseCondition volumes. Call the beforePhase method
        callConditions(true, phaseForward);
        

        // Start phase

        GameObject c = gameObject.transform.FindChild("Model").gameObject;
        savedScale = c.transform.localScale;
        phaseToPosition = spawnPosition;
        phaseFromPosition = transform.position;
        playerMovement.currentMovementWaypoint = newPhasePoint;
        phaseRemainingTime = 0;
        phasing = true;

        Renderer rend = gameObject.GetComponent<Renderer>();
        rend.enabled = false;

        // If we are inside any phaseCondition volumes. Call the afterPhase method
        callConditions(false, phaseForward);
        return true;
    }

    void pauseGame()
    {
        if (Time.timeScale == 1f)
            Time.timeScale = 0f;
        else Time.timeScale = 1f;
    }

    // We are current in a PhaseVolume. Check that we can phase
    private bool canPhaseJumpInVolume(MovementWaypoint current, bool phaseForward)
    {
        if (phaseForward && currentPhaseVolume.nextPhaseWaypoint == null)
        {
            return false;
        }
        else if (!phaseForward && currentPhaseVolume.previousPhaseWaypoint == null)
        {
            return false;
        }

        return true;
    }

    private bool canPhaseJump(MovementWaypoint current, bool phaseForward)
    {
        // Check cooldown
        if(coolDownRemainingTime < phaseCoolDown)
        {
            // Still cooling down!
            return false;
        }

        // Are we inside a volume?
        if (currentPhaseVolume != null)
        {
            return canPhaseJumpInVolume(current, phaseForward);
        }

        MovementWaypoint phasedPoint = phaseForward ? current.nextPhasePoint : current.previousPhasePoint;
        if (phasedPoint == null)
        {
            //Debug.LogError("FAILED: 0");
            return false;
        }


        MovementWaypoint nextPoint = current.next;
        if (nextPoint == null && current.previous == null)
        {
            // We can not move from this node. But we can still phase
            return true;
        }

        // We can't move forward
        if (nextPoint == null)
        {
            return false;
        }

        MovementWaypoint nextPhasePoint = phaseForward ? nextPoint.nextPhasePoint : nextPoint.previousPhasePoint;
        if (nextPhasePoint != null && nextPhasePoint.next == null && nextPhasePoint.previous == null)
        {
            // We can move from this node, but not in the next plane. But we can still phase
            return true;
        }

        // #1 A and B do not have a next point
        if (nextPhasePoint == null)
        {
            return false;
        }

        // Check An and Bn ARE connected
        bool areConnected = false;
        MovementWaypoint B = phaseForward ? current.nextPhasePoint : current.previousPhasePoint;
        for (MovementWaypoint C = B.next; C != null; C = C.next)
        {
            if (C == nextPhasePoint)
            {
                areConnected = true;
                break;
            }
        }
        if (!areConnected)
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

    // We are in a PhaseVolume. Return the position we are in
    private void getPhasePointInVolume(bool phaseForward, out Vector3 newPoint, out MovementWaypoint newPhasedPoint)
    {
        newPhasedPoint = phaseForward ? currentPhaseVolume.nextPhaseWaypoint : currentPhaseVolume.previousPhaseWaypoint;
        newPoint = newPhasedPoint.transform.position;
    }

    // Get the Points that we will end up if we phase
    public void getPhasePoint(bool phaseForward, out Vector3 newPoint, out MovementWaypoint newPhasedPoint)
    {
        // If we are in a volume. Prioritize that
        if (currentPhaseVolume != null)
        {
            getPhasePointInVolume(phaseForward, out newPoint, out newPhasedPoint);
            return;
        }

        // Not in a volume
        MovementWaypoint current = playerMovement.currentMovementWaypoint;
        MovementWaypoint phasedPoint = phaseForward ? current.nextPhasePoint : current.previousPhasePoint;

        Vector3 spawnPosition = Vector3.zero;
        MovementWaypoint spawnPhase = null;

        if (current.next == null || phasedPoint == null)
        {
            // Don't have a next on A's plane. Just use the exact position
            spawnPosition = phasedPoint.transform.position;
            spawnPhase = phasedPoint;
        }
        else {
            // Find midpoint between both points
            spawnPosition = getMidPoint(current, phasedPoint, phaseForward);
            spawnPhase = getPreviousWayPoint(current, spawnPosition, phaseForward);
        }

        // Apply the Y according to the largest Y of the points then add the players extra Y
        float extraY = transform.position.y - current.transform.position.y;

        // Use the Y of the highest point? (current or next)
        if (copyYOnPhase)
        {
            //Vector3 opposite = getMidPoint(phasedPoint, current, phaseForward);
            float maxY = Mathf.Max(transform.position.y, spawnPosition.y);
            //Debug.Log("pos " + transform.position.y);
            //Debug.Log("pos " + spawnPosition.y);
            spawnPosition.y = maxY;
            //Debug.Log("pos " + spawnPosition.y);
        }

        // If we have jumped before phasing, add that height to the next phase area
        if (copyJumpedHeightOnPhase)
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
        for (MovementWaypoint b = B; b != Bn; b = b.next)
        {
            Bd += (b.transform.position - b.next.transform.position).magnitude;
        }

        // Find out where we should be in the graph by using Ap
        float Bp = 0f;
        MovementWaypoint previousWayPoint = B;
        while (Bp <= Ap && previousWayPoint.next != null)
        {
            float distance = (previousWayPoint.transform.position - previousWayPoint.next.transform.position).magnitude;
            float percent = distance / Bd;
            Bp += percent;

            if (previousWayPoint.next != Bn)
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
        //Debug.Log("B " + B.name);
        //Debug.Log("Bn " + Bn);
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
        return A + AB;
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
        if (graphA == next)
        {
            Bn = graphA.next;
            An = !phaseForward ? Bn.nextPhasePoint : Bn.previousPhasePoint;
        }
        else
        {
            An = graphA.next;
            Bn = phaseForward ? An.nextPhasePoint : An.previousPhasePoint;
        }

        Debug.LogWarning("A " + A);
        Debug.LogWarning("B " + B);
        Debug.LogWarning("An " + An);
        Debug.LogWarning("Bn " + Bn);

        // Get the maximum distance we need to travel in B
        float Bd = 0f;
        for (MovementWaypoint i = Bn; i != B; i = i.previous)
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
            float tempDistance = (distance + mag) / Bd;
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

        newPosition = getPointOnLine(newWaypoint.transform.position, newWaypoint.next.transform.position, remainingDistancePercentage);
    }

    private MovementWaypoint getA(MovementWaypoint current, MovementWaypoint next, bool phaseForward)
    {
        // B X----X----X
        //   |         |
        //   |         |
        // A X---------X

        MovementWaypoint cnP = phaseForward ? current.next.nextPhasePoint : current.next.previousPhasePoint;
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
			camChase.transform.rotation = mainChase.transform.rotation;
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

    public void setPhaseVolume(PhaseVolume v)
    {
        this.currentPhaseVolume = v;
    }

    public PhaseVolume getPhaseVolume()
    {
        return currentPhaseVolume;
    }

    public AudioSource getAudioSource()
    {
        if (audioSource == null)
        {
            Debug.LogError("Object " + gameObject.name + " does not have an AudioSource Component!");
        }
        return audioSource;
    }

    public bool isPhasing()
    {
        return phasing;
    }

    public bool isCoolingDown()
    {
        return coolDownRemainingTime < phaseCoolDown;
    }
}
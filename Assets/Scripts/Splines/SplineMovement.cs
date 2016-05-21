using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SplineController))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class SplineMovement : MonoBehaviour {

    private SplineController splineController;
    private SplineInterpolator interp;
    private CharacterController controller;
    private AudioSource audioSource;
    
    public Vector3 speed = new Vector3(10f, 10f, 10f);
    public float gravity = 0.03f;
    public float jump = 0.4f;

    public MovementWaypoint currentMovementWaypoint;
    public float changeWaypointDistance = 0.25f; // Distance we must get to in order to chaneg to the next waypoint

    int index = 0;
    float falling = 0f;

    // SOUNDS
    private bool fallSoundPlayed = false;
    private bool nextFootStepSoundLeft = true;
    private float nextFootStepSound = 0f;
    public float footStepSoundDelay = 1f;
	//private float maxVolume = 200f;
    // Volumes (100 represents 100% volume intensity)
	[Range(min: 0, max: 100)]
    public float[] leftFootStepSoundsVolume;

	[Range(min: 0, max: 100)]
    public float[] rightFootStepSoundsVolume;

	[Range(min: 0, max: 100)]
    public float[] fallSoundsVolume;

	[Range(min: 0, max: 100)]
    public float[] jumpSoundsVolume;

    // Clips
    public AudioClip[] leftFootStepSounds;
    public AudioClip[] rightFootStepSounds;
    public AudioClip[] fallSounds;
    public AudioClip[] jumpSounds;


    // Use this for initialization
    void Start () {
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        interp = GetComponent<SplineInterpolator>();
        splineController = GetComponent<SplineController>();

        transform.position = currentMovementWaypoint.transform.position;

        // Look at the next point
        if (currentMovementWaypoint != null && currentMovementWaypoint.next != null)
        {
            transform.LookAt(currentMovementWaypoint.next.transform);
        }
    }
	
	// Update is called once per frame
	void Update () {

        Vector3 movement = getSplinePoint(Input.GetAxis("Horizontal"), 0);
        if ( movement == Vector3.zero)
        {
            return;
        }

        if (controller.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                SoundMaster.playRandomSound(jumpSounds, jumpSoundsVolume, audioSource);
                falling = jump;
            }

            if (falling < 0 && !fallSoundPlayed)
            {
                SoundMaster.playRandomSound(fallSounds, fallSoundsVolume, audioSource);
                fallSoundPlayed = true;
            }
        }
        else
        {
            falling -= gravity;
            fallSoundPlayed = false;
        }

        // Save current Y
        float currentY = transform.position.y;

        // Move
        controller.Move(new Vector3(movement.x, falling, movement.z));

        // Did we hit something moving up?
        if(falling > 0 && transform.position.y == currentY)
        {
            falling = 0;
        }

        // Play footstep if we are walking, not falling.
        if( movement.z != 0 && transform.position.y == currentY )
        {
            playFootStep();
        }
    }

    Vector3 getSplinePoint(float moveLeft, float moveForward)
    {
        Vector3 movement = Vector3.zero;
        if (moveLeft > 0)
        {
            Debug.Log("current " + currentMovementWaypoint.gameObject.name);
            Transform parent = currentMovementWaypoint.transform.parent;
            if (parent == null)
            {
                Debug.Log("No parent");
            }
            else if (parent.gameObject.GetComponent<WaypointGroup>() == null)
            {
                Debug.Log("No WaypointGroup");
            }
            else
            {
                index = index + 1;
                if (index >= 100)
                {

                    if (currentMovementWaypoint.next != null && currentMovementWaypoint.next.next != null && currentMovementWaypoint.next.next.next != null)
                    {
                        index = 0;
                        Debug.Log("current " + currentMovementWaypoint.gameObject.name);
                        currentMovementWaypoint = currentMovementWaypoint.next;
                        Debug.Log("current " + currentMovementWaypoint.gameObject.name);
                    }
                    else
                    {
                        index = 99;
                        return Vector3.zero;
                    }
                }
                Debug.Log("index " + index);

                WaypointGroup group = parent.gameObject.GetComponent<WaypointGroup>();
                Vector3 p = group.getPoint(currentMovementWaypoint, ((float)index) / 100);
                if (p == Vector3.zero)
                {
                    Debug.Log("No position behind");
                }
                else if (p == transform.position)
                {
                    index -= 2;
                }
                else {
                    //Debug.Log("Moving " + p);
                    movement = p;
                }
            }
        }
        else if (moveLeft < 0)
        {
            Transform parent = currentMovementWaypoint.transform.parent;
            if (parent == null)
            {
                Debug.Log("No parent");
            }
            else if (parent.gameObject.GetComponent<WaypointGroup>() == null)
            {
                Debug.Log("No WaypointGroup");
            }
            else
            {
                index = index - 1;
                if (index < 0)
                {

                    if (currentMovementWaypoint.previous != null && currentMovementWaypoint.previous.previous != null)
                    {
                        index = 100;
                        currentMovementWaypoint = currentMovementWaypoint.previous;
                    }
                    else
                    {
                        index = 0;
                        return Vector3.zero;
                    }
                }

                //Debug.Log("Is correct type");
                WaypointGroup group = parent.gameObject.GetComponent<WaypointGroup>();
                Vector3 p = group.getPoint(currentMovementWaypoint, ((float)index) / 100);
                if (p == Vector3.zero)
                {
                    Debug.Log("No position ahead");
                }
                else if (p == transform.position)
                {
                    index += 2;
                }
                else {
                    //Debug.Log("Moving " + p);
                    movement = p;
                }
            }
        }
        
        //movement.y = transform.position.y-0.1f;
        transform.LookAt(movement);
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, transform.localEulerAngles.z);

        Vector3 newPos = transform.forward * Mathf.Abs(moveLeft) * speed.z;
        movement += transform.forward * moveForward * speed.x;
        newPos *= Time.deltaTime;
        return newPos;
    }

    /*Vector3 moveWithWaypoints(float moveLeft, float moveForward)
    {
        if( currentMovementWaypoint == null)
        {
            Debug.LogError("No Waypoint assigned to player. Can not move!");
        }

        // Get the next point we want to move to
        MovementWaypoint nextPoint = null;
        if( moveLeft > 0 && currentMovementWaypoint.next != null )
        {
            nextPoint = currentMovementWaypoint.next;

            // Check if we have already hit this target
            Vector3 nextPosition = new Vector3(nextPoint.transform.position.x, transform.position.y, nextPoint.transform.position.z);
            if ((nextPosition - transform.position).magnitude < changeWaypointDistance)
            {
                currentMovementWaypoint = nextPoint;
                nextPoint = nextPoint.next;
            }
        }
        else if (moveLeft < 0 )
        {
            // When moving left. We want to move BACK to the current waypoint.
            nextPoint = currentMovementWaypoint;

            // Check if we have already hit this target
            Vector3 nextPosition = new Vector3(nextPoint.transform.position.x, transform.position.y, nextPoint.transform.position.z);
            if ((nextPosition - transform.position).magnitude < changeWaypointDistance)
            {
                nextPoint = nextPoint.previous;

                // If we do not have a previosu waypoint to move to. Don't save it as our current waypoint
                if (currentMovementWaypoint.previous != null) { 
                    currentMovementWaypoint = nextPoint;
                }
            }
        }

        // Make sure we can move somewhere
        if (nextPoint != null)
        {
            //Debug.LogWarning("Current point " + currentMovementWaypoint.name);
            //Debug.LogWarning("Next point " + nextPoint.name);

            // Look at the next point
            transform.LookAt(nextPoint.transform);
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, transform.localEulerAngles.z);

            Vector3 movement = transform.forward * Mathf.Abs(moveLeft) * speed.z;
            //movement += transform.forward * moveForward * speed.x;
            movement *= Time.deltaTime;
            return movement;
        }

        // Can not move
        return Vector3.zero;
    }*/

    /*void rotateToFloor()
    {
        float range = 1000f;
        float speed = 50.0f;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, range))
        {
            GameObject o = hit.collider.gameObject;
            //Debug.LogWarning("XHIT TARGET " + o.name + " " + o.tag);
            if (o != null)
            {
                var targetRotation = o.transform.rotation;
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
            }
        }
    }*/

    private void playFootStep()
    {
        if( Time.time < nextFootStepSound)
        {
            return;
        }

        // Play sound
        if(nextFootStepSoundLeft)
        {
            SoundMaster.playRandomSound(leftFootStepSounds, leftFootStepSoundsVolume, audioSource);
        }
        else
        {
            SoundMaster.playRandomSound(rightFootStepSounds, rightFootStepSoundsVolume, audioSource);
        }

        // Delay next step
        nextFootStepSound = Time.time + footStepSoundDelay;


    }
}

using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {
    
    CharacterController controller;

    public bool EnableLeftRightMovement = true; // can we move left/right?
    public bool EnableForwardBackMovement = false; // can we move forward/back?
    public bool EnableMoveAccordingToCamera = true; // We move accorging to the angle of the camera
    public Vector3 speed = new Vector3(5f, 5f, 5f);
    public float gravity = 1f;
    public float jump = 5f;

    public MovementWaypoint currentMovementWaypoint;
    public float changeWaypointDistance = 1; // Distance we must get to in order to chaneg to the next waypoint

    float falling = 0f;

    // SOUNDS
    private bool fallSoundPlayed = false;
    private bool nextFootStepSoundLeft = true;
    private float nextFootStepSound = 0f;
    public float footStepSoundDelay = 1f;

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
        transform.position = currentMovementWaypoint.transform.position;

        if( !EnableMoveAccordingToCamera)
        {
            // Look at the next point
            if (currentMovementWaypoint != null && currentMovementWaypoint.next != null)
            {
                transform.LookAt(currentMovementWaypoint.next.transform);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        zoomCamera();

        float moveHorizontal = EnableLeftRightMovement ? Input.GetAxis("Horizontal") : 0;
        float moveVertical = EnableForwardBackMovement ? Input.GetAxis("Vertical") : 0;

        Vector3 movement = Vector3.zero;
        if( EnableMoveAccordingToCamera)
        {
            movement = Camera.main.transform.forward * moveVertical* speed.z;
            movement += Camera.main.transform.right * moveHorizontal * speed.x;
            movement *= Time.deltaTime;
        }
        else
        {
            movement = moveWithWaypoints(moveHorizontal, moveVertical);
        }


        if (controller.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                SoundMaster.playRandomSound(jumpSounds, jumpSoundsVolume, transform.position);
                falling = jump;
            }

            if (falling < 0 && !fallSoundPlayed)
            {
                SoundMaster.playRandomSound(fallSounds, fallSoundsVolume, transform.position);
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

    Vector3 moveWithWaypoints(float moveLeft, float moveForward)
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
    }

    void zoomCamera()
    {
        const float zoomScalar = 10;

        float zoom = Input.GetAxis("Mouse ScrollWheel");
        if (zoom != 0)
        {
            //Debug.LogWarning("SCROLL " + zoom);
            Camera camera = Camera.main;
            ChasePlayer chase = camera.GetComponent<ChasePlayer>();
            chase.distance += -zoom*zoomScalar;
        }
    }

    void rotateToFloor()
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
    }

    private void playFootStep()
    {
        if( Time.time < nextFootStepSound)
        {
            return;
        }

        // Play sound
        if(nextFootStepSoundLeft)
        {
            SoundMaster.playRandomSound(leftFootStepSounds, leftFootStepSoundsVolume, transform.position);
        }
        else
        {
            SoundMaster.playRandomSound(rightFootStepSounds, rightFootStepSoundsVolume, transform.position);
        }

        // Delay next step
        nextFootStepSound = Time.time + footStepSoundDelay;

        
    }
}

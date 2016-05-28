using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {
    
    private CharacterController controller;
    private AudioSource audioSource;

    public bool EnableLeftRightMovement = true; // can we move left/right?
    public bool EnableForwardBackMovement = false; // can we move forward/back?
    public bool EnableMoveAccordingToCamera = true; // We move accorging to the angle of the camera
    public Vector3 speed = new Vector3(5f, 5f, 5f);
    public float turnSpeed = 10f;
    public float gravity = 1f;
    public float jump = 5f;

    public MovementWaypoint currentMovementWaypoint;
    public float changeWaypointDistance = 1; // Distance we must get to in order to chaneg to the next waypoint

    private float falling = 0f;
    private Quaternion rotateTo = Quaternion.identity;

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

		if (currentMovementWaypoint != null) {
			transform.position = currentMovementWaypoint.transform.position;
		}

        if( !EnableMoveAccordingToCamera)
        {
            // Look at the next point
            if (currentMovementWaypoint != null && currentMovementWaypoint.next != null)
            {
                transform.LookAt(currentMovementWaypoint.next.transform);
            }
        }
    }

    public void ShakeCamera(float intensity)
    {
        Camera cam = Camera.main;
        ShakeCamera shake = cam.GetComponent<ShakeCamera>();
        if (shake != null)
        {
            shake.DoShake(intensity);
        }
    }

    // Update is called once per frame
    void Update() {
        if (controller == null) { //If the player can't be controlled, don't let it move
            return;
        }

        if (rotateTo != Quaternion.identity) {
            transform.rotation = Quaternion.Slerp(transform.rotation, rotateTo, Time.deltaTime * turnSpeed);
        }

        // Stop the player from doing stuff if we are phasing
        PhaseJump phaseJump = GetComponent<PhaseJump>();
        if(phaseJump != null && phaseJump.isPhasing())
        {
            return;
        }

        // TODO Hardcode smoth stop movement
        float moveHorizontal = EnableLeftRightMovement ? Input.GetAxisRaw("Horizontal") : 0;

        Vector3 movement = Vector3.zero;
        if( EnableMoveAccordingToCamera)
        {
            movement = Camera.main.transform.right * moveHorizontal * speed.x;
            movement *= Time.deltaTime;
        }
        else if( moveHorizontal != 0 )
        {
            movement = moveWithWaypoints(moveHorizontal);
        }


        if (controller.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                SoundMaster.playRandomSound(jumpSounds, jumpSoundsVolume, getAudioSource());
                falling = jump;
            }

            if (falling < 0 && !fallSoundPlayed)
            {
                //Debug.Log("Falled");
                SoundMaster.playRandomSound(fallSounds, fallSoundsVolume, getAudioSource());
                fallSoundPlayed = true;

                //float intensity = Mathf.Max(0, Mathf.Min(1,Mathf.Abs((falling))*0.5f));
                //ShakeCamera(intensity);
                //Debug.Log("Shake " + intensity);
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

    Vector3 moveWithWaypoints(float moveLeft)
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
            // Face in drection of movement
            var flatVectorToTarget = transform.position - nextPoint.transform.position;
            flatVectorToTarget.y = 0;
            Quaternion newRotation = Quaternion.LookRotation(flatVectorToTarget);
            Debug.Log("AngleA " + Quaternion.Angle(transform.rotation, newRotation));
            Debug.Log("AngleT " + transform.rotation.eulerAngles);
            newRotation *= Quaternion.Euler(0, 178, 0);

            Vector3 camPos = Camera.main.transform.position;

            float thingAngle = Vector3.Angle(transform.forward, camPos - transform.position);
            bool facing = thingAngle > 90;
            Debug.Log("Is Rotating to Camera: " + facing + " (" + thingAngle + ")");


            //Debug.Log("Angle " + newRotation.eulerAngles);
            rotateTo = newRotation;

            // Move towards target
            Vector3 moveToPoint = nextPoint.transform.position-transform.position;
            float mag = moveToPoint.magnitude;
            float scalar = 2f / mag;
            moveToPoint *= scalar;

            moveToPoint += transform.position;
            moveToPoint.y = transform.position.y;


            float time = speed.z * Time.deltaTime;
            Vector3 nextPosition = Vector3.MoveTowards(transform.position, moveToPoint, time);

            Vector3 movement = nextPosition - transform.position;
            //Debug.Log("movement " + movement + " mag " + movement.magnitude);
            return movement;
        }


        // Can not move
        return Vector3.zero;
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
            SoundMaster.playRandomSound(leftFootStepSounds, leftFootStepSoundsVolume, getAudioSource());
        }
        else
        {
            SoundMaster.playRandomSound(rightFootStepSounds, rightFootStepSoundsVolume, getAudioSource());
        }

        // Delay next step
        nextFootStepSound = Time.time + footStepSoundDelay;


    }

    public AudioSource getAudioSource()
    {
        if (audioSource == null)
        {
            Debug.LogError("Object " + gameObject.name + " does not have an AudioSource Component!");
        }
        return audioSource;
    }
}

using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

    private Animator anim;
    private CharacterController controller;
    private AudioSource audioSource;
    
    public Vector3 speed = new Vector3(5f, 5f, 5f);
    public float movementDecay = 4f;
    public float movementIncrement = 3f;
    public float turnSpeed = 10f;
    public float gravity = 12f; //gravity acceleration
    public float jump = 5f; // jump velocity

    public MovementWaypoint currentMovementWaypoint;
    public float changeWaypointDistance = 1; // Distance we must get to in order to chaneg to the next waypoint

    private float falling = 0f;
    private float moveTime = 1f;
    private Vector3 moveToPosition = new Vector3(0, 0, 0);
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
        anim = GetComponent<Animator>();

        if (currentMovementWaypoint != null) {
			transform.position = currentMovementWaypoint.transform.position;
		}

        // Look at the next point
        if (currentMovementWaypoint != null && currentMovementWaypoint.next != null)
        {
            transform.LookAt(currentMovementWaypoint.next.transform);
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

        float previewCamera = Input.GetAxis("PreviewPhase");
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        MovementWaypoint nextPoint = getNextWaypoint(moveHorizontal);
        rotatePlayer(moveHorizontal, nextPoint);

        // Start walking to waypoint
        Vector3 movement = moveToPosition;
        if (moveHorizontal != 0 && previewCamera == 0)
        {
            // Get the move vector and slowy start moving
            moveToPosition = moveWithWaypoints(nextPoint);
            movement = moveToPosition;
            moveTime = Mathf.Min(1, moveTime+Time.deltaTime* movementIncrement);
            anim.SetBool("IsWalking", true);
        }
        else  {
            // Slowly stop moving
            moveTime = Mathf.Max(0, moveTime - Time.deltaTime * movementDecay);
            anim.SetBool("IsWalking", false);
        }
        movement *= moveTime;


        if (controller.isGrounded)
        {
            anim.SetBool("IsInAir", false);
            if (Input.GetButtonDown ("Jump")) {
				SoundMaster.playRandomSound (jumpSounds, jumpSoundsVolume, getAudioSource ());
				falling = jump;

                anim.SetTrigger("isJumping");
            } else {
				falling = -1f; //Reset falling speed to stop massively quick falls
                anim.SetBool("isJumping",false);
            }

            if (falling < 0f && !fallSoundPlayed)
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
            anim.SetBool("IsInAir", true);
            fallSoundPlayed = false;
        }

		//Apply gravity (always, or isGrounded doesn't work properly)
		falling -= gravity * Time.deltaTime; //convert gravity to velocity

        // Save current Y
        float currentY = transform.position.y;

        // Move
        movement.y = falling;

        Vector3 movementVector = new Vector3(movement.x, movement.y, movement.z) * Time.deltaTime;
        controller.Move (movementVector);

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

        // Animation
        updateAnimation(movementVector);
    }

    void rotatePlayer(float moveLeft, MovementWaypoint nextPoint)
    {
        if( nextPoint == null || Input.GetAxis("PreviewPhase") != 0)
        {
            // Can't rotate
            return;
        }

        // Face in drection of movement
        var flatVectorToTarget = transform.position - nextPoint.transform.position;
        flatVectorToTarget.y = 0;
        Quaternion newRotation = Quaternion.LookRotation(flatVectorToTarget);
        //Debug.Log("AngleA " + Quaternion.Angle(transform.rotation, newRotation));
        //Debug.Log("AngleT " + transform.rotation.eulerAngles);
        newRotation *= Quaternion.Euler(0, 178, 0);

        //Debug.Log("Angle " + newRotation.eulerAngles);
        rotateTo = newRotation;
    }

    MovementWaypoint getNextWaypoint(float moveLeft)
    {
        if (currentMovementWaypoint == null)
        {
            Debug.LogError("No Waypoint assigned to player. Can not move!");
        }

        // Get the next point we want to move to
        MovementWaypoint nextPoint = null;
        if (moveLeft > 0 && currentMovementWaypoint.next != null)
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
        else if (moveLeft < 0)
        {
            // When moving left. We want to move BACK to the current waypoint.
            nextPoint = currentMovementWaypoint;

            // Check if we have already hit this target
            Vector3 nextPosition = new Vector3(nextPoint.transform.position.x, transform.position.y, nextPoint.transform.position.z);
            if ((nextPosition - transform.position).magnitude < changeWaypointDistance)
            {
                nextPoint = nextPoint.previous;

                // If we do not have a previosu waypoint to move to. Don't save it as our current waypoint
                if (currentMovementWaypoint.previous != null)
                {
                    currentMovementWaypoint = nextPoint;
                }
            }
        }
        return nextPoint;
    }

    Vector3 moveWithWaypoints(MovementWaypoint nextPoint)
    {
        // Make sure we can move somewhere
        if (nextPoint == null){
            // Can not move
            return Vector3.zero;
        }

		//Get target position, ignoring y
		Vector3 moveToPoint = new Vector3 (nextPoint.transform.position.x, transform.position.y, nextPoint.transform.position.z);

        float time = speed.z * Time.deltaTime;
		Vector3 nextPosition = Vector3.MoveTowards(transform.position, moveToPoint, time);

        Vector3 movement = nextPosition - transform.position;

		return movement.normalized * speed.x;
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



    //
    // Animations
    //
    private int isInAir = Animator.StringToHash("IsInAir");
    private int isWalking = Animator.StringToHash("IsWalking");

    private bool inAir = false;

    public void updateAnimation(Vector3 movement)
    {

        /*bool isGrounded = Mathf.Abs(movement.y) < 0.025;
        if ( !inAir && !isGrounded)
        {
            Debug.Log("In Air");
            anim.SetBool(isInAir,true);
            inAir = !inAir;
        }
        else if( inAir && isGrounded)
        {
            Debug.Log("Not In Air");
            anim.SetBool(isInAir, false);
            inAir = !inAir;
        }*/
        
    }






}

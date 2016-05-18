using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

public class ChasePlayer : MonoBehaviour
{
    //public AnimationCurve ApertureChase;
    public GameObject whatToChase = null;
    public Vector3 rotationVector = new Vector3(0f, 0f, 0f);
	public float distance = 10f;
    public float chaseSpeed = 10f;
	public float rotateSpeed = 20f;
    public float minFallShakeDistance = 0.5f;

    public bool chaseX = true;
    public bool chaseY = true;
    public bool chaseZ = true;
    public bool enableChase = true;
    public bool startInPosition = true;
    public bool startLookingAtPlayer = true;
    public bool lookAtPlayer = true;
	public bool waypointTracking = true;

    private ShakeCamera shake;
	private Vector3 targetPosition;
    private Vector3 lastPosition;
    private Vector3 lastWatchedPosition;
    private float lastChangeInY;

    private Component dofs;
    private float savedAperture;
    private float setX = float.MaxValue;
    private float setY = float.MaxValue;
    private float setZ = float.MaxValue;

    // These values get changed in the constructor
    private bool originalChaseXState;
    private bool originalChaseYState;
    private bool originalChaseZState;

	private Vector3 previousRotation = Vector3.zero;

    // Use this for initialization
    void Start()
    {
        shake = GetComponent<ShakeCamera>();

        // We need the original states to be the opposite
        // So we can save the set position
        originalChaseXState = !chaseX;
        originalChaseYState = !chaseY;
        originalChaseZState = !chaseZ;

        // Set our position to where we should be
        if (startInPosition)
        {
            this.transform.position = getNewPosition();
        }

        // Start looking at the player
        if (startLookingAtPlayer)
        {
            this.transform.LookAt(whatToChase.transform);
        }

		//Waypoint tracking must have player tracking
		if (waypointTracking) {
			lookAtPlayer = true;
		}

        lastPosition = transform.position;
        dofs = GetComponent("DepthOfField");
        if( dofs != null)
        {
            // Save what aperture currently is
            FieldInfo fi = dofs.GetType().GetField("aperture");
            savedAperture = float.Parse(fi.GetValue(dofs).ToString());
        }
    }

    public void setDOFAperture(float a)
    {
        //  Blur according to the distance the camer is at
        if (dofs != null)
        {
            FieldInfo fi = dofs.GetType().GetField("aperture");
            fi.SetValue(dofs, Mathf.Abs(distance));
        }
    }

    void zoomCamera()
    {
        const float zoomScalar = 10;

        float zoom = Input.GetAxis("Mouse ScrollWheel");
        if (zoom != 0)
        {
            distance += -zoom * zoomScalar;

            //  Blur according to the distance the camer is at
            if (dofs != null)
            {
                FieldInfo fi = dofs.GetType().GetField("focalLength");
                fi.SetValue(dofs, Mathf.Abs(distance));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //this.transform.position = lastPosition;
        zoomCamera();

		//TODO: Fix chasing when phasing -- it looks weird
        if (enableChase)
        {
            Vector3 newPos = getNewPosition();
            float time = chaseSpeed * Time.deltaTime;

            this.transform.position = Vector3.Slerp(transform.position, newPos, time);
        }
        

        // Look at the object we want to chase
        if (lookAtPlayer)
        {
			//float time = rotateSpeed * Time.deltaTime;
            this.transform.LookAt(whatToChase.transform);
			//transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(whatToChase.transform.position - transform.position), time);
        }


        float changeInY = lastWatchedPosition.y - whatToChase.transform.position.y;
        lastPosition = this.transform.position;
        lastWatchedPosition = whatToChase.transform.position;
        lastChangeInY = changeInY;


        if (changeInY >= minFallShakeDistance && lastChangeInY > 0.1)
        {
            //Debug.Log("=Change " + changeInY);
            //Debug.Log("=lastChange " + lastChangeInY);
            shake.DoShake();
        }
        else if (changeInY > 0.1 || changeInY < -0.1 )
        {
            //Debug.Log("Change " + changeInY);
            //Debug.Log("lastChange " + lastChangeInY);
        }


        //shake.DoShake();
    }

    public Vector3 getNewPosition()
    {
        // Assign a new position if chaseX has changed!
        if (originalChaseXState != chaseX) { setX = whatToChase.transform.position.x; originalChaseXState = chaseX; }
        if (originalChaseYState != chaseY) { setY = whatToChase.transform.position.y; originalChaseYState = chaseY; }
        if (originalChaseZState != chaseZ) { setZ = whatToChase.transform.position.z; originalChaseZState = chaseZ; }

        Vector3 offset = new Vector3(0, 0, -distance);
        Vector3 expectedPosition = Vector3.zero;
        if (chaseX) { expectedPosition.x = whatToChase.transform.position.x; } else { expectedPosition.x = setX; }
        if (chaseY) { expectedPosition.y = whatToChase.transform.position.y; } else { expectedPosition.y = setY; }
        if (chaseZ) { expectedPosition.z = whatToChase.transform.position.z; } else { expectedPosition.z = setZ; }


        Vector3 offsetPosition = expectedPosition + offset;
		Vector3 rotation = rotationVector;
		if (waypointTracking) {
			rotation += RotatePerpendicularToWaypoint ();
		}
		Vector3 rotatedVector = RotatePointAroundPivot(offsetPosition, expectedPosition, RotatePerpendicularToWaypoint()+rotationVector);


        //Debug.LogWarning("expectedPosition : " + expectedPosition);
        //Debug.LogWarning("offsetPosition : " + offsetPosition);
        //Debug.LogWarning("rotatedVector : " + rotatedVector);
        return rotatedVector;
    }

    public void instantlyMoveToPlayer()
    {
        Vector3 newPos = getNewPosition();
        this.transform.position = newPos;


        // Look at the object we want to chase
        this.transform.LookAt(whatToChase.transform);
    }
		
	private Vector3 RotatePerpendicularToWaypoint(){
		Movement mov = whatToChase.GetComponent<Movement>();
	
		if (mov == null || mov.currentMovementWaypoint == null) {
			//Do nothing if we can't find a waypoint
			return previousRotation;
		}
			
		MovementWaypoint way1 = mov.currentMovementWaypoint;
		MovementWaypoint way2 = way1.next;

		if (way2 != null) {
			//Calculate a vector perpendicular to the vector between the two points
			//and rotation angle
			Vector3 pos1 = new Vector3(way1.transform.position.x, 0, way1.transform.position.z);
			Vector3 pos2 = new Vector3 (way2.transform.position.x, 0, way2.transform.position.z);

			Vector3 path = pos2 - pos1;
			Vector3 normal = Vector3.Cross(path, Vector3.up);

			float rotation = Vector3.Angle(Vector3.back, normal);

			//check whether to rotate left or right
			Vector3 cross = Vector3.Cross(Vector3.back, normal);
			if (cross.y < 0) {
				rotation = -rotation;
			}

			previousRotation = new Vector3 (0, rotation, 0);
			return new Vector3(0, rotation, 0);
		}

		return previousRotation;
	}

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }
}
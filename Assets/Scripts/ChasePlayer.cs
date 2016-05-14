using UnityEngine;
using System.Collections;

public class ChasePlayer : MonoBehaviour
{

    public GameObject whatToChase = null;
    public Vector3 rotationVector = new Vector3(0f, 0f, 0f);
    public float distance = 10f;
    public float chaseSpeed = 10f;

    public bool chaseX = true;
    public bool chaseY = true;
    public bool chaseZ = true;
    public bool enableChase = true;
    public bool startInPosition = true;
    public bool startLookingAtPlayer = true;
    public bool lookAtPlayer = true;

    private float setX = float.MaxValue;
    private float setY = float.MaxValue;
    private float setZ = float.MaxValue;

    // These values get changed in the constructor
    private bool originalChaseXState;
    private bool originalChaseYState;
    private bool originalChaseZState;

    // Use this for initialization
    void Start()
    {

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
    }

    // Update is called once per frame
    void Update()
    {
        
        // Get the new position
        if (enableChase)
        {
            Vector3 newPos = getNewPosition();
            float time = chaseSpeed * Time.deltaTime;

            this.transform.position = Vector3.Slerp(this.transform.position, newPos, time);
        }
        

        // Look at the object we want to chase
        if (lookAtPlayer)
        {
            this.transform.LookAt(whatToChase.transform);
        }
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
        Vector3 rotatedVector = RotatePointAroundPivot(offsetPosition, expectedPosition, rotationVector);


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

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }
}

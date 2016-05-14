using UnityEngine;
using System.Collections;

public class ModifyCameraRotation : MonoBehaviour {

    // When we enter this Trigger. What should we change to?
    public float newDistance1Enter = 0f;
    public Vector3 newRotation1Enter = new Vector3();

    // When we leave exit 1. What angle should we leave?
    public float newDistance1Exit = 0f;
    public Vector3 newRotation1Exit = new Vector3();
    public Vector3 newRotation1ExitActivation = new Vector3(0, 0, 0); // Which direction will activate this rotation

    // When we leave exit 2. What angle should we leave?
    public float newDistance2Exit = 0f;
    public Vector3 newRotation2Exit = new Vector3();
    public Vector3 newRotation2ExitActivation = new Vector3(0, 0, 0); // Which direction will activate this rotation

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Camera cam = Camera.main;
            ChasePlayer chase = cam.GetComponent<ChasePlayer>();

            // Rotate to new Stuff
            chase.rotationVector = newRotation1Enter;
            chase.distance = newDistance1Enter;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if( other.gameObject.tag == "Player")
        {
            Camera cam = Camera.main;
            ChasePlayer chase = cam.GetComponent<ChasePlayer>();

            // Reset
            int choose = getCameraModifyOptions(other);
            if (choose == 1)
            {
                chase.rotationVector = newRotation1Exit;
                chase.distance = newDistance1Exit;
            }
            else if (choose == 2)
            {
                chase.rotationVector = newRotation2Exit;
                chase.distance = newDistance2Exit;
            }
        }
    }

    private int getCameraModifyOptions(Collider coll)
    {
        RaycastHit MyRayHit;
        Vector3 direction = (transform.position - coll.gameObject.transform.position).normalized;
        Ray MyRay = new Ray(coll.gameObject.transform.position, direction);

        if (Physics.Raycast(MyRay, out MyRayHit))
        {

            if (MyRayHit.collider != null)
            {

                Vector3 MyNormal = MyRayHit.normal;
                MyNormal = MyRayHit.transform.TransformDirection(MyNormal);

                if (MyNormal == MyRayHit.transform.up) { return getIndexFromVector(HitDirection.Top); }
                if (MyNormal == -MyRayHit.transform.up) { return getIndexFromVector(HitDirection.Bottom); }
                if (MyNormal == MyRayHit.transform.forward) { return getIndexFromVector(HitDirection.Front); }
                if (MyNormal == -MyRayHit.transform.forward) { return getIndexFromVector(HitDirection.Back); }
                if (MyNormal == MyRayHit.transform.right) { return getIndexFromVector(HitDirection.Right); }
                if (MyNormal == -MyRayHit.transform.right) { return getIndexFromVector(HitDirection.Left); }
            }
        }

        // Couldn't do it
        return -1;
    }

    private enum HitDirection { None, Top, Bottom, Front, Back, Left, Right }
    private int getIndexFromVector(HitDirection v)
    {
        // +X
        if (v == HitDirection.Right && newRotation1ExitActivation.x > 0){ return 1; }
        if (v == HitDirection.Right && newRotation2ExitActivation.x > 0) { return 2; }

        // -X
        if (v == HitDirection.Left && newRotation1ExitActivation.x < 0) { return 1; }
        if (v == HitDirection.Left && newRotation2ExitActivation.x < 0) { return 2; }

        // +Y
        if (v == HitDirection.Top && newRotation1ExitActivation.y > 0) { return 1; }
        if (v == HitDirection.Top && newRotation2ExitActivation.y > 0) { return 2; }

        // -Y
        if (v == HitDirection.Bottom && newRotation1ExitActivation.y < 0) { return 1; }
        if (v == HitDirection.Bottom && newRotation2ExitActivation.y < 0) { return 2; }


        // +Z
        if (v == HitDirection.Front && newRotation1ExitActivation.z > 0) { return 1; }
        if (v == HitDirection.Front && newRotation2ExitActivation.z > 0) { return 2; }

        // -Z
        if (v == HitDirection.Back && newRotation1ExitActivation.z < 0) { return 1; }
        if (v == HitDirection.Back && newRotation2ExitActivation.z < 0) { return 2; }

        return -1;
    }
}

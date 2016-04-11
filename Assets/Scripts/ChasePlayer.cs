using UnityEngine;
using System.Collections;

public class ChasePlayer : MonoBehaviour {

    public GameObject whatToChase = null;
    public Vector3 rotationVector = new Vector3(0f,0f,0f);
    public float distance = 10f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        

        Vector3 offset = new Vector3(0, 0, -distance);
        Vector3 newPos = RotatePointAroundPivot(whatToChase.transform.position+offset, whatToChase.transform.position, rotationVector);
        this.transform.position = Vector3.Slerp(this.transform.position, newPos, 4 * Time.deltaTime); ;


        /*
        this.transform.position = whatToChase.transform.position + new Vector3(0,distance,0);
        transform.RotateAround(whatToChase.transform.position, new Vector3(1, 0, 0), rotation.x);
        transform.RotateAround(whatToChase.transform.position, new Vector3(0, 1, 0), rotation.y);
        transform.RotateAround(whatToChase.transform.position, new Vector3(0, 0, 1), rotation.z);*/


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

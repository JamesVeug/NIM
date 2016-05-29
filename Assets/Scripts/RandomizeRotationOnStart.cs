using UnityEngine;
using System.Collections;

public class RandomizeRotationOnStart : MonoBehaviour {

    public float minXRotation = 1;
    public float maxXRotation = 1;
    public float minYRotation = 1;
    public float maxYRotation = 1;
    public float minZRotation = 1;
    public float maxZRotation = 1;

    // Use this for initialization
    void Start () {
        float x = Random.Range(minXRotation, maxXRotation);
        float y = Random.Range(minYRotation, maxYRotation);
        float z = Random.Range(minZRotation, maxZRotation);
        transform.rotation = Quaternion.Euler(new Vector3(x, y, z));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

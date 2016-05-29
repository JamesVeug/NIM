using UnityEngine;
using System.Collections;

public class RandomizeScaleOnStart : MonoBehaviour {


    public float minXScale = 1;
    public float maxXScale = 1;
    public float minYScale = 1;
    public float maxYScale = 1;
    public float minZScale = 1;
    public float maxZScale = 1;

    // Use this for initialization
    void Start () {
        transform.localScale = new Vector3(Random.Range(minXScale, maxXScale), Random.Range(minYScale, maxYScale), Random.Range(minZScale, maxZScale));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

using UnityEngine;
using System.Collections;

public class BallTrigger : MonoBehaviour {

    public Rigidbody ball;
    public bool freezeZ;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void onTriggerEnter()
    {
        if (freezeZ == true)
        {
            ball.constraints = RigidbodyConstraints.FreezePositionZ;

        }
    }
    
}

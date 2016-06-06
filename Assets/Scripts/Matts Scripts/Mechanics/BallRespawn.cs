using UnityEngine;
using System.Collections;

public class BallRespawn : MonoBehaviour {

    private Vector3 respawnPoint;
    public float downwardsForce;
    public float respawnY;


	// Use this for initialization
	void Start () {
       
        respawnPoint = this.transform.position;
        
            

    }
	
	void FixedUpdate () {
        Rigidbody rb = this.GetComponent<Rigidbody>();
        rb.AddForce(Physics.gravity*downwardsForce, ForceMode.Acceleration);
        if (this.transform.position.y <= respawnY) {
            this.transform.position = respawnPoint;
            rb.AddForce(Physics.gravity * 0, ForceMode.Acceleration);
        }
	}
}

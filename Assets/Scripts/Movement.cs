using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

    public float MoveSpeed = 6f;
    public float JumpSpeed = 10f;
    public float Gravity = 1f;
    
    private Vector3 lastPosition;
    private CharacterController controller;

    private float vertical_velocity = 0f;

	// Use this for initialization
	void Start () {
        controller = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {


        // Move left/right
        float horizontal_velocity = MoveSpeed * Input.GetAxis("Horizontal");

        // Move up/down
        if ( controller.isGrounded)
        {
            if ( Input.GetButtonDown("Jump"))
            {
                vertical_velocity = JumpSpeed;
            }
            else
            {
                vertical_velocity = -1f;
            }
        }
        else
        {
            vertical_velocity -= Gravity;
        }
        

        controller.Move(new Vector3(horizontal_velocity, vertical_velocity, 0f) * Time.deltaTime);
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PickupTemp1 : MonoBehaviour {

    private float y0;
    private float amplitude = 1;
    private float speed = 1;
    private float rotateSpeed = -25.0f;

   	// Use this for initialization
	void Start () {
        y0 = transform.position.y;
        //transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        amplitude = Random.Range(0.15f, 0.25f);
        speed = Random.Range(0.8f, 1.2f);
        rotateSpeed = Random.Range(20f, 30f);

       
    }
	
	// Update is called once per frame
	void Update () {
        // Put the floating movement in the Update function:
        Vector3 p = transform.position;
        p.y = y0 + amplitude * Mathf.Sin(speed * Time.time);
        transform.position = p;

        transform.Rotate(transform.up * rotateSpeed * Time.deltaTime);
    }
}

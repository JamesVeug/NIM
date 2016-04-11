using UnityEngine;
using System.Collections;

public class FloorButton : MonoBehaviour {

    private const int STATE_IDLE = 0;
    private const int STATE_CLOSING = 1;
    private const int STATE_CLOSED = 2;

    private int state = STATE_IDLE;

    public float closeDistance = 2f;

    private Vector3 initPos;
    private Vector3 closePos;

	// Use this for initialization
	void Start () {
        initPos = transform.position;
        closePos = transform.position - new Vector3(0f, closeDistance,0f);
    }
	
	// Update is called once per frame
	void Update () {

        if (state == STATE_CLOSING)
        {
            if ((closePos - transform.position).magnitude <= 0.1f)
            {
                state = STATE_CLOSED;
                closed();
            }
            else {
                transform.position += (closePos - initPos) * Time.deltaTime;
            }
        }
	}

    void OnTriggerEnter(Collider c)
    {
        Debug.LogWarning("COLLIDED");
        if (c.gameObject.tag == "Player")
        {
            Debug.LogWarning("PLAYER");
            state = STATE_CLOSING;
        }
    }

    void closed()
    {
        GameObject door = GameObject.Find("TrapDoor");
        Debug.LogWarning("Door " + door);

        door.transform.Rotate(new Vector3(0, 0, 90));
    }
}

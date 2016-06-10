using UnityEngine;
using System.Collections;

public class ScatterBuzzers : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {

            foreach (Transform t in transform)
            {
                AIBuzzer buzz = t.GetComponent<AIBuzzer>();
                if (buzz != null)
                {
                    buzz.spanRange();
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {

            foreach (Transform t in transform)
            {
                AIBuzzer buzz = t.GetComponent<AIBuzzer>();
                if (buzz != null)
                {
                    buzz.shrinkRange();
                }
            }
        }
    }
}

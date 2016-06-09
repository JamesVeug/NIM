using UnityEngine;
using System.Collections;

public class LightFlicker : MonoBehaviour {
    private Light light;

    public float max = 1.5f;
    public float min = 0.5f;
    public float frequency = 1f;


    private float initial;
    private float lastChange = 0f;

	// Use this for initialization
	void Start () {
        light = gameObject.GetComponent<Light>();
        initial = light.intensity;
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (lastChange >= frequency)
        {
            float r = Random.Range(min, max);
            light.intensity = r;
            lastChange = 0f;
        }
        else
        {
            lastChange += Time.deltaTime;
        }
	}
}

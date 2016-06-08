using UnityEngine;
using System.Collections;

public class FadeLightOnStart : MonoBehaviour {
    private Light light;

    public float timefade = 0.15f;

    private float currentTime = 0f;
    private float initial;

    // Use this for initialization
    void Start()
    {
        light = gameObject.GetComponent<Light>();
        initial = light.intensity;
        currentTime = timefade;
    }

    // Update is called once per frame
    void LateUpdate()
    {

        light.intensity = initial * (currentTime/ timefade);
        currentTime -= Time.deltaTime;
    }
}

using UnityEngine;
using System.Collections;

public class PlayerFlashLight : MonoBehaviour {

    public bool startedOn;
    public Light[] lights;

    public bool isOn;

	// Use this for initialization
	void Start () {
        isOn = startedOn;
        toggledFlashlight();
	}

    void toggledFlashlight()
    {

        foreach (Light l in lights)
        {
            l.enabled = isOn;
        }
    }
	
	// Update is called once per frame
	void Update () {
	
        if(Input.GetButtonDown("FlashLight"))
        {
            isOn = !isOn;
            toggledFlashlight();
        }
        else if( Input.GetButtonDown("Cancel"))
        {
            // HACKY HACKY HACKY HACKY HACKY
            Application.Quit();
        }
	}
}

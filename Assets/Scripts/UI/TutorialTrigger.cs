using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TutorialTrigger : MonoBehaviour {
	public static float FADE_TIME = 1.0f;

	[TextArea(3,10)]
	public string text = "";

	private UITutorial panel;

	// Use this for initialization
	void Awake () {
		panel = FindObjectOfType<UITutorial>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag != "Player") {
			return; //Only the player triggers tutorials
		}

        if (panel != null)
        {
            panel.ShowText(text, FADE_TIME);
        }
	}

	void OnTriggerExit(Collider other){
		if (other.gameObject.tag != "Player") {
			return; //Only the player triggers tutorials
		}

        if (panel != null)
        {
            panel.FadeOut(FADE_TIME);
        }
	}
}

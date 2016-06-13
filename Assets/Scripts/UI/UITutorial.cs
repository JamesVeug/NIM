using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UITutorial : MonoBehaviour {
	public static float DELAY = 2f;

	Text textbox;
	CanvasGroup panel;

	IEnumerator current; //the currently active coroutine

	void Awake () {
		textbox = this.GetComponentInChildren<Text> ();
		panel = this.GetComponent<CanvasGroup> ();
	}

	// Use this for initialization
	void Start () {
		panel.alpha = 1f;
	}

	public void ShowText(string text, float time){
		if (current != null) {
			StopCoroutine (current);
		}

		current = Fade(panel.alpha, 1f, time);
		StartCoroutine (current);

		textbox.text = text;
	}

	//Fades the UI out
	public void FadeOut(float time){
		if (current != null) {
			StopCoroutine (current);
		}
		current = Delay(Fade(panel.alpha, 0f, time), DELAY);
		StartCoroutine (current);
	}

	private IEnumerator Fade(float start, float end, float time){
		yield return new WaitForEndOfFrame ();
		float elapsed = 0;

		panel.alpha = start;
		while (elapsed < time) {
			panel.alpha = Mathf.Lerp (panel.alpha, end, (elapsed / time));
			elapsed += Time.deltaTime;
			yield return new WaitForEndOfFrame ();
		}
	}

	private IEnumerator Delay(IEnumerator routine, float delay){
		yield return new WaitForSeconds (delay);
		current = routine;
		StartCoroutine (current);
	}
}

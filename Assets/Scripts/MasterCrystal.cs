using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MasterCrystal : MonoBehaviour {

	void Start(){
		gameObject.SetActive (false);
	}

	void OnTriggerEnter(Collider other){
		//Go to the next level
		int current = SceneManager.GetActiveScene().buildIndex;
		int total = SceneManager.sceneCountInBuildSettings;

		Debug.Log ("Current: " + current + "  -- Total: " + total);

		if (current + 1 >= total) {
			SceneManager.LoadScene (0);
		} else {
			SceneManager.LoadScene (current + 1);
		}
	}
}

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UILoadScene : MonoBehaviour {

	public GameObject loadingScreen;

	public void LoadScene(int level){

		if (loadingScreen != null) {
			loadingScreen.SetActive (true);
		}
		SceneManager.LoadScene (level);
		Time.timeScale = 1;
	}
}

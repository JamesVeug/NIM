using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UILoadScene : MonoBehaviour {

	public GameObject loadingScreen;

	public void LoadScene(int level){

		loadingScreen.SetActive (true);
		SceneManager.LoadScene (level);

	}
}

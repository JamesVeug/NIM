using UnityEngine;
using System.Collections;

public class UILoadScene : MonoBehaviour {

	public GameObject loadingScreen;

	public void LoadScene(int level){

		loadingScreen.SetActive (true);
		Application.LoadLevel (level);

	}
}

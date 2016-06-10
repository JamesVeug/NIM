using UnityEngine;
using System.Collections;
using UnityEditor;

public class UIShowPanel : MonoBehaviour {

	public void showPanel(GameObject panel){
		panel.SetActive (true);
	}

	public void hidePanel(GameObject panel){
		panel.SetActive (false);
	}

	public void quit(){
		Application.Quit ();
    }
}

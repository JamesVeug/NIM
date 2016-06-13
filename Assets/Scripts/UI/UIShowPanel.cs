using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIShowPanel : MonoBehaviour {
	private static UIShowPanel instance;
    private GameObject activePanel = null;

	void Awake(){
		instance = this;
	}

	public void showPanel(GameObject panel){
		panel.SetActive (true);
        activePanel = panel;
	}

	public void hidePanel(GameObject panel){
		panel.SetActive (false);
		activePanel = null;
    }
    
    public GameObject getActivePanel()
    {
        return activePanel;
    }

	public void quit(){
		Application.Quit ();
    }

	public static UIShowPanel getInstance(){
		if (instance) {
			return instance;
		} else {
			return null;
		}
	}
}

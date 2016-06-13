using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIShowPanel : MonoBehaviour {
	private static UIShowPanel instance;
    private List<GameObject> activePanels = new List<GameObject>();

	void Awake(){
		instance = this;
	}

	public void showPanel(GameObject panel){
		panel.SetActive (true);
        activePanels.Add(panel);
	}

	public void hidePanel(GameObject panel){
		panel.SetActive (false);
        activePanels.Remove(panel);
    }
    
    public List<GameObject> getActivePanels()
    {
        return activePanels;
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

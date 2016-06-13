using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIShowPanel : MonoBehaviour {
    private static List<GameObject> activePanels = new List<GameObject>();

	public void showPanel(GameObject panel){
		panel.SetActive (true);
        activePanels.Add(panel);
	}

	public void hidePanel(GameObject panel){
		panel.SetActive (false);
        activePanels.Remove(panel);
    }
    
    public static List<GameObject> getActivePanels()
    {
        return activePanels;
    }

	public void quit(){
		Application.Quit ();
    }
}

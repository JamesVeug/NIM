using UnityEngine;
using System.Collections;

public class UIMenu : MonoBehaviour {

	private static bool menuOpen = false;

	//Set the menu state
	public void SetMenuOpen(bool open){
		if (menuOpen == open) {
			return;
		}

		//Pause/unpause the gam
		if (open) {
			Time.timeScale = 0;
		} else {
			Time.timeScale = 1;
		}
			
		menuOpen = open;
		gameObject.SetActive(open);
	}

	//Functions for use with buttons
	public void CloseMenu(){
		SetMenuOpen (false);
	}

	public void OpenMenu(){
		SetMenuOpen (true);
	}

	public static bool isOpen(){
		return menuOpen;
	}
}

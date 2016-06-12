using UnityEngine;
using System.Collections;

public class UIMenu : MonoBehaviour {

    public bool pauseGameOnOpen = false;
	private static bool menuOpen = false;

    public void Start()
    {
        menuOpen = true;
    }

	//Set the menu state
	public void SetMenuOpen(bool open){
		if (menuOpen == open) {
			return;
		}

		//Pause/unpause the gam
		if (open && pauseGameOnOpen) {
			Time.timeScale = 0;
		} else if( !open && pauseGameOnOpen){
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

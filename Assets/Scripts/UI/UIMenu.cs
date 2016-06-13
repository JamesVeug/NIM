using UnityEngine;
using System.Collections;

public class UIMenu : MonoBehaviour {

    public bool pauseGameOnOpen = false;
	public bool disableClose = false;
	private bool menuOpen = false;

    public void Start()
    {
        menuOpen = true;
		SetMenuOpen (disableClose);
    }

	//Set the menu state
	public void SetMenuOpen(bool open){
		if (menuOpen == open) {
			return;
		}

		//Pause/unpause the gam
		if (open && pauseGameOnOpen) {
			Time.timeScale = 0f;
		} else if( !open && pauseGameOnOpen){
			Time.timeScale = 1f;
		}
			
		menuOpen = open;
		gameObject.SetActive(open);
	}

	//Functions for use with buttons
	public void CloseMenu(){
		SetMenuOpen (false || disableClose);
	}

	public void OpenMenu(){
		SetMenuOpen (true);
	}

	public bool isOpen(){
		return menuOpen;
	}
}

using UnityEngine;
using System.Collections;

public class UIMenuController : MonoBehaviour {

	public UIMenu menu = null;

	//Work around for timeScale = 0 problems with
	//multiple presses
	private bool toggled = false;

	// Update is called once per frame
	void Update () {
		if (menu == null) {
			return;
		}

		bool buttonDown = Input.GetButtonDown ("Cancel");
        if (buttonDown && !menu.isOpen()) {
			menu.OpenMenu ();
		}
        else if(buttonDown && menu.isOpen())
        {
            menu.CloseMenu();
        }
	}
}

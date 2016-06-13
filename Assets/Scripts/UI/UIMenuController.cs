using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class UIMenuController : MonoBehaviour {

	private UIMenu menu = null;
    public bool menuStartsOpen = false;
    public GameObject subMenuForKeyboard;

    private bool keyboardSubmitCurrentlyPressed = false;
    private bool VerticalArrowCurrentlyPressed = false;
    private int xboxButtonIndex = 0;

    private GameObject startSubMenu;

	void Awake()
	{
		menu = FindObjectOfType<UIMenu> ();
	}
    
    void Start()
    {
        startSubMenu = subMenuForKeyboard;

        // Open menu
        if (menuStartsOpen)
        {
            menu.OpenMenu();
            setInteractable();
        }
    }

	void onEnable(){
	}

    // Update is called once per frame
    void LateUpdate()
    {

        if (menu == null)
        {
            Debug.Log("UIMenuController: No menu assigned");
            return;
        }

        // Open the menu
        bool cancelbutton = Input.GetButtonDown("Cancel");
        bool MainMenuToggleButton = Input.GetButtonDown("MainMenuToggle");
        bool MainMenuToggleButtonXbox = Input.GetButtonDown("MainMenuToggleXbox");
        if ((MainMenuToggleButton|| MainMenuToggleButtonXbox) && !menu.isOpen())
        {
            // Open the menu
            menu.OpenMenu();
            setInteractable();
        }
        else if (cancelbutton && menu.isOpen())
        {
            // Close the menu
            menu.CloseMenu();
            return;
        }

        // Menu not open
        if( !menu.isOpen())
        {
            return;
        }
        
        // Move curser on the Menu Up and Down
        bool verticalPressed = Input.GetButtonDown("Vertical") || (Input.GetAxis("MainMenuXboxControlsVertical") != 0);
        bool verticalReleased = Input.GetButtonUp("Vertical") || (Input.GetAxis("MainMenuXboxControlsVertical") == 0);
        if (verticalPressed && !VerticalArrowCurrentlyPressed)
        {
            float key = Input.GetAxisRaw("Vertical") == 0 ? Input.GetAxis("MainMenuXboxControlsVertical") : Input.GetAxisRaw("Vertical");
            if (key == -1)
            {
                moveDown();
            }
            else
            {
                moveUp();
            }
            setInteractable();
            VerticalArrowCurrentlyPressed = true;
        }
        else if (verticalReleased && VerticalArrowCurrentlyPressed)
        {
            VerticalArrowCurrentlyPressed = false;
        }

        // Move cursor on the menu Left and Right
        bool horizontalPressed = Input.GetButton("Horizontal") || (Input.GetAxis("MainMenuXboxControlsHorizontal") != 0);
        if (horizontalPressed && !VerticalArrowCurrentlyPressed)
        {
            float key = Input.GetAxisRaw("Horizontal") == 0 ? Input.GetAxis("MainMenuXboxControlsHorizontal") : Input.GetAxisRaw("Horizontal");

            Slider slider = getSlider(xboxButtonIndex);
            if( slider != null)
            {
                slider.value = slider.value + 0.01f*key;
            }
        }

        // Accept selection on menu
        if (Input.GetButtonDown("Submit") && !keyboardSubmitCurrentlyPressed)
        {
            invokeButton();
            setInteractable();
            VerticalArrowCurrentlyPressed = true;
        }
        else if (!Input.GetButtonDown("Submit") && VerticalArrowCurrentlyPressed)
        {
            keyboardSubmitCurrentlyPressed = false;
        }
    }

    private void setInteractable()
    {
        Button button = getButton(xboxButtonIndex);
        if (button != null)
        {
			button.Select ();
            return;
        }

        Slider slider = getSlider(xboxButtonIndex);
        if (slider != null)
        {
			slider.Select ();
            return;
        }

		if (xboxButtonIndex != 0) { //If we can't find one, reset the button index and try again
			xboxButtonIndex = 0;
			setInteractable ();
			return;
		}

        Debug.LogError("Could not find selected child " + subMenuForKeyboard.transform.GetChild(xboxButtonIndex).gameObject.name);
    }

    private void invokeButton()
    {
        Button b = getButton(xboxButtonIndex);
		b.onClick.Invoke();

		updatePanel ();
    }

	private bool updatePanel(){
		GameObject active = UIShowPanel.getInstance ().getActivePanel ();

		if (active == null) {
			active = startSubMenu;
		}

		if (active != subMenuForKeyboard) {
			subMenuForKeyboard = active;
			xboxButtonIndex = 0;
			return true;
		}
			

		return false;
	}

    private void moveDown()
    {
		if (!updatePanel ()) {
			xboxButtonIndex++;
		}

        if( xboxButtonIndex >= subMenuForKeyboard.transform.childCount)
        {
            xboxButtonIndex = 0;
        }
    }

    private void moveUp()
    {
		if (!updatePanel ()){
			xboxButtonIndex--;
		}
			
		if (xboxButtonIndex < 0)

        {
            xboxButtonIndex = subMenuForKeyboard.transform.childCount-1;
        }
    }

    private Button getButton(int xboxButtonIndex)
    {
        return subMenuForKeyboard.transform.GetChild(xboxButtonIndex).gameObject.GetComponent<Button>();
    }

    private Slider getSlider(int xboxButtonIndex)
    {
        return subMenuForKeyboard.transform.GetChild(xboxButtonIndex).gameObject.GetComponent<Slider>();
    }
}

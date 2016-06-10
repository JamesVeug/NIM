using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class UIMenuController : MonoBehaviour {

	public UIMenu menu = null;
    public GameObject subMenu;

    private bool keyboardSubmitCurrentlyPressed = false;
    private bool VerticalArrowCurrentlyPressed = false;
    private int xboxButtonIndex = 0;

    private GameObject startSubMenu;

	//Work around for timeScale = 0 problems with
	//multiple presses
	private bool toggled = false;
    
    void Start()
    {
        startSubMenu = subMenu;
    }

    // Update is called once per frame
    void Update()
    {
        if (menu == null)
        {
            return;
        }

        // Open the menu
        bool cancelbutton = Input.GetButtonDown("Cancel");
        bool MainMenuToggleButton = Input.GetButtonDown("MainMenuToggle");
        bool MainMenuToggleButtonXbox = Input.GetButtonDown("MainMenuToggleXbox");
        if ((MainMenuToggleButton|| MainMenuToggleButtonXbox) && !UIMenu.isOpen())
        {
            menu.OpenMenu();
            setInteractable(false);
        }
        else if (cancelbutton && UIMenu.isOpen())
        {
            menu.CloseMenu();
            return;
        }

        // Menu not open
        if( !UIMenu.isOpen())
        {
            return;
        }
        
        // Move curser on the Menu Up and Down
        bool verticalPressed = Input.GetButtonDown("Vertical") || (Input.GetAxis("MainMenuXboxControlsVertical") != 0);
        bool verticalReleased = Input.GetButtonUp("Vertical") || (Input.GetAxis("MainMenuXboxControlsVertical") == 0);
        if (verticalPressed && !VerticalArrowCurrentlyPressed)
        {
            float key = Input.GetAxisRaw("Vertical") == 0 ? Input.GetAxis("MainMenuXboxControlsVertical") : Input.GetAxisRaw("Vertical");
            setInteractable(true);
            if (key == -1)
            {
                moveDown();
            }
            else
            {
                moveUp();
            }
            setInteractable(false);
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
            setInteractable(true);
            invokeButton();
            setInteractable(false);
            VerticalArrowCurrentlyPressed = true;
        }
        else if (!Input.GetButtonDown("Submit") && VerticalArrowCurrentlyPressed)
        {
            keyboardSubmitCurrentlyPressed = false;
        }
    }

    private void setInteractable(bool state)
    {
        Button button = getButton(xboxButtonIndex);
        if (button != null)
        {
            button.interactable = state;
            return;
        }

        Slider slider = getSlider(xboxButtonIndex);
        if (slider != null)
        {
            slider.interactable = state;
            return;
        }

        Debug.LogError("Could not find selected child " + subMenu.transform.GetChild(xboxButtonIndex).gameObject.name);
    }

    private void invokeButton()
    {
        Button b = getButton(xboxButtonIndex);
        b.onClick.Invoke();
        xboxButtonIndex = 0;

        List<GameObject> panels = UIShowPanel.getActivePanels();
        if (panels.Count > 0) {
            subMenu = panels[0];
        }
        else
        {
            subMenu = startSubMenu;
        }
    }

    private void moveDown()
    {
        xboxButtonIndex++;
        if( xboxButtonIndex >= subMenu.transform.childCount)
        {
            xboxButtonIndex = 0;
        }
    }

    private void moveUp()
    {
        xboxButtonIndex--;
        if (xboxButtonIndex < 0)
        {
            xboxButtonIndex = subMenu.transform.childCount-1;
        }
    }

    private Button getButton(int xboxButtonIndex)
    {
        return subMenu.transform.GetChild(xboxButtonIndex).gameObject.GetComponent<Button>();
    }

    private Slider getSlider(int xboxButtonIndex)
    {
        return subMenu.transform.GetChild(xboxButtonIndex).gameObject.GetComponent<Slider>();
    }
}

using UnityEngine;
using System.Collections;

public class MainMenuPanel : MonoBehaviour {
    public GameObject MainMenu;
    public static bool isOpen = false;

	// Use this for initialization
	void Start () {
        if (MainMenu == null)
        {
            Debug.Log("No MainMenu object asssigned to MainMenuPanel script for " + gameObject.name);
        }
        else
        {
            MainMenu.gameObject.SetActive(isOpen);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if( MainMenu == null)
        {
            Debug.Log("No MainMenu object asssigned to MainMenuPanel script for " + gameObject.name);
        }
        else if( Input.GetButtonDown("MainMenuToggle") && !MainMenu.gameObject.activeSelf)
        {
            MainMenu.gameObject.SetActive(true);
            isOpen = true;
        }
        else if(Input.GetButtonDown("MainMenuToggle") && MainMenu.gameObject.activeSelf)
        {
            MainMenu.gameObject.SetActive(false);
            isOpen = false;
        }
	}
}

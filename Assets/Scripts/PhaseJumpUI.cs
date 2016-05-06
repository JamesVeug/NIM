using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PhaseJumpUI : MonoBehaviour {
    PhaseJump jump;

    public Sprite canPhaseLeft;
    public Sprite canPhaseRight;
    public Sprite canNotPhaseLeft;
    public Sprite canNotPhaseRight;
    public Sprite selectPhaseLeft;
    public Sprite selectPhaseRight;
    public Sprite missing;
    public Sprite menu;

    public GameObject previewObject;
    public Camera previewCamera;

    // Use this for initialization
    void Start () {
        jump = GetComponent<PhaseJump>();

    }
	
	// Update is called once per frame
	void Update () {
	    if( jump == null)
        {
            Debug.LogError("NO PHASE JUMP SCRIPT APPLIED TO OBJECT!");
            return;
        }

        bool menuOpen = jump.phaseMenuIsOpen();
        
        Sprite menuImage = !menuOpen ? missing : menu;
        Image menuGuiImage = GameObject.Find("PhaseImageMenu").GetComponent<Image>();
        menuGuiImage.sprite = menuImage;

        // Which direction have we selected from the menu?
        int direction = jump.getJumpDirection();

        bool canJumpForward = jump.canPhaseForward();
        Sprite nextImage = !menuOpen ? missing : canJumpForward ? (direction == 1 ? selectPhaseRight : canPhaseRight) : canNotPhaseRight;
        Image nextGuiImage = GameObject.Find("PhaseImageNext").GetComponent<Image>();
        nextGuiImage.sprite = nextImage;

        bool canJumpBackwards = jump.canPhaseBack();
        Sprite previousImage = !menuOpen ? missing : canJumpBackwards ? (direction == -1 ? selectPhaseLeft : canPhaseLeft) : canNotPhaseLeft;
        Image previousGuiImage = GameObject.Find("PhaseImagePrevious").GetComponent<Image>();
        previousGuiImage.sprite = previousImage;

        // Preview the area
        if ((direction == 1 && jump.canPhaseForward()) || (direction == -1 && jump.canPhaseBack()))
        {

            // Move the preview object
            Vector3 spawnPosition = Vector3.zero;
            MovementWaypoint newPhasePoint = null; 
            jump.getPhasePoint(direction == 1, out spawnPosition, out newPhasePoint);
            //jump.previewPhaseCamera(previewCamera, previewObject, spawnPosition, direction == 1);


            previewObject.transform.position = spawnPosition;
            previewCamera.depth = 1;
        }
        else
        {
            previewCamera.depth = -1;

            Renderer r = previewObject.GetComponent<Renderer>();
            hideRenderers();
        }
    }

    private void hideRenderers()
    {
        previewObject.transform.position = new Vector3(0,-2000,0);
    }
}

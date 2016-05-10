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

    // SOUNDS
    public AudioClip[] openMenuSound;
    public AudioClip[] closeMenuSound;
    public AudioClip[] selectForwardArrowSound;
    public AudioClip[] selectBackwardArrowSound;

    private bool menuIsOpen = false;
    private bool forwardArrowSelected = false;
    private bool backwardArrowSelected = false;

    // Use this for initialization
    void Start () {
        jump = GetComponent<PhaseJump>();

    }
	
	// Update is called once per frame
	void Update () {
	    if( jump == null)
        {
            Debug.LogError("No PhaseJump script given.");
            return;
        }
        GameObject phaseMenuImageO = GameObject.Find("PhaseImageMenu");
        GameObject phaseNextImageO = GameObject.Find("PhaseImageNext");
        GameObject phasePrevImageO = GameObject.Find("PhaseImagePrevious");
        if( phaseMenuImageO == null) { Debug.LogError("PhaseImageMenu not setup in Canvas. Can not show menu!"); return; }
        if (phaseNextImageO == null) { Debug.LogError("PhaseImageNext not setup in Canvas. Can not show menu!"); return; }
        if (phasePrevImageO == null) { Debug.LogError("PhaseImagePrevious not setup in Canvas. Can not show menu!"); return; }

        bool menuOpen = jump.phaseMenuIsOpen();
        if (!menuIsOpen && menuOpen == true)
        {
            playRandomSound(openMenuSound);
        }
        else if (menuIsOpen && menuOpen == false)
        {
            playRandomSound(closeMenuSound);
        }
        menuIsOpen = menuOpen;

        Sprite menuImage = !menuOpen ? missing : menu;
        Image menuGuiImage = phaseMenuImageO.GetComponent<Image>();
        menuGuiImage.sprite = menuImage;

        // Which direction have we selected from the menu?
        int direction = jump.getJumpDirection();

        // Phase Forward Button
        bool canJumpForward = jump.canPhaseForward();
        Sprite nextImage = !menuOpen ? missing : canJumpForward ? (direction == 1 ? selectPhaseRight : canPhaseRight) : canNotPhaseRight;
        Image nextGuiImage = phaseNextImageO.GetComponent<Image>();
        nextGuiImage.sprite = nextImage;
        if (!forwardArrowSelected && direction == 1)
        {
            playRandomSound(selectForwardArrowSound);
        }
        forwardArrowSelected = direction == 1;

        // Phase Back button
        bool canJumpBackwards = jump.canPhaseBack();
        Sprite previousImage = !menuOpen ? missing : canJumpBackwards ? (direction == -1 ? selectPhaseLeft : canPhaseLeft) : canNotPhaseLeft;
        Image previousGuiImage = phasePrevImageO.GetComponent<Image>();
        previousGuiImage.sprite = previousImage;
        if (!backwardArrowSelected && direction == -1)
        {
            playRandomSound(selectBackwardArrowSound);
        }
        backwardArrowSelected = direction == -1;

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

    // Play a random sound from a given array of sound files
    private void playRandomSound(AudioClip[] clips)
    {
        // Don't try playing a sound if there aren't any
        if (clips == null || clips.Length <= 0)
        {
            return;
        }

        // Check that we have an Audio Source Component
        AudioSource a = GetComponent<AudioSource>();
        if (a == null)
        {
            Debug.LogWarning("Player does not have an Audio Source component");
            return;
        }

        // Play random sound
        int index = Random.Range(0, clips.Length);
        a.PlayOneShot(clips[index]);
    }
}

using UnityEngine;
using System.Collections;

public class PhaseCondition : MonoBehaviour {


    // Conditions to do after Phase
    public bool effectAfterPhase = true;
    public bool effectOnForwardPhase = true;

    // Modify camera
    public bool enableModifyCamera = false;
    public Camera cameraToEdit;
    public bool cameraChaseX = true;
    public bool cameraChaseY = true;
    public bool cameraChaseZ = true;
    public bool camLookAtPlayer = true;
    public bool camChasePlayer = true;
    public float camChaseSpeed = 10f;
    public float cameraDistance = 10f;
    public Vector3 cameraRotation = Vector3.zero;

    // Move the player
    public bool enablePlayerModify = false;
    public bool playerMoveCameraOnPhase = true;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PhaseJump move = other.gameObject.GetComponent<PhaseJump>();
            if (move == null)
            {
                Debug.LogWarning("Player does not have PhaseJump script!");
                return;
            }
            move.addPhaseCondition(this);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PhaseJump move = other.gameObject.GetComponent<PhaseJump>();
            if (move == null)
            {
                Debug.LogWarning("Player does not have PhaseJump script!");
                return;
            }
            move.removePhaseCondition(this);
        }
    }

    // Trigger this before we phase
    public void triggerBeforePhase(bool phaseForward)
    {
        if (effectAfterPhase && phaseForward == effectOnForwardPhase)
        {
            ChangeOptions();
        }
    }

    // Trigger AFTER we phased
    public void triggerAfterPhase(bool phaseForward)
    {
        if (!effectAfterPhase && phaseForward == effectOnForwardPhase)
        {
            ChangeOptions();
        }
    }

    public void ChangeOptions()
    {
        // Modify the main camera if we need to
        if (enableModifyCamera)
        {
            ChasePlayer cam = cameraToEdit.GetComponent<ChasePlayer>();
            cam.chaseX = cameraChaseX;
            cam.chaseY = cameraChaseY;
            cam.chaseZ = cameraChaseZ;
            cam.rotationVector = cameraRotation;
            cam.distance = cameraDistance;
            cam.lookAtPlayer = camLookAtPlayer;
            cam.chaseSpeed = camChaseSpeed;
            cam.enableChase = camChasePlayer;
        }

        // Modify the player if we need to
        if (enablePlayerModify)
        {
            PhaseJump player = GameObject.FindGameObjectWithTag("Player").GetComponent<PhaseJump>();
            player.moveCameraOnPhase = playerMoveCameraOnPhase;
        }
    }
}

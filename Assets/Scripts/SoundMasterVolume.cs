using UnityEngine;
using System.Collections;

public class SoundMasterVolume : MonoBehaviour {

    // What to do when we move in and out of the thing
    public float MasterVolume_OnEnter = 25;
    public float MasterVolume_OnExit = 0;

    void OnTriggerEnter(Collider other)
    {
        SoundMaster.setMasterVolume(MasterVolume_OnEnter);
    }

    void OnTriggerExit(Collider other)
    {
        SoundMaster.setMasterVolume(MasterVolume_OnExit);
    }
}

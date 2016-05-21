using UnityEngine;
using System.Collections;
using XInputDotNetPure;

public class ShakeCamera : MonoBehaviour
{
    private bool Shaking;
    private float CurrentShakeDecay;
    private float CurrentShakeIntensity;


    public float ShakeIntensity = 0.085f;
    public float ShakeDecay = 0.005f;
    public float XboxVibrateIntensity = 0.5f;

    private Vector3 OriginalPos;
    private Quaternion OriginalRot;

    // Use this for initialization
    void Start () {
	
	}

    /*void OnGUI()
    {
        // Creates a button on the screen when in-game
        if (GUI.Button(new Rect(10, 200, 100, 30), "Shake Camera"))
            DoShake();

    }*/

    void Update()
    {
        if (CurrentShakeIntensity > 0)
        {
            //Debug.Log("Shaking");
            transform.position += Random.insideUnitSphere * CurrentShakeIntensity;
            transform.rotation *= new Quaternion(Random.Range(-CurrentShakeIntensity, CurrentShakeIntensity) * .2f,
                                            Random.Range(-CurrentShakeIntensity, CurrentShakeIntensity) * .2f,
                                            Random.Range(-CurrentShakeIntensity, CurrentShakeIntensity) * .2f,
                                            Random.Range(-CurrentShakeIntensity, CurrentShakeIntensity) * .2f);

            CurrentShakeIntensity -= CurrentShakeDecay;
        }
        else if (Shaking)
        {
            Shaking = false;
        }

        // Vibrate controller
        float vibrationIntensity = (ShakeIntensity / CurrentShakeIntensity)* XboxVibrateIntensity;
        GamePad.SetVibration(PlayerIndex.One, vibrationIntensity, vibrationIntensity);

        // Use triggers to toggle vibration
        //GamePadState state = GamePad.GetState(PlayerIndex.One);
        //GamePad.SetVibration(PlayerIndex.One, state.Triggers.Left, state.Triggers.Right);
    }

    public void DoShake()
    {
        OriginalPos = transform.position;
        OriginalRot = transform.rotation;

        CurrentShakeIntensity = ShakeIntensity;
        CurrentShakeDecay = ShakeDecay;
        Shaking = true;
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShardUI : MonoBehaviour
{
    public int totalPickups = 0;
    public Text pickupCounterText;

    //private static float newScale = 0f; // Change size to this
    public static float maxScaleSpeed = 0.25f;
    public static float scaleSpeed = 0.1f;
    public static float speedDecay = 0.002f;

    private Scrollbar bar;
    private bool alertedCompletion = false;
    private static float currentSpeed = 0;
    private static int pickups = 0;

    // We picked up a shard
    public static void pickupShard()
    {
        pickups++;
        currentSpeed = Mathf.Min(maxScaleSpeed, currentSpeed + scaleSpeed);
    }

	// Use this for initialization
	void Start () {
        bar = GetComponent<Scrollbar>();
        bar.size = 0;
        currentSpeed = scaleSpeed;
    }

    void Update()
    {
        if( bar == null)
        {
            Debug.LogError("No scroll bar has been added to GameObject '" + gameObject.name + "'");
            return;
        }

        // Change the scroll bar
        float pickupRatio = ((float)pickups / (float)totalPickups);
        if (pickupRatio > bar.size)
        {
            bar.size = Mathf.Min(bar.size + currentSpeed * Time.deltaTime, pickupRatio);
        }
        else
        {
            bar.size = Mathf.Max(bar.size - currentSpeed * Time.deltaTime, pickupRatio);
        }

        // Update pickup Counter
        pickupCounterText.text = pickups.ToString() + "/" + totalPickups.ToString();

        // Decay the speed so it doesn't stay so fast
        if ( bar.size == pickupRatio)
        {
            //Debug.Log("Fast");
            // Decay faster if we have reached our target
            currentSpeed = Mathf.Max(currentSpeed - speedDecay*2, scaleSpeed);
        }
        else
        {
            // Slow down the update rate
            currentSpeed = Mathf.Max(currentSpeed - speedDecay, scaleSpeed);
        }
        //Debug.Log("Speed " + currentSpeed);

        // Check if we have completed the bar
        if( bar.size >= 1 && !alertedCompletion)
        {
            completedBar();
        }
    }

    // We have collected all the shards
    public void completedBar()
    {
        Debug.Log("Picked up all Shards! =D");

        // Don't call this method again
        alertedCompletion = true;
    }
}

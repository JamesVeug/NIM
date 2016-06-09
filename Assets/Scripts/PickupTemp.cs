using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PickupTemp : MonoBehaviour {
    private static int totalPickups = 0;
    private static int pickups = 0;
    private bool addedThisPickup = false; // TEMP. Checks if we have added this crystal to the total pickups

    private Text pickupCounterText;

    private float y0;
    private float amplitude = 1;
    private float speed = 1;
    private float rotateSpeed = -25.0f;

    public Material[] materials;
    public Mesh[] meshes;

    [Range(min: 0, max: 100)]
    public float[] picksupSoundsVolumes;
    public AudioClip[] pickupSounds;

	// Use this for initialization
	void Start () {
        y0 = transform.position.y;
        pickups = 0;
        totalPickups = 0;
        //transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        amplitude = Random.Range(0.15f, 0.25f);
        speed = Random.Range(0.8f, 1.2f);
        rotateSpeed = Random.Range(20f, 30f);

        if (meshes.Length > 0)
        {
            int meshI = Random.Range(0, meshes.Length);
            MeshFilter rend = GetComponent<MeshFilter>();
            rend.mesh = meshes[meshI];
        }

        if (materials.Length > 0)
        {
            int materialsI = Random.Range(0, materials.Length);
            Renderer renderer = GetComponent<Renderer>();
            renderer.material = materials[materialsI];
        }

        GameObject counter = GameObject.Find("PickupCounter");
        if (counter != null)
        {
            pickupCounterText = counter.GetComponent<Text>();
            if (pickupCounterText != null)
            {
                pickupCounterText.text = pickups.ToString() + "/" + totalPickups.ToString();
            }
        }

        
    }

    void updatePickupCounter()
    {
        if (pickupCounterText != null)
        {
            pickupCounterText.text = pickups.ToString() + "/" + totalPickups.ToString();
        }
    }
	
	// Update is called once per frame
	void Update () {
        // Save the y position prior to start floating (maybe in the Start function):
        if (!addedThisPickup)
        {
            addedThisPickup = true;
            totalPickups++;
            updatePickupCounter();
        }

        // Put the floating movement in the Update function:
        Vector3 p = transform.position;
        p.y = y0 + amplitude * Mathf.Sin(speed * Time.time);
        transform.position = p;

        transform.Rotate(transform.up * rotateSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if( !other.gameObject.tag.Equals("Player"))
        {
            // Only be picked up by the player
            return;
        }

        pickups++;

        updatePickupCounter();

        AudioSource source = other.gameObject.GetComponent<AudioSource>();
        if (source != null) { 
            SoundMaster.playRandomSound(pickupSounds, picksupSoundsVolumes, source);
        }
        Destroy(gameObject);
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PickupTemp : MonoBehaviour {

    private float y0;
    private float amplitude = 1;
    private float speed = 1;
    private float rotateSpeed = -25.0f;

    public Material[] materials;
    public Mesh[] meshes;
    
    public float[] picksupSoundsVolumes;
    public AudioClip[] pickupSounds;

	// Use this for initialization
	void Start () {
        y0 = transform.position.y;
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
    }
	
	// Update is called once per frame
	void Update () {
        // Save the y position prior to start floating (maybe in the Start function):


        // Put the floating movement in the Update function:

        Vector3 p = transform.position;
        p.y = y0 + amplitude * Mathf.Sin(speed * Time.time);
        transform.position = p;

        transform.Rotate(transform.up * rotateSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject phaseMenuImageO = GameObject.Find("PickupCounter");
        if (phaseMenuImageO != null)
        {
            Text menuGuiImage = phaseMenuImageO.GetComponent<Text>();
            if (menuGuiImage != null)
            {
                int num = int.Parse(menuGuiImage.text);
                num++;
                menuGuiImage.text = "" + num;
            }
            else
            {
                Debug.Log("2");
            }
        }
        else
        {
            Debug.Log("3");
        }

        AudioSource source = other.gameObject.GetComponent<AudioSource>();
        if (source != null) { 
            SoundMaster.playRandomSound(pickupSounds, picksupSoundsVolumes, source);
        }
        Destroy(gameObject);
    }

    void OnTriggerExit(Collider other)
    {
    }
}

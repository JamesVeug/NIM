using UnityEngine;
using System.Collections;

public class RandomizeMaterialOnStart : MonoBehaviour {

    public Material[] materials;
    // Use this for initialization
    void Start () {
        if (materials.Length > 0)
        {
            int materialsI = Random.Range(0, materials.Length);
            Renderer renderer = GetComponent<Renderer>();
            renderer.material = materials[materialsI];
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}

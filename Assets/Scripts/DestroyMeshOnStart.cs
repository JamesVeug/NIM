using UnityEngine;
using System.Collections;

public class DestroyMeshOnStart : MonoBehaviour {

    // Use this for initialization
    void Start()
    {

        // Destroys mesh renderer
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        Destroy(mesh);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

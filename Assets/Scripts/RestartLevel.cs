using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class RestartLevel : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        // Object needs to have the tag Player to activate this!
        if( other.gameObject.tag ==  "Player")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void OnTriggerExit(Collider other)
    {
    }
}

using UnityEngine;
using System.Collections;

public class UILoadScene : MonoBehaviour {

	public void LoadScene(int level){
		
		Application.LoadLevel (level);

	}
}

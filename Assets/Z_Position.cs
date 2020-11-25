using UnityEngine;
using System.Collections;

public class Z_Position : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
			//Creates a variable to check the objects position.
		Debug.Log (GameObject.Find("Cube").transform.position.z);
	}
}

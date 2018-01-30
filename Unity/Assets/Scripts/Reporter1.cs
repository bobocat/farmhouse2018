using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reporter1 : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        Debug.Log("rot: " + transform.localEulerAngles);
		
	}
}


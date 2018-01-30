using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

     public void MoveUp()
    {
        transform.Translate(new Vector3(0f, .2f, 0f));
    }

    public void MoveDown()
    {
        transform.Translate(new Vector3(0f, -.2f, 0f));
    }


}

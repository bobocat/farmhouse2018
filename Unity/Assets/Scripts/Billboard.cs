using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {

    public Transform target;

    GameObject tempLookat;

	// Use this for initialization
	void Start () {

//        tempLookat.transform.position = transform.position;
//        tempLookat = new GameObject();

//        target = VRTK.VRTK_DeviceFinder.HeadsetTransform();
    }

	
	// Update is called once per frame
	void Update () {

        transform.LookAt(target.position);

        //        tempLookat.transform.LookAt(target);
        //        transform.eulerAngles = new Vector3(transform.eulerAngles.x, tempLookat.transform.eulerAngles.y, transform.eulerAngles.z);

    }

}

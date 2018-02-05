using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour {

    [HideInInspector]
    public enum followTypeType {allRotation, Yonly, posOnly };

    public followTypeType followType;

    private Transform headset;

	// Use this for initialization
	void Start () {


        headset = FindObjectOfType<HeadsetGrabber>().transform;
		
	}

	
	// Update is called once per frame
	void Update () {

        try
        {
            ///            Vector3 headRot = VRTK.VRTK_DeviceFinder.HeadsetTransform().localEulerAngles;

            transform.position = headset.position;

            if (followType == followTypeType.Yonly)
            {
                transform.localEulerAngles = new Vector3(0f, headset.localEulerAngles.y, 0f);
            }
            else if (followType == followTypeType.allRotation)
            {
                transform.rotation = headset.rotation;
            }


        }
        catch { }


    }

}

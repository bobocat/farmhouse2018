using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour {

    [HideInInspector]
    public enum followTypeType {allRotation, Yonly, posOnly };

    public followTypeType followType;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        try
        {
///            Vector3 headRot = VRTK.VRTK_DeviceFinder.HeadsetTransform().localEulerAngles;

///            transform.position = VRTK.VRTK_DeviceFinder.HeadsetTransform().position;

            if (followType == followTypeType.Yonly)
            {
///                transform.localEulerAngles = new Vector3(0f, headRot.y, 0f);
            }
            else if (followType == followTypeType.allRotation)
            {
///                transform.rotation = VRTK.VRTK_DeviceFinder.HeadsetTransform().rotation;
            }


        }
        catch { }


    }

}

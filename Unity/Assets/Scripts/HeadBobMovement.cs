using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobMovement : MonoBehaviour {

    public float timeDiff = .2f;    // how far back should we go to look for a change in head position
    private float nextTimeCheck;    // the next time we will be checking the headset position

    public float distanceThreshold = .1f;   // how far does the headset need to move in Y for us to consider this a movement signal

    private Vector3 lastPosition;
    private Vector3 currentPosition;

	// Use this for initialization
	void Start () {

///        lastPosition = VRTK.VRTK_DeviceFinder.HeadsetTransform().position;
        nextTimeCheck = Time.timeSinceLevelLoad + timeDiff;

    }
	
	// Update is called once per frame
	void Update () {

        // is it time to check for a new position?
        if (Time.timeSinceLevelLoad >= nextTimeCheck)
        {
            // get the current position of the headset
///            currentPosition = VRTK.VRTK_DeviceFinder.HeadsetTransform().position;

            if (Mathf.Abs(currentPosition.y - lastPosition.y) > distanceThreshold)
            {
                MoveForward();
            }

            nextTimeCheck = Time.timeSinceLevelLoad + timeDiff;
        }

    }


    void MoveForward()
    {

    }

}

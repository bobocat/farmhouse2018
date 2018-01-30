using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    public float targetY;
    public Transform player;
    public Transform diffNeeded;
    public Transform newPos;
    public Transform desired;


    // Use this for initialization
    void Start () {

        Debug.Log("test"+gameObject.name);
        
	}

	
	// Update is called once per frame
	void Update () {

        QuaternianTest();
		
	}

/*
    private Quaternion SetOffsetQuaternian(Quaternion playerQ, Quaternion targetQ)
    {
        Quaternion offsetQ;

        // this gets the difference between the playerQ and the targetQ
        Quaternion diffNeededQ = Quaternion.Inverse(playerQ) * targetQ;

        diffNeeded.rotation = diffNeededY;

        // now the newPos object will be rotated to the player + diffNeeded object

        newPos.rotation = player.rotation * diffNeeded.rotation;

        return offsetQ;

    }
*/

    private void QuaternianTest()
    {

        // where I want to go is player - desired = diff needed = what the dest angle should be
        //        float diffNeededY = desired.localEulerAngles.y - player.localEulerAngles.y;
        Quaternion diffNeededY = Quaternion.Inverse(player.rotation) * desired.rotation;
        

        // now I need to do is record the diff needed onto the diffNeeded object
        diffNeeded.rotation = diffNeededY;

        // now the newPos object will be rotated to the player + diffNeeded object

        newPos.rotation = player.rotation * diffNeeded.rotation;

//        newPos.localEulerAngles = new Vector3(0f, (player.localEulerAngles.y + diffNeeded.localEulerAngles.y), 0f);

    }

    private void EulerTest()
    {
        // where I want to go is player - desired = diff needed = what the dest angle should be
        float diffNeededY = desired.localEulerAngles.y - player.localEulerAngles.y;

        // now I need to do is record the diff needed onto the diffNeeded object
        diffNeeded.localEulerAngles = new Vector3(0f, diffNeededY, 0f);

        // now the newPos object will be rotated to the player + diffNeeded object

        newPos.localEulerAngles = new Vector3(0f, (player.localEulerAngles.y + diffNeeded.localEulerAngles.y), 0f);

        //        float newY = currY.rotation.y + diff;

    }

    public void TextOutput()
    {

        Debug.Log("test output is working");
    }
}

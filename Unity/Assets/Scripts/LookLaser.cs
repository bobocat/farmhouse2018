using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookLaser : MonoBehaviour {

    public bool useLookat = false;

    private float distanceToSensor;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }

    private void FixedUpdate()
    {
        if (useLookat)
        {
            Vector3 fwd = transform.TransformDirection(Vector3.forward);

            RaycastHit hit;

            if (Physics.Raycast(transform.position, fwd, out hit))
            {
//                print("hit " + hit.collider.name + " at " + hit.distance);

                if (hit.collider.GetComponent<Sensor>() != null)
                {
                    // figure out how far we are from the sensor
                    distanceToSensor = Vector3.Distance(transform.position, hit.transform.position);

                    hit.collider.GetComponent<Sensor>().OnLookedAt(distanceToSensor);
                }

            }

            //            if (Physics.Raycast(transform.position, -Vector3.up, out hit))
            //        if (Physics.Raycast(transform.position, fwd, 10))
            //            print("There is something in front of the object!");

        }

    }


}

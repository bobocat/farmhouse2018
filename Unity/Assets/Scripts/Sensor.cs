using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sensor : MonoBehaviour {

    public int triggerCount = 0;

    public UnityEvent touched;
    public UnityEvent triggered;

    public bool useLookedAt = false;
    public float lookAtSeconds = .5f;       // how long the player has to look at this sensor for it to activate
    private float lookAtStart = 0f;         // when did they start looking at this sensor
    private bool beingLookedAt = false;     // are we being looked at?
    private float lookAtTimePassed = 0f;    // how long have we been looked at?
    public float timeBetweenLooks = 3f;     // how long after a look has finished can we register another look?
    private float timeOfLastLook = 0f;      // when was our last look completed?

    private float lookAtDistance = 3f;      // you have to be this close to activate the sensor

    public UnityEvent lookedAt;

    public UnityEvent grabbed;
    public UnityEvent clicked;

    // Use this for initialization
    void Start () {

        timeOfLastLook = Time.timeSinceLevelLoad;

    }
	
	// Update is called once per frame
	void Update () {


    }


    void FixedUpdate()
    {

        if (useLookedAt)
        {
            if (lookAtTimePassed > lookAtSeconds)
            {
                Debug.Log(gameObject.name + "  has been lookAtTriggered!");
                lookedAt.Invoke();
                lookAtTimePassed = 0;
                timeOfLastLook = Time.timeSinceLevelLoad;
                triggerCount++;
            }
            else if (lookAtTimePassed > 0f)
            {
                lookAtTimePassed -= Time.deltaTime * .15f;
            }
        }

    }

    // this gets trigger by the laser that is attached to the player!
    public void OnLookedAt(float distanceToPlayer)
    {
//        Debug.Log("dist: " + distanceToPlayer);

        // we can only register a look if there has been a delay since the last look
        if (Time.timeSinceLevelLoad > (timeOfLastLook + timeBetweenLooks))
        {
            // only activate within the lookAtDistance
            if (distanceToPlayer < lookAtDistance)
            {
                lookAtTimePassed += Time.deltaTime;
            }
            //        Debug.Log("someone is looking at us! " + lookAtTimePassed);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        // screen this so it only reacts to the body entering the trigger
        if (other.gameObject.name.Contains("Body"))
        {
//            Debug.Log(gameObject.name + "  triggered " + other.gameObject.name);
            triggered.Invoke();
            triggerCount++;
        }
        else if (other.gameObject.name.Contains("Index"))
        {
//            Debug.Log(gameObject.name + "  touched "+other.gameObject.name);
            touched.Invoke();
            triggerCount++;
        }
        else
        {
//            Debug.Log(gameObject.name + "  touched " + other.gameObject.name);
        }


    }

}

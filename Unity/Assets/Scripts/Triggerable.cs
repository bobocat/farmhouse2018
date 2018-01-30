using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Triggerable : MonoBehaviour {

    public UnityEvent onActivated;          // this gets fired when we hit our trigger threshold

    [Tooltip("when we have been triggered this many times then we fire our OnActivated")]
    public int triggersToActivate = 1;      // when we have been triggered this many times then we fire our OnActivated
    public int triggerCount = 0;            // track how many times we have been triggered

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ResetTriggers()
    {
        triggerCount = 0;
    }

    public void OnTriggered()
    {
        triggerCount++;

        if (triggerCount >= triggersToActivate)
        {
            onActivated.Invoke();
            ResetTriggers();
        }
    }
}

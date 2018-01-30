using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class TextTrigger : MonoBehaviour {

    public UnityEvent triggerEnter;

    public Text textToChange;       // this is text on one of the balloons

    public string[] textArray;      // a list of text strings that we will cycle through

    private int index;              // the next string to show

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        triggerEnter.Invoke();
    }
}

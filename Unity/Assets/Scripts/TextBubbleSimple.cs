using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Collections;
using UnityEngine.UI;


public class TextBubbleSimple : MonoBehaviour {

    private GameManager gameManager;

    public Text textField;          // where to write the test message
    public GameObject bubble;       // the bubble we will turn off and on

    private float timeUntilHide;            // this counts down to let us know when to hide the bubble again

    // Use this for initialization
    void Start () {

        try { gameManager = FindObjectOfType<GameManager>(); }
            catch { Debug.Log("missing gameManager in scene"); }

        HideBubble();

	}

	
	// Update is called once per frame
	void Update () {

        if (timeUntilHide > 0)
        {
            timeUntilHide -= Time.deltaTime;

            if (timeUntilHide <= 0)
            {
                HideBubble();
            }
        }
		
	}


    void ShowBubble()
    {
        bubble.SetActive(true);
    }

    void HideBubble()
    {
        bubble.SetActive(false);
    }


    public void ShowMessage(string text, float delay)
    {

//        Debug.Log(text);
        textField.text = text.Replace("|","\n");

        ShowBubble();

        timeUntilHide = delay;

    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Collections;
using UnityEngine.UI;

public class Reporter : MonoBehaviour {

    public Text textField;
    private string text;

    //private List<Transform> GOList = new List<Transform>();
    private GameObject[] GOList;

    int offset = 0;

	// Use this for initialization
	void Start () {

        // get a list of all gameobjects in the scene

        GOList = FindObjectsOfType<GameObject>();

        text = "";

        foreach (GameObject t in GOList)
        {
            if (t.name.Contains("body"))

            text += t.gameObject.name + " : " + t.transform.position.ToString() + "\n";
        }

        Debug.Log(text);

        //VRTK.VRTK_SDKObjectAlias.SDKObject.Headset

///        Debug.Log("headset: " + VRTK.VRTK_DeviceFinder.HeadsetTransform());

        

    }

    // Update is called once per frame
    void Update () {

        if (Input.GetKeyDown(KeyCode.Space)) offset++;

        if (offset >= GOList.Length) offset = GOList.Length;

        try
        {
///            text = "headset: " + VRTK.VRTK_DeviceFinder.HeadsetTransform().position;
        }
        catch { }

        textField.text = text;
	}

}

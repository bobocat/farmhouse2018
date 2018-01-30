using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class EventGroup {

    public string ID;
    public TextMessage textMessage;
    public AudioClip sound;
    public string animState;
    public bool complete = false;
    public string gmCondition;

}

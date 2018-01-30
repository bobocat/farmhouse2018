using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Collections;

[System.Serializable]
public class Chapter {

    public string name;
    public bool complete = false;
    public List<Condition> conditions = new List<Condition>();

}

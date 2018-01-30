using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{

    public UnityEvent onTriggerEnter;          // this gets fired when we hit our trigger threshold

    public void OnTriggerEnter()
    {
        onTriggerEnter.Invoke();
    }
}

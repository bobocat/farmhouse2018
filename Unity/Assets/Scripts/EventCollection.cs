using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Collections;

//[System.Serializable]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
public class EventCollection : MonoBehaviour{

    public List<EventGroup> eventGroupList = new List<EventGroup>();

    private GameManager gameManager;

    public TextBubbleSimple textBubble;
    public Animator anim;
    public AudioSource audioSource;

    public bool loop = false;           // should we reset all the completes and start again?

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();

    }

    // reset all the complete triggers
    public void Reset()
    {
        foreach (EventGroup eventGroup in eventGroupList)
        {
            eventGroup.complete = false;
        }
    }


    private bool CheckAllComplete()
    {
        bool allComplete = true;

        foreach (EventGroup eventGroup in eventGroupList)
        {
            if (eventGroup.complete == false)
            {
                allComplete = false;
            }
        }

        Debug.Log("checking complete: " + allComplete);

        return allComplete;

    }

        public void TriggerEvent()
    {

        // go through the events in the list
        foreach (EventGroup eventGroup in eventGroupList)
        {
            // find the first event that is incomplete and do the stuff in it
            if (eventGroup.complete == false)
            {
                // do the stuff in the event
                if (eventGroup.textMessage.text != "")
                {
                    textBubble.ShowMessage(eventGroup.textMessage.text, eventGroup.textMessage.delay);
                }

                // mark the event as complete
                eventGroup.complete = true;

                Debug.Log("event collection event complete");

                // update the condition in the gameManager
                gameManager.CompleteCondition(eventGroup.gmCondition);

                break;
            }

        }

        if (loop && (CheckAllComplete() == true)) Reset();

    }

}

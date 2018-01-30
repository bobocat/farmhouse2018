using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Collections;
using UnityEngine.UI;

[System.Serializable]
public class TextMessage
{
    public string text;        // what the message says
    public float delay = 3f;	    // time to stay on screen
}

[System.Serializable]
public class TextMessageGroup
{
    public string chapter;                                             // should match the name in gameManager
    public List<TextMessage> textMessageList = new List<TextMessage>();   // collection 
}

public class TextBubble : MonoBehaviour {

    private GameManager gameManager;

    public List<TextMessageGroup> messageGroupList = new List<TextMessageGroup>();
    public Text textField;          // where to write the test message
    public GameObject bubble;       // the bubble we will turn off and on

    private int messageIndex = 0;
    private int messagesInCurrentChapter;               // how many messages are in the current chapter
//    private int[] messageOrder = new int[10];       // the order the messages will be shown in (the 10 is the max number of messages)

    private string currentChapter;

    private TextMessageGroup currentChapterGroup = new TextMessageGroup();       // this is so it is easier to deal with the current chapter

    private float timeUntilHide;            // this counts down to let us know when to hide the bubble again

    // Use this for initialization
    void Start () {

        try { gameManager = FindObjectOfType<GameManager>(); }
            catch { Debug.Log("missing gameManager in scene"); }

//        UpdateChapterGroup();           // set the chaptergroup to match the chapter set in the gameManager

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

    // this creates the order that we will be shuffling the messages in
    void SetMessageOrder()
    {
/*
        for (int i = 0; i < messageGroup; i++)
        {
            indexList[i] = i;
        }

        return indexList;
*/
    }

    void ShowBubble()
    {
        bubble.SetActive(true);
    }

    void HideBubble()
    {
        bubble.SetActive(false);
    }

/*
    void UpdateChapterGroup()
    {
        Debug.Log("updating chapter group in TextBubble");

        // if the chapter has changed
        if (gameManager.GetCurrentChapter() != currentChapterGroup.chapter)
        {
            // set the chapter to something neutral in case we dont have a matching chapter
            currentChapterGroup.chapter = "";

            // then go through all the message groups
            foreach (TextMessageGroup messageGroup in messageGroupList)
            {
                // and find the one that matches the new chapter
                if (messageGroup.chapter == gameManager.GetCurrentChapter())
                {
                    // and set the currentChapterGroup to match it
                    currentChapterGroup = messageGroup;
                }
            }
        }
    }
*/
    void ShowMessage(int index)
    {

        if (currentChapterGroup.chapter != "")
        {
            textField.text = currentChapterGroup.textMessageList[index].text;
        }

    }

/*
    // can cycle through a series of text
    public void NextMessage()
    {
        // make our chapter is synced up with the gameManager
        UpdateChapterGroup();

        // show the next message for the current chapter
        ShowMessage(messageIndex);

        ShowBubble();

        timeUntilHide = currentChapterGroup.textMessageList[messageIndex].delay;

        // reset the message index if we have hit the end of the messages
        if (messageIndex >= currentChapterGroup.textMessageList.Count-1)
        {
            messageIndex = 0;
        }
        else
        {
            messageIndex++;
        }

    }
*/
    // can access the series randomly (but go through all of them)
    void RandomMessage()
    {

    }


}

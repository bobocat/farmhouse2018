using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Collections;


public class GameManager : MonoBehaviour {

    public List<Chapter> chapterList = new List<Chapter>();

    public List<ChapterControl> chapterObjectList = new List<ChapterControl>();

    public G.chapterType currentChapter;

    public Transform cameraRig;

    public List<Condition> conditionList = new List<Condition>();

//    public string currentChapter;
//    public int currentChapterIndex;

    public Transform MollonkaStart;
    public Transform DeathStart;
    public Transform FatherStart;
    public Transform WeddingStart;
    public Transform theVoid;               // a safe area where the headBall won't run into anything

//    public VRTK.VRTK_BasicTeleport teleporter;

    //    public DestinationMarker marker;

    public Animator titleAnimator;          // anim controller for the titles
    public Text titleText;                  // text of the titles

    public Transform sceneScaleObject;      // the object that we will scale to change the scene scale. top of the environment hierarchy

    private float sceneScaleMollonka = 1f;
    private float sceneScaleDeath = 1.4f;
    private float sceneScaleFather = 1f;

    private Transform playspace;
    private Transform headset;

    public Transform startGameGroup;        // this is some stuff that we should only see at the start of the game

    private void Awake()
    {
//        UpdateChapter();

    }

    // Use this for initialization
    void Start () {

        SetChapter(G.chapterType.tutorial);

        startGameGroup.gameObject.SetActive(true);

        Invoke("GetCameraRefs", 1f);

        // build the chapter list
        ChapterControl [] ccList  = FindObjectsOfType<ChapterControl>();
        foreach (ChapterControl cc in ccList)
        {
            chapterObjectList.Add(cc);
        }

//                Invoke("QuickStart", 2f);
//        StartCoroutine(TitlesOpening());

//                Invoke("Test", 3f);

        //        Invoke("Fade", 6f);

        //        Invoke("ChangeToMollonka", 6f);

        //        Invoke("ChangeToDeath", 6f);

        //        Invoke("ChangeToFather", 16f);


    }


    void QuickStart()
    {
        TeleportBrute(MollonkaStart);
        SetChapter(G.chapterType.mollonka);
    }

    bool CheckCondition(string conditionName)
    {
        bool status = false;

        foreach (Condition condition in conditionList)
        {
            if (condition.name == conditionName)
            {
                if (condition.complete)
                {
                    status = true;
                }
            }
        }

        return status;
    }

    void CheckChapterAdvance()
    {

        if (currentChapter == G.chapterType.mollonka)
        {
            if (CheckCondition("FatherMessages") && CheckCondition("WeddingMessages"))
            {
                SetChapter(G.chapterType.visions);
            }
        }
        else if (currentChapter == G.chapterType.visions)
        {
            if (CheckCondition("SawVision"))
            {
                SetChapter(G.chapterType.death);
                StartCoroutine(DeathOpening());
            }
        }
        else if (currentChapter == G.chapterType.death)
        {
            if (CheckCondition("GaveEgg"))
            {
                SetChapter(G.chapterType.father);
                StartCoroutine(FatherOpening());
            }
        }
        else if (currentChapter == G.chapterType.father)
        {
            if (CheckCondition("LookedEgg"))
            {
                SetChapter(G.chapterType.wedding);
                StartCoroutine(WeddingOpening());
            }
        }

    }

    void SetChapter(G.chapterType newChapter)
    {
        foreach (ChapterControl cc in chapterObjectList)
        {
            if (cc.chapter == newChapter)
            {
                cc.gameObject.SetActive(true);
            }
            else
            {
                cc.gameObject.SetActive(false);
            }
        }

        currentChapter = newChapter;
    }

    void GetCameraRefs()
    {
        playspace = FindObjectOfType<PlayspaceGrabber>().transform;
        //headset = Camera.main.transform;
        try
        {
            headset = FindObjectOfType<HeadsetGrabber>().transform;
        }
        catch
        {

        }

    }

    public void RunOpeningSequence()
    {
        StartCoroutine(TitlesOpening());
    }

    IEnumerator TitlesOpening()
    {

        startGameGroup.gameObject.SetActive(false);

        yield return new WaitForSeconds(1);

        PlayTitle("Mollonka");

        yield return new WaitForSeconds(8);     // wait while the title is playing

        SteamVR_Fade.Start(Color.black, 0f);    // fade the headset to black

        yield return new WaitForSeconds(1);     // wait while the title is playing

        TeleportBrute(MollonkaStart);           // go to the start position in the room

        SteamVR_Fade.Start(Color.clear, 4f);    // fade the headset to clear

        SetChapter(G.chapterType.mollonka);

    }

    IEnumerator DeathOpening()
    {

        startGameGroup.gameObject.SetActive(false);

        yield return new WaitForSeconds(1);


        PlayTitle("Death");

        yield return new WaitForSeconds(8);

        TeleportBrute(DeathStart);

    }


    IEnumerator FatherOpening()
    {

        yield return new WaitForSeconds(1);


        PlayTitle("Papa");

        yield return new WaitForSeconds(8);

        TeleportBrute(FatherStart);

    }

    IEnumerator WeddingOpening()
    {

        yield return new WaitForSeconds(1);

//        PlayTitle("Papa");

        yield return new WaitForSeconds(8);

        TeleportBrute(WeddingStart);

    }



    void PlayTitle(string txt)
    {
        titleText.text = txt;        
        titleAnimator.Play("title_mollonka");
    }

    void Test()
    {
        TeleportBrute(MollonkaStart);
    }

    void TeleportBrute(Transform destination)
    {

        Vector3 offset = new Vector3(headset.localPosition.x * -1f, headset.localPosition.y, headset.localPosition.z * -1f);

        // move the playspace to where the player is (only if this isn't the simulator)
        if (headset != null)
        {
            playspace.Translate(headset.localPosition);
        }


        // this works to rotate to the target position and compensates for the headset rotation
        float newRot = destination.localEulerAngles.y - headset.localEulerAngles.y;
        playspace.eulerAngles = Vector3.zero;     // this seems to work to set  the rotation to zero
        playspace.Rotate(0f, newRot, 0f);


        // move the playspace to the target location
        playspace.position = destination.position;

        // move the playspace the opposite of the first move to fix the player's location
        if (headset != null)
        {
            playspace.Translate(offset);
        }

    }



    public void SetSceneScale(float scale)
    {
        sceneScaleObject.transform.localScale = new Vector3(scale,scale,scale);       
    }


    public void ChangeToMollonka()
    {
//        StartCoroutine(TeleportIE(MollonkaStart));

        SetSceneScale(sceneScaleMollonka);

    }

    public void ChangeToDeath()
    {
//        StartCoroutine(TeleportIE(DeathStart));

        SetSceneScale(sceneScaleDeath);
    }

    public void ChangeToFather()
    {
//        StartCoroutine(TeleportIE(FatherStart));

        SetSceneScale(sceneScaleFather);
    }

    /*
        void Teleport()
        {

            Quaternion q = Quaternion.Inverse(VRTK.VRTK_DeviceFinder.HeadsetTransform().rotation) * MollonkaStart.rotation;

            teleporter.Teleport(MollonkaStart,MollonkaStart.position,q,true);

            Debug.Log("teleporting");

            GetComponent<VRTK_HeadsetFade>().Unfade(.3f);
        }
    */
    void Fade()
    {

//        GetComponent<VRTK_HeadsetFade>().Fade(Color.black, 3f);


//        marker.SetDestination(MollonkaStart.gameObject);
    }


	// Update is called once per frame
	void Update () {

		
	}

    /*
        // look at the current conditions and figure out what chapter we are in
        void UpdateChapter()
        {
            Debug.Log("updating chapter in gameManager");

            // if we don't find any incomplete chapters than we will return an all complete
            currentChapter = "allComplete";

            // go through the chapters in reverse order to see what the highest incomplete one is
            for (int i = chapterList.Count-1; i >= 0; i--)
            {
                if (chapterList[i].complete == false)
                {
                    currentChapter = chapterList[i].name;
                    currentChapterIndex = i;
                }
            }

        }
    */

    public void CompleteCondition(string conditionName)
    {
        if (conditionName == "") conditionName = "none";
        Debug.Log("completing condition: " + conditionName);

        foreach (Condition condition in conditionList)
        {
                if (condition.name == conditionName)
                {
                    condition.complete = true;

                    CheckChapterAdvance();
                }
        }
    }


}

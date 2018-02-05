using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HeadBall : MonoBehaviour {

    private Animator anim;

	// Use this for initialization
	void Start () {

        anim = GetComponent<Animator>();

        SetToClear();

	}

    public void SetToBlack()
    {
        anim.Play("black");
    }

    public void SetToClear()
    {
        anim.Play("clear");
    }


    public void FadeToBlack()
    {
        anim.Play("fadeToBlack");
    }

    public void FadeToClear()
    {
        anim.Play("fadeToClear");
    }


    // Update is called once per frame
    void Update () {
		
	}
}

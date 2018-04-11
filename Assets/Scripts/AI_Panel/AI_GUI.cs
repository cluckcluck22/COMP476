using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AI_GUI : MonoBehaviour {

    public string mString;
    public GameObject NameText;
    public Text mText;
    public float fadeTime;
    public bool displayName;
    public bool displayStats;

	void Start () 
    {
        NameText.SetActive(true);
        displayName = true;
        mText = NameText.GetComponentInChildren<Text>();
        mText.color = Color.clear;
	}
	

	void Update () 
    {
        FadeText();
	}

    //void OnMouseOver()
    //{
    //    displayStats = true;
    //}

    //void OnMouseExit()
    //{
    //    displayStats = false;
   // }

    void FadeText()
    {
        /*
        if(displayStats)
        {
            mText.text = mString;
            mText.color = Color.Lerp(mText.color, Color.white, fadeTime * Time.deltaTime);
        }
        else 
        {
            mText.color = Color.Lerp(mText.color, Color.clear, fadeTime * Time.deltaTime);
        }
        */

        if (displayName)
        {
            mText.text = mString;   //Name of the Animal is always displayed
            mText.color = Color.Lerp(mText.color, Color.black, fadeTime * Time.deltaTime);
        }
        else
        {
            displayName = false;
        }
    }
}

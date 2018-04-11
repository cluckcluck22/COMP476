using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AI_GUI : MonoBehaviour {

    public string mString;
    public Text mText;
    public float fadeTime;
    public bool displayStats;

	void Start () 
    {
        mText = GameObject.Find("Text").GetComponent<Text>();
        mText.color = Color.clear;
	}
	

	void Update () 
    {
        FadeText();
	}

    void OnMouseOver()
    {
        Debug.Log("DISPLAY TEXT!");
        displayStats = true;
    }

    void OnMouseExit()
    {
        displayStats = false;
    }

    void FadeText()
    {
        if(displayStats)
        {
            mText.text = mString;
            mText.color = Color.Lerp(mText.color, Color.white, fadeTime * Time.deltaTime);
        }
        else 
        {
            mText.color = Color.Lerp(mText.color, Color.clear, fadeTime * Time.deltaTime);
        }
    }
}

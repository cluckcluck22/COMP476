using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AI_GUI : MonoBehaviour
{

    //Name of Animal
    public string mString;
    public GameObject NameStatPanel;
    public GameObject NameText;
    public Text mText;
    [SerializeField]
    private bool displayName;

    //Stats of Animal
    public GameObject StatsPanel;   //when set to active images and text will be active as well
    public GameObject Stat1Text;
    public GameObject Stat2Text;
    public GameObject StatName;
    public Text mStatName;
    public Text mStat1Text;
    public Text mStat2Text;
    [SerializeField] 
    private bool displayStats;

    public float fadeTime;


    void Start()
    {
        NameStatPanel.SetActive(true);
        NameText.SetActive(true);
        displayName = true;
        mText = NameText.GetComponentInChildren<Text>();
        mText.color = Color.clear;
    }


    void Update()
    {
        FadeText();
    }

    void FadeText()
    {
        if (displayStats)
        {
            StatsPanel.SetActive(true);
            NameStatPanel.SetActive(false);
            NameText.SetActive(false);
            mStatName = StatName.GetComponentInChildren<Text>();
            mStat1Text = Stat1Text.GetComponentInChildren<Text>();
            mStat2Text = Stat2Text.GetComponentInChildren<Text>();
            mStatName.text = mString;
            mStat1Text.text = CurrentValueOfStat1();
            mStat2Text.text = CurrentValueOfStat2();
            mStatName.color = Color.Lerp(mText.color, Color.black, fadeTime * Time.deltaTime);
        }
        else
        {
            displayStats = false;
            NameStatPanel.SetActive(true);
            NameText.SetActive(true);
            if (StatsPanel != null)
                StatsPanel.SetActive(false);
            displayName = true;
            mText.text = mString;  
            mText.color = Color.Lerp(mText.color, Color.black, fadeTime * Time.deltaTime);
        }

    }

    private string CurrentValueOfStat1()
    {
        //TODO: Grab the stat1 here
        return "FuncStat1";
    }

    private string CurrentValueOfStat2()
    {
        //TODO: Grab the stat1 here
        return "FuncStat1";
    }

    //Get and Set
    //Will be set with framer radius trigger
    public bool DisplayStats
    {
        get
        {
            return displayStats;
        }

        set
        {
            displayStats = value;
        }
    }
}

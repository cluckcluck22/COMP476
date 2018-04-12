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

    //Animal AI information
    AnimalAI AnimalStat;


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
        //The Stats should only be viewable to the farmer when in range of AI, else farmer will only see the name of AI
        if (PhotonNetwork.connected || PhotonNetwork.isMasterClient)
        {
            if (displayStats)
            {
                if (gameObject.tag == "Dog") //Tagged on the AI canvas only for dogs
                {
                    return;
                }

                StatsPanel.SetActive(true);
                NameStatPanel.SetActive(false);
                NameText.SetActive(false);
                mStatName = StatName.GetComponentInChildren<Text>();
                mStat1Text = Stat1Text.GetComponentInChildren<Text>();
                mStat2Text = Stat2Text.GetComponentInChildren<Text>();

                mStatName.text = mString;
                mStat1Text.text = CurrentValueOfStat1();
                mStat2Text.text = CurrentValueOfStat2();
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

        //The Mimic only sees the names of the AI's
        if (!PhotonNetwork.connected || !PhotonNetwork.isMasterClient)
        {
            if (displayName)
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

    }

    private string CurrentValueOfStat1()
    {
        string text = "MIMIC HUNGER";
        AnimalStat = gameObject.GetComponentInParent<AnimalAI>();
        if (AnimalStat != null)
            text = ((int)AnimalStat.hunger).ToString();

        return text;
    }

    private string CurrentValueOfStat2()
    {
        string text = "MIMIC FATIGUE";
        AnimalStat = gameObject.GetComponentInParent<AnimalAI>();
        if (AnimalStat != null)
            text = ((int)AnimalStat.fatigue).ToString();

        return text;
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

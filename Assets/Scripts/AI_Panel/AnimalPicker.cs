using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalPicker : MonoBehaviour {

    bool SelectionEnabled;
    GameObject currentChoice;
    bool F1, F2, F3, F4;

    //Toggle Options
    public GameObject CowChoices;
    public GameObject PigChoices;
    public GameObject SheepChoices;
    public GameObject RamChoices;
    
    //Actual transforms of the Animal (clones)
    public GameObject[] MimicAnimalOptions;
    public Transform[] cows;
    public Transform[] pigs;
    public Transform[] sheep;
    public Transform[] rams;
    bool haveClones = false;

	// Use this for initialization
	void Start () 
    {
    }
	
	// Update is called once per frame
	void Update () 
    {
        //if(!haveClones) {
        //    MimicAnimalOptions = GameObject.FindGameObjectsWithTag("AI_Clones"); //Getting animals but we want clones
        //    haveClones = true;
        //    setFilteredList();
        //}

        MimicAnimalOptions = GameObject.FindGameObjectsWithTag("AI_Animal"); //Getting animals but we want clones

        HandleInput();

    }

    //Display the appropriate list of animal sub-category for player to see.
    void HandleInput()
    {
        if(Input.GetKeyDown(KeyCode.F1) && !F2 && !F3 && !F4)
        {       
            currentChoice = CowChoices;
            SelectionEnabled = !SelectionEnabled;
            F1 = !F1;       
        }
        else if(Input.GetKeyDown(KeyCode.F2) && !F1 && !F3 && !F4) 
        {
            currentChoice = PigChoices;
            SelectionEnabled = !SelectionEnabled;
            F2 = !F2;
        }
        else if(Input.GetKeyDown(KeyCode.F3) && !F1 && !F2 && !F4) 
        {
            currentChoice = SheepChoices;
            SelectionEnabled = !SelectionEnabled;
            F3 = !F3;
        }
        else if(Input.GetKeyDown(KeyCode.F4) && !F1 && !F2 && !F3) 
        {
            currentChoice = RamChoices;
            SelectionEnabled = !SelectionEnabled;
            F4 = !F4;
        }
        

        if (SelectionEnabled)
            currentChoice.SetActive(true);
        else
            currentChoice.SetActive(false);
    }

    void setFilteredList() 
    {
        cows = GetFilteredAnimalType("_Cow");
        pigs = GetFilteredAnimalType("_Pig");
        sheep = GetFilteredAnimalType("_Sheep");
        rams = GetFilteredAnimalType("_Ram");
    }

    //returns a filtered list of animal clones of designated (type)
    Transform[] GetFilteredAnimalType(string type) {
        //TODO
        return null;
    }

    void ChooseSelection(GameObject[] animal)
    {
        int counter = 0;
        animal = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            animal[i] = transform.GetChild(i).gameObject;
        }
    }


}

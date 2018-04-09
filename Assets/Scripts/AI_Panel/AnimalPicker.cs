using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalPicker : MonoBehaviour {

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
        if(Input.GetKeyDown(KeyCode.F1))
        {
            
        }
        else if(Input.GetKeyDown(KeyCode.F2)) 
        {
            
        }
        else if(Input.GetKeyDown(KeyCode.F3)) 
        {
            
        }
        else if(Input.GetKeyDown(KeyCode.F4)) 
        {
            
        }
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
        foreach(GameObject animal in MimicAnimalOptions)
        {
            if(animal.name.Contains(type))
            {
                return animal.transform;
            }
        }
        return null;
    }


}

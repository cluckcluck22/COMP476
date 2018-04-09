using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimalPicker : MonoBehaviour
{

    bool SelectionEnabled;
    GameObject currentChoice;
    bool F1, F2, F3, F4;
    int counter = 0;
    GameObject[] animal_arr;

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
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        ////Needs to be done
        //if (!haveClones)
        //{
        //    MimicAnimalOptions = GameObject.FindGameObjectsWithTag("AI_Clones"); //Getting animals but we want clones
        //    haveClones = true;
        //    setFilteredList();
        //}
        
        HandleInput();

    }

    //Display the appropriate list of animal sub-category for player to see.
    void HandleInput()
    {

        if (Input.GetKeyDown(KeyCode.F1) && !F2 && !F3 && !F4)
        {
            currentChoice = CowChoices;
            animal_arr = new GameObject[currentChoice.transform.childCount];
            SelectionEnabled = !SelectionEnabled;
            F1 = !F1;
        }
        else if (Input.GetKeyDown(KeyCode.F2) && !F1 && !F3 && !F4)
        {
            currentChoice = PigChoices;
            animal_arr = new GameObject[currentChoice.transform.childCount];
            SelectionEnabled = !SelectionEnabled;
            F2 = !F2;
        }
        else if (Input.GetKeyDown(KeyCode.F3) && !F1 && !F2 && !F4)
        {
            currentChoice = SheepChoices;
            animal_arr = new GameObject[currentChoice.transform.childCount];
            SelectionEnabled = !SelectionEnabled;
            F3 = !F3;
        }
        else if (Input.GetKeyDown(KeyCode.F4) && !F1 && !F2 && !F3)
        {
            currentChoice = RamChoices;
            animal_arr = new GameObject[currentChoice.transform.childCount];
            SelectionEnabled = !SelectionEnabled;
            F4 = !F4;
        }


        if (SelectionEnabled)
        {
            currentChoice.SetActive(true);
        }
        else
        {
            if (currentChoice == null)
                return;

            currentChoice.SetActive(false);
        }

        //Mouse ScrollWheel Up or E
        if ((Input.GetAxis("Mouse ScrollWheel") < 0 || Input.GetKeyDown(KeyCode.E)) && SelectionEnabled)
        {
            ChooseSelectionForward(currentChoice);
        }

        //Mouse ScrollWheel Down or Q
        if ((Input.GetAxis("Mouse ScrollWheel") > 0 || Input.GetKeyDown(KeyCode.Q)) && SelectionEnabled)
        {
            ChooseSelectionBackward(currentChoice);
        }
    }


    //returns a filtered list of animal clones of designated (type)
    public void OnValueChange()
    {
        
    }

    void ChooseSelectionForward(GameObject currentChoice)
    {
        Debug.Log("In choose seclection");

        PopulateTheAnimalArr(currentChoice);

        //Move through the array and when on the selected choice highlight the selection:
        animal_arr[counter % animal_arr.Length].GetComponent<Toggle>().isOn = true;
        counter++;

        //Create another function that will add the  transform of the clone set to a certain number....1, 2, 3, 4

    }

    void ChooseSelectionBackward(GameObject currentChoice)
    {
        if (counter == 0)
            counter = animal_arr.Length;


        PopulateTheAnimalArr(currentChoice);

        //Move through the array and when on the selected choice highlight the selection:
        animal_arr[counter % animal_arr.Length].GetComponent<Toggle>().isOn = true;
        counter--;
    }

    void PopulateTheAnimalArr(GameObject currentChoice)
    {
        //Populated the array with the toggle choices for that animal
        for (int i = 0; i < animal_arr.Length; i++)
        {
            animal_arr[i] = currentChoice.transform.GetChild(i).gameObject;
            if (i == 0)
            {
                animal_arr[i].GetComponent<Toggle>().isOn = true;
            }
        }
    }

}

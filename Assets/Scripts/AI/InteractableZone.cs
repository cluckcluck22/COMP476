using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableZone : MonoBehaviour {

    List<AnimalAI> animals;
    Interactable[] iObjects;

    private void OnTriggerEnter(Collider other)
    {
        //TODO register animal
    }

    private void OnTriggerExit(Collider other)
    {
        //TODO deregister animal
    }

    public int getAnimalCount(string type)
    {
        int count=0;

        foreach (AnimalAI a in animals)
        {
            //if the animal is of the correct type, increment count
        }

        return count;
    }

    public bool hasInteractable(Interactable.Type t)
    {
        for (int i = 0; i < iObjects.Length; i++)
        {
            if (iObjects[i].getType() == t) { return true; }
        }

        return false;
    }
}

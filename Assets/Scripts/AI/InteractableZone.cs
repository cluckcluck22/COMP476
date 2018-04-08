using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableZone : MonoBehaviour {

    List<AnimalAI> animals;
    List<Interactable> iObjects;

    private void Start()
    {
        animals = new List<AnimalAI>();
        iObjects = new List<Interactable>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable != null)
        {
            iObjects.Add(interactable);
        }

        AnimalAI animal = other.GetComponent<AnimalAI>();
        if (animal != null)
        {
            animals.Add(animal);
        }
    }

    public void handlerOntriggerEnter(Collider other)
    {

    }

    private void OnTriggerExit(Collider other)
    {
        AnimalAI animal = other.GetComponent<AnimalAI>();
        if (animal != null)
        {
            animals.Remove(animal);
        }
    }

    public int getAnimalCount(Species type)
    {
        int count=0;

        foreach (AnimalAI a in animals)
        {
            if (a.getSpecies() == type)
            {
                count++;
            }
        }

        return count;
    }

    public bool hasInteractable(Interactable.Type t)
    {
        for (int i = 0; i < iObjects.Count; i++)
        {
            if (iObjects[i].getType() == t &&
                iObjects[i].isAvailable())
            {
                return true;
            }
        }

        return false;
    }
}

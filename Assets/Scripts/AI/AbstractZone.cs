using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class AbstractZone : MonoBehaviour
{
    HashSet<AnimalAI> animals;

    private void Start()
    {
        animals = new HashSet<AnimalAI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        handleOnTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        handleOnTriggerExit(other);
    }

    protected virtual void handleOnTriggerEnter(Collider other)
    {
        AnimalAI animal = other.GetComponent<AnimalAI>();
        if (animal != null)
        {
            animals.Add(animal);
        }
    }

    protected virtual void handleOnTriggerExit(Collider other)
    {
        AnimalAI animal = other.GetComponent<AnimalAI>();
        if (animal != null)
        {
            animals.Remove(animal);
        }
    }

    public int getAnimalCount(Species type)
    {
        int count = 0;
        foreach (AnimalAI a in animals)
        {
            if (a.getSpecies() == type)
            {
                count++;
            }
        }
        return count;
    }

    public bool isAnimalInZone(AnimalAI animal)
    {
        return animals.Contains(animal);
    }


}

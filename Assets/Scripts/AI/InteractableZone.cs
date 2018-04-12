using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableZone : MonoBehaviour {

    private HashSet<AnimalAI> animals;

    public ZoneType type;

    public List<Interactable> needsInteractables { get; private set; }
    public List<Interactable> rallyInteractables { get; private set; }

    public enum ZoneType
    {
        Needs,
        Rally
    }

    private void Awake()
    {
        animals = new HashSet<AnimalAI>();
        needsInteractables = new List<Interactable>();
        rallyInteractables = new List<Interactable>();

        Interactable[] interactableObjects = GetComponentsInChildren<Interactable>();

        foreach (Interactable el in interactableObjects)
        {
            switch (el.type)
            {
                case Interactable.Type.Food:
                case Interactable.Type.Rest:
                    needsInteractables.Add(el);
                    break;
                default:
                    rallyInteractables.Add(el);
                    break;
            }
        }
    }

    private void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        AnimalAI animal = other.GetComponent<AnimalAI>();
        if (animal != null)
        {
            animals.Add(animal);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        AnimalAI animal = other.GetComponent<AnimalAI>();
        if (animal != null)
        {
            animals.Remove(animal);
        }
    }

    public bool hasNeedsInteractable(Interactable.Type t)
    {
        for (int i = 0; i < needsInteractables.Count; i++)
        {
            if (needsInteractables[i].getType() == t &&
                needsInteractables[i].isAvailable())
            {
                return true;
            }
        }
        return false;
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

    public int getAllAnimalCount()
    {
        return animals.Count;
    }

    public bool isAnimalInZone(AnimalAI animal)
    {
        return animals.Contains(animal);
    }

    public Interactable getAvailableRallySpot()
    {
        foreach (Interactable interactable in rallyInteractables)
        {
            if (interactable.isAvailable())
                return interactable;
        }
        return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BTCoroutine = System.Collections.Generic.IEnumerator<BTNodeResult>;

public class ZoneManager : MonoBehaviour {

    public InteractableZone[] interactableZones;

    public List<InteractableZone> needsZones { get; private set; }
    public List<InteractableZone> rallyZones { get; private set; }

    private void Awake()
    {
        needsZones = new List<InteractableZone>();
        rallyZones = new List<InteractableZone>();

        foreach (InteractableZone zone in interactableZones)
        {
            if (zone.type == InteractableZone.ZoneType.Rally)
            {
                rallyZones.Add(zone);
            }
            else
            {
                needsZones.Add(zone);
            }
        }
    }

    public Transform findInteractableZone(AnimalAI animal, Interactable.Type type)
    {
        InteractableZone candidate = null;
        int highestCount = int.MaxValue;
        int currentCount = 0;

        float closestDistance = float.MaxValue;
        InteractableZone closestZone = needsZones[0];

        foreach (InteractableZone zone in needsZones)
        {
            if (zone.hasNeedsInteractable(type))
            {
                currentCount = zone.getAnimalCount(animal.getSpecies());
                if (zone.isAnimalInZone(animal))
                    currentCount--;

                if (currentCount < highestCount)
                {
                    highestCount = currentCount;
                    candidate = zone;
                }

                float currentDistance = (zone.transform.position - animal.transform.position).magnitude;
                if (currentDistance <= closestDistance)
                {
                    closestDistance = currentDistance;
                    closestZone = zone;
                }
            }


        }
        if (candidate == null)
        {
            return closestZone.transform;
        }
        else
        {
            return candidate.transform;
        }
    }

    public Transform findRallyZone(AnimalAI animal)
    {
        InteractableZone candidate = null;
        int bestCount = 0;
        int currentCount = 0;
        int leastOccupiedZone = int.MaxValue;
        InteractableZone clearestZone = null;

        foreach (InteractableZone zone in rallyZones)
        {
            currentCount = zone.getAnimalCount(animal.getSpecies());
            if (zone.isAnimalInZone(animal))
                currentCount--;

            if (currentCount > bestCount)
            {
                bestCount = currentCount;
                candidate = zone;
            }

            int animalCount = zone.getAllAnimalCount();
            if (animalCount <= leastOccupiedZone)
            {
                leastOccupiedZone = animalCount;
                clearestZone = zone;
            }
        }

        if (candidate == null)
        {
            return clearestZone.transform;
        }
        else
        {
            return candidate.transform;
        }
    }

    public List<Interactable> getAllNeedsInteractables()
    {
        List<Interactable> output = new List<Interactable>();

        foreach (InteractableZone zone in interactableZones)
        {
            output.AddRange(zone.needsInteractables);
        }

        return output;
    }

    public InteractableZone findMyZone(AnimalAI animal)
    {
        foreach (InteractableZone zone in needsZones)
        {
            if (zone.isAnimalInZone(animal))
                return zone;
        }

        foreach (InteractableZone zone in rallyZones)
        {
            if (zone.isAnimalInZone(animal))
                return zone;
        }

        return null;
    }

    public Interactable requestIdleInteractable(AnimalAI animal)
    {
        InteractableZone zone = findMyZone(animal);
        if (zone != null)
        {
            Interactable interactable = zone.getAvailableRallySpot();
            if (interactable != null)
            { 
                interactable.reserve(animal);
                return interactable;
            }
        }
        return null;
    }

    public bool canRequestIdleInteractable(AnimalAI animal)
    {
        InteractableZone zone = findMyZone(animal);
        if (zone != null)
        {
            Interactable interactable = zone.getAvailableRallySpot();
            if (interactable != null)
            {
                return true;
            }
        }
        return false;
    }
}

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
        int highestCount = 0;
        int currentCount = 0;
        foreach (InteractableZone zone in needsZones)
        {
            if (zone.hasNeedsInteractable(type))
            {
                currentCount = zone.getAnimalCount(animal.getSpecies());
                if (zone.isAnimalInZone(animal))
                    currentCount--;

                if (currentCount > highestCount)
                {
                    highestCount = currentCount;
                    candidate = zone;
                }
            }
        }
        if (candidate == null)
        {
            // Pick a random one
            int random = Random.Range(0, needsZones.Count - 1);
            candidate = interactableZones[random];
        }

        return candidate.transform;
    }

    public Transform findRallyZone(AnimalAI animal)
    {
        InteractableZone candidate = null;
        int bestCount = 0;
        int currentCount = 0;
        float closestDistance = float.MaxValue;
        InteractableZone closestZone = null;

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

            float distance = (zone.transform.position - animal.transform.position).magnitude;
            if ( distance <= closestDistance )
            {
                closestDistance = distance;
                closestZone = zone;
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
                return interactable;
            }
        }
        return null;
    }
}

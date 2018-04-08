using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour {

	public InteractableZone[] interactableZones;

    private List<InteractableZone> needsZones;
    private List<InteractableZone> rallyZones;

    private void Start()
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

    public Transform FindInteractableZone(AnimalAI animal, Interactable.Type type)
    {
        InteractableZone candidate = null;
        int highestCount = 0;
        int currentCount = 0;
        foreach (InteractableZone zone in needsZones)
        {
            if (zone.hasInteractable(type))
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

    public Transform FindRallyZone(AnimalAI animal)
    {
        InteractableZone candidate = null;
        int bestCount = 0;
        int currentCount = 0;
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
        }

        if (candidate == null)
        {
            // Pick a random one
            int random = Random.Range(0, rallyZones.Count - 1);
            candidate = interactableZones[random];
        }

        return candidate.transform;

    }
}

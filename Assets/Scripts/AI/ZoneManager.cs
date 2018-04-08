using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour {

	public InteractableZone[] interactableZones;

    public Transform FindInteractableZone(AnimalAI animal, Interactable.Type type)
    {
        int highestCount = -1;
        int currentCount = 0;
        InteractableZone candidate = null;
        foreach (InteractableZone zone in interactableZones)
        {
            if (zone.hasInteractable(type))
            {
                currentCount = zone.getAnimalCount(animal.getSpecies());
                if (currentCount > highestCount)
                {
                    highestCount = currentCount;
                    candidate = zone;
                }
            }
        }
        if (candidate != null)
        {
            return candidate.transform;
        }
        else
        {
            return null;
        }
    }

    public Transform FindRallyZone(AnimalAI animal)
    {
        // This should only be called once, when the animal has all its needs fulfilled

        List<InteractableZone> rallyZones = new List<InteractableZone>();
        foreach (InteractableZone zone in interactableZones)
        {
            if (zone.type == InteractableZone.ZoneType.Rally)
            {
                rallyZones.Add(zone);
            }
        }

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

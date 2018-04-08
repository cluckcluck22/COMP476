using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RallyZone
{
    public Species species;
    public Transform position;
}

public class ZoneManager : MonoBehaviour {

	public InteractableZone[] interactableZones;
    public RallyZone[] rallyZones;

    private Dictionary<Species, List<Transform>> rallyDictionay;

    private void Awake()
    {
        rallyDictionay = new Dictionary<Species, List<Transform>>();
        foreach (RallyZone zone in rallyZones)
        {
            if (!rallyDictionay.ContainsKey(zone.species))
            {
                rallyDictionay[zone.species] = new List<Transform>();
            }
            rallyDictionay[zone.species].Add(zone.position);
        }
    }

    public Transform FindInteractableZone(AnimalAI animal, Interactable.Type type)
    {
        int highestCount = -1;
        InteractableZone candidate = null;
        foreach (InteractableZone zone in interactableZones)
        {
            if (zone.hasInteractable(type))
            {
                if (zone.getAnimalCount(animal.getSpecies()) > highestCount)
                {
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
        if (!rallyDictionay.ContainsKey(animal.getSpecies()))
        {
            throw new Exception("Zone manager has no rally zone for species : " + animal.getSpecies().ToString());
        }

        return null;
        // Find closest rally zone

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PerceptionModule
{
    private class PerceptionEntry
    {
        public AnimalAI animal { get; set; }
        public BoxCollider boxCollider { get; set; }
        public Vector3 lkp { get; set; }
        public float lkpTimeStamp { get; set; }
        public bool isLkpValid { get; set; }
        public bool isFlock { get; set; }
        public float aversion { get; set; }


        public PerceptionEntry(AnimalAI animal, bool isFlock)
        {
            this.animal = animal;
            this.lkp = new Vector3();
            this.isLkpValid = false;
            this.lkpTimeStamp = float.MinValue;
            this.aversion = 0.0f;
            this.isFlock = isFlock;
            this.boxCollider = animal.GetComponent<BoxCollider>();
        }
    }



    private AnimalAI self;
    private Dictionary<AnimalAI, PerceptionEntry> entries;
    public Vector3 flockCenter { get; private set; }
    public bool hasNoFlock { get; private set; }
    public List<Interactable> interactablesInRange { get; private set; }

    public PerceptionModule(AnimalAI self)
    {
        this.self = self;
        this.entries = new Dictionary<AnimalAI, PerceptionEntry>();
        this.flockCenter = self.getPosition();
        hasNoFlock = false;
        interactablesInRange = new List<Interactable>();
    }

    public void drawPerception()
    {
        foreach(PerceptionEntry entry in entries.Values)
        {
            if (entry.isLkpValid)
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.yellow;

            Gizmos.DrawSphere(entry.lkp + 2 * Vector3.up, 0.5f);
            Gizmos.DrawLine(entry.lkp, entry.animal.getPosition() + 2 * Vector3.up);

            if (entry.isFlock)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(entry.animal.getPosition() + 3 * Vector3.up, 0.5f);
            }

            Gizmos.color = Color.black;
            Gizmos.DrawSphere(entry.animal.getPosition() + 4 * Vector3.up, 0.5f);

        }

        Gizmos.color = Color.black;
        foreach (Interactable interactable in interactablesInRange)
        {
            Gizmos.DrawSphere(interactable.transform.position + 4 * Vector3.up, 0.5f);
        }

        if (!hasNoFlock)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(flockCenter + 2 * Vector3.up, 0.5f);
        }
    }

    #region Entry manipulation
    public void register(AnimalAI animal)
    {
        if (!entries.Keys.Contains(animal))
        {
            entries[animal] = new PerceptionEntry(animal, animal.getSpecies() == self.getSpecies());
        }
    }

    public void overrideFlockAffiliation(AnimalAI animal, bool isFlockOverride)
    {
        if (entries.Keys.Contains(animal))
        {
            entries[animal].isFlock = isFlockOverride;
        }
    }
    #endregion

    #region Updaters
    public void updateEntries()
    {
        int layerIndex = LayerMask.NameToLayer("Animal");
        int layerMask = 1 << layerIndex;
        Collider[] colliders = Physics.OverlapSphere(self.getPosition(), self.getSighDistance() + 10.0f, layerMask);
        AnimalAI animal;
        foreach (Collider collider in colliders)
        {
            animal = null;
            animal = collider.GetComponent<AnimalAI>();
            if (animal != null && animal != self && !entries.Keys.Contains(animal))
            {
                float angleToTarget = Vector3.Angle(collider.transform.position - self.getHeadPosition(), self.transform.forward);
                if (angleToTarget <= self.getFov() / 2 && hasLineOfSight(collider as BoxCollider))
                    register(animal);
            }
        }
    }

    public void updateInteractables()
    {
        interactablesInRange.Clear();
        int layerIndex = LayerMask.NameToLayer("Interactable");
        int layerMask = 1 << layerIndex;
        Collider[] colliders = Physics.OverlapSphere(self.getPosition(), self.getSighDistance() + 10.0f, layerMask);
        Interactable interactable;
        foreach (Collider collider in colliders)
        {
            interactable = null;
            interactable = collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                interactablesInRange.Add(interactable);
            }
        }
    }

    public void updateFlockCenter()
    {
        hasNoFlock = false;

        // Consider entries that are alive, part of the flock and has valid lkp
        List<PerceptionEntry> validFlockMembers = new List<PerceptionEntry>();
        foreach (PerceptionEntry entry in entries.Values)
        {
            if (entry.isFlock && !entry.animal.isDead() && entry.isLkpValid)
                validFlockMembers.Add(entry);
        }

        // If there are no valid members, then the center of mass is the entity itself
        // Raise the hasNoFlock flag so that the BT can use it and behave accordingly
        if (validFlockMembers.Count() == 0)
        {
            flockCenter = self.getPosition();
            hasNoFlock = true;
            return;
        }

        // Find average position
        Vector3 avgPos = Vector3.zero;
        foreach (PerceptionEntry entry in validFlockMembers)
        {
            avgPos += entry.lkp;
        }
        avgPos /= validFlockMembers.Count();

        // Find the distance for one standard deviation
        float variance = 0.0f;
        foreach (PerceptionEntry entry in validFlockMembers)
        {
            variance += Vector3.SqrMagnitude(entry.lkp - avgPos);
        }
        if (validFlockMembers.Count != 1)
            variance /= validFlockMembers.Count - 1;

        float sigma = Mathf.Sqrt(variance);

        // Define center of mass as the avg position of all valid flock members within one standard
        // deviations of the average position
        flockCenter = Vector3.zero;
        int count = 0;
        foreach (PerceptionEntry entry in validFlockMembers)
        {
            if ((avgPos - entry.animal.getPosition()).magnitude <=  sigma)
            {
                count++;
                flockCenter += entry.animal.getPosition();
            }
        }
        if (count != 0)
        {
            flockCenter /= count;
        }
    }

    public void updateAllLkp()
    {
        foreach (PerceptionEntry entry in entries.Values)
            updateLkp(entry);
    }

    private void updateLkp(PerceptionEntry entry)
    {
        // Can see target
        float angleToTarget = Vector3.Angle(entry.boxCollider.bounds.center - self.getHeadPosition(), self.transform.forward);
        if (angleToTarget <= self.getFov()/2 &&
            (entry.boxCollider.bounds.center - self.getHeadPosition()).magnitude <= self.getSighDistance() &&
            hasLineOfSight(entry.boxCollider))
        {
            entry.lkp = entry.animal.transform.position;
            entry.isLkpValid = true;
            entry.lkpTimeStamp = Time.time;
            return;
        }

        // Can see lkp (but not on target)
        float angleToLkp = Vector3.Angle(entry.lkp - self.getHeadPosition(), self.transform.forward);
        if (angleToLkp <= self.getFov()/2 &&
            (entry.lkp - self.getHeadPosition()).magnitude <= self.getLkpSightDistance() &&
            hasLineOfSight(entry.lkp))
        {
            entry.isLkpValid = false;
            entry.lkpTimeStamp = float.MinValue;
            return;
        }

        if (entry.lkpTimeStamp + entry.animal.getLkpExpire() <= Time.time)
        {
            entry.isLkpValid = false;
            entry.lkpTimeStamp = float.MinValue;
            return;
        }
    }


    #endregion

    #region Helpers
    public bool hasLineOfSight(BoxCollider box)
    {
        RaycastHit hit;
        Vector3[] testPositions = { box.bounds.center, box.bounds.min, box.bounds.max };
        Vector3 start = self.getHeadPosition();

        foreach (Vector3 pos in testPositions)
        {
            Physics.Linecast(start, box.bounds.center, out hit);
            if (hit.collider == box)
            {
                return true;
            }
        }
        return false;
    }

    public bool hasLineOfSight(Vector3 position)
    {
        if (!Physics.Linecast(self.getHeadPosition(), position))
            return true;

        return false;
    }
    #endregion

}

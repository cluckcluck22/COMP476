using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableZone : AbstractZone {

    List<Interactable> iObjects;

    private void Start()
    {
        iObjects = new List<Interactable>();
    }

    protected override void handleOnTriggerEnter(Collider other)
    {
        base.handleOnTriggerEnter(other);
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable != null)
        {
            iObjects.Add(interactable);
        }
    }

    protected override void handleOnTriggerExit(Collider other)
    {
        base.handleOnTriggerExit(other);
        // Interactables should never "exit" the trigger collider
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

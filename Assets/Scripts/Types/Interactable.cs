using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {

    public enum Type
    {
        Food,
        Rest,
        Talk,
        Chill
    }

    public float maxCount;
    public Type type;

    public Transform[] interactionSpots;

    public bool useOnlyFront;

    private float count;

    private int reservedCount;

    private AnimalAI[] occupied;

    private Transform[] actualInteractionSpots;

    private List<AnimalAI> clients;

    // Use this for initialization
    void Start ()
    {
        count = maxCount;

        clients = new List<AnimalAI>();

        if (useOnlyFront)
        {
            actualInteractionSpots = new Transform[2];
            actualInteractionSpots[0] = interactionSpots[0];
            actualInteractionSpots[1] = interactionSpots[4];
        }
        else
        {
            actualInteractionSpots = interactionSpots;
        }

        occupied = new AnimalAI[actualInteractionSpots.Length];
        for (int i = 0; i < occupied.Length; i++)
        {
            occupied[i] = null;
        }

        reservedCount = 0;
    }

    void Update()
    {
        foreach (AnimalAI animal in clients)
        {
            if (animal == null)
                continue;

            switch (type)
            {
                case Type.Food:
                    animal.giveFood(consume(animal.getConsumption(Type.Food)));
                    break;
                case Type.Rest:
                    animal.giveRest(consume(animal.getConsumption(Type.Rest)));
                    break;
            }
        }
    }

    public void attach(AnimalAI animal)
    {
        clients.Add(animal);
    }

    public void detach(AnimalAI animal)
    {
        clients.Remove(animal);
        unReserve(animal);
    }

    public void reserve(AnimalAI animal)
    {
        for (int i = 0; i < occupied.Length; i++)
        {
            if (occupied[i] == null)
            {
                occupied[i] = animal;
                break;
            }
        }
        reservedCount++;
        if (reservedCount > occupied.Length)
            print("Interactable too many reserved");
    }

    public void unReserve(AnimalAI animal)
    {
        for (int i = 0; i < occupied.Length; i++)
        {
            if (occupied[i] == animal)
            {
                occupied[i] = null;
            }
        }
        reservedCount--;
    }

    public void fill(float amount)
    {
        count += amount;
        count = Mathf.Min(maxCount, count);
    }

    private float consume(float amount)
    {
        if (amount >= count)
        {
            count = 0;
            return count;
        }

        count -= amount;
        return amount;
    }

    public bool isEmpty()
    {
        return count == 0;
    }

    public bool isAvailable()
    {
        return reservedCount < actualInteractionSpots.Length && (!isNeedsInterable() || !isEmpty());
    }

    public Interactable.Type getType()
    {
        return type;
    }

    public Transform getInteractionPos(AnimalAI animal)
    {
        int spot = -1;
        for (int i = 0; i < occupied.Length; i++)
        {
            if (occupied[i] == animal)
            {
                spot = i;
                break;
            }
        }
        if (spot == -1)
            print("Interactable can't find spot");

        return actualInteractionSpots[spot];
    }

    public bool isNeedsInterable()
    {
        switch (type)
        {
            case Type.Food:
            case Type.Rest:
                return true;
            default:
                return false;
        }
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {

    public enum Type
    {
        Food,
        Rest,
        Play
    }

    public float maxCount;
    public Type type;

    public Transform[] interactionSpots;

    private float count;
    private List<AnimalAI> clients;
    private List<AnimalAI> reserved;

	// Use this for initialization
	void Start ()
    {
        count = maxCount;
        clients = new List<AnimalAI>();
        reserved = new List<AnimalAI>();
	}

    void Update()
    {
        foreach (AnimalAI animal in clients)
        {
            switch (type)
            {
                case Type.Food:
                    animal.giveFood(consume(animal.getConsumption(Type.Food)));
                    break;
                case Type.Play:
                    animal.giveEntertainement(consume(animal.getConsumption(Type.Play)));
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
        reserved.Remove(animal);
    }

    public void reserve(AnimalAI animal)
    {
        reserved.Add(animal);
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
        return reserved.Count < interactionSpots.Length && !isEmpty();
    }

    public int getClients()
    {
        return reserved.Count;
    }

    public Interactable.Type getType()
    {
        return type;
    }

    public Vector3 getInteractionPos(AnimalAI animal)
    {
        int spot = reserved.IndexOf(animal);
        Matrix4x4 toWorld = transform.localToWorldMatrix;
        //Vector3 offset = interactionSpots[spot].position;
        //Vector4 spotPoint = new Vector4(offset.x, offset.y, offset.z, 1.0f);
        //return toWorld * spotPoint;
        return interactionSpots[spot].position;
    }
}

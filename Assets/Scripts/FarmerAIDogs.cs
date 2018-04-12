using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmerAIDogs : MonoBehaviour
{

    public GameObject Dog;

    void Start()
    {

    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.E))
        {
            //Patrol
            Dog.GetComponent<DogAI>().TellDogToPatrol();
        }

        if (Input.GetKey(KeyCode.Q))
        {
            //Follow
            Dog.GetComponent<DogAI>().TellDogToFollow();
        }
    }
}

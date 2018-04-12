using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmerAIDogs : MonoBehaviour {

    DogAI myDog;

	void Start () 
    {
		
	}
	
	void FixedUpdate ()
    {
        if (Input.GetKey(KeyCode.E))
        {
            //Patrol
            //myDog = GetComponent<>(DogAI).TellDogToPatrol();
        }

        if (Input.GetKey(KeyCode.Q))
        {
            //Follow
            //myDog = GetComponent<>(DogAI).TellDogToFollow();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmerAIDogs : MonoBehaviour {

    public GameObject Dog;
    public GameObject[] DogArr;

	void Start () 
    {
		
	}
	
	void FixedUpdate ()
    {
        if (Input.GetKey(KeyCode.E))
        {
            //Patrol
            foreach(GameObject dog in DogArr)
            { 
                Dog.GetComponent<DogAI>().TellDogToPatrol();
            }
        }

        if (Input.GetKey(KeyCode.Q))
        {
            //Follow
            foreach (GameObject dog in DogArr)
            {
                Dog.GetComponent<DogAI>().TellDogToFollow();
            }
        }
    }
}

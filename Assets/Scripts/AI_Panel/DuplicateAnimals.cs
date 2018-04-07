using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuplicateAnimals : MonoBehaviour {

    public GameObject[] AI;
    public GameObject[] duplicateAI;
    public Transform cloneSpawn;
	// Use this for initialization
	void Start () {
        AI = GetAnimals();
        duplicateAI = CloneAI(AI);
    }

    // Update is called once per frame
    void Update () {
		
	}

    GameObject[] GetAnimals()
    {
        GameObject[] AI = GameObject.FindGameObjectsWithTag("AI_Animal");
        return AI;
    }

    GameObject[] CloneAI(GameObject[] animalsToClone)
    {
        GameObject[] clones = new GameObject[animalsToClone.Length];
        int i = 0;
        foreach(GameObject animal in animalsToClone)
        {
            clones[i] = Instantiate(animal,cloneSpawn.position,Quaternion.identity);
            clones[i].GetComponent<AnimatorDriverAnimal>().enabled = false;
            clones[i].GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
            clones[i].GetComponent<Rigidbody>().isKinematic = true;
            clones[i].GetComponent<AnimalAI>().enabled = false;
            i++;
        }
        return clones;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuplicateAnimals : MonoBehaviour {

    GameObject[] AI;
    public GameObject[] duplicateAI;
    public Transform cloneSpawn;
	// Use this for initialization
	void Start () {
        AI = GetAnimals();
        duplicateAI = CloneAI(AI);
    }

    // Update is called once per frame
    void Update()
    {
        TransformToSelectedOption();
    }

    void DetachCurrentChild()
    {
        if(transform.childCount == 1)
        {
            Transform child = transform.GetChild(0);
            transform.DetachChildren();
            child.position = cloneSpawn.position;
        }
    }

    void TransformToSelectedOption()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            DetachCurrentChild();
            duplicateAI[0].transform.position = transform.position;
            duplicateAI[0].transform.parent = this.transform;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            DetachCurrentChild();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            DetachCurrentChild();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            DetachCurrentChild();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            DetachCurrentChild();
        }
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
            AnimalAI animalAiScript = clones[i].GetComponent<AnimalAI>();
            animalAiScript.enabled = false;
            animalAiScript.debugBT = false;
            animalAiScript.debugNav = false;
            animalAiScript.debugPerception = false;
            i++;
        }
        return clones;
    }
}

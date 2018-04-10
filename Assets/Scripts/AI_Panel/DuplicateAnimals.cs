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
        CorrectAI(duplicateAI);
        duplicateAI[0].transform.position = transform.position;
        duplicateAI[0].transform.parent = this.transform;
        GetComponent<MimicMovemenment>().m_AnimatorDriverAnimal = transform.GetChild(0).GetComponent<AnimatorDriverAnimal>();
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
            GetComponent<MimicMovemenment>().m_AnimatorDriverAnimal = transform.GetChild(0).GetComponent<AnimatorDriverAnimal>();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            DetachCurrentChild();
            duplicateAI[1].transform.position = transform.position;
            duplicateAI[1].transform.parent = this.transform;
            GetComponent<MimicMovemenment>().m_AnimatorDriverAnimal = transform.GetChild(1).GetComponent<AnimatorDriverAnimal>();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            DetachCurrentChild();
            duplicateAI[2].transform.position = transform.position;
            duplicateAI[2].transform.parent = this.transform;
            GetComponent<MimicMovemenment>().m_AnimatorDriverAnimal = transform.GetChild(2).GetComponent<AnimatorDriverAnimal>();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            DetachCurrentChild();
        }
    }

    GameObject[] GetAnimals()
    {
        GameObject[] AI = GameObject.FindGameObjectsWithTag("AI_Animal");
        return AI;
    }

    public void CorrectAI(GameObject[] AI)
    {
        foreach(GameObject animal in AI)
        {
            animal.GetComponent<AnimatorDriverAnimal>().enabled = false;
            animal.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
            animal.GetComponent<Rigidbody>().isKinematic = true;
            AnimalAI animalAiScript = animal.GetComponent<AnimalAI>();
            animalAiScript.enabled = false;
            animalAiScript.debugBT = false;
            animalAiScript.debugNav = false;
            animalAiScript.debugPerception = false;
        }
    }
}

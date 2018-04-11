using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuplicateAnimals : MonoBehaviour {

    public GameObject[] duplicateAI;
    public Transform cloneSpawn;
    public Transform[] filteredAnimals;
    public GameObject TransformGUI;
	// Use this for initialization
	void Start () {
        CorrectAI(duplicateAI);
        duplicateAI[0].transform.parent = this.transform;
        duplicateAI[0].transform.position = transform.position;
        duplicateAI[0].transform.rotation = transform.rotation;
        GetComponent<MimicMovemenment>().m_AnimatorDriverAnimal = transform.GetChild(0).GetComponent<AnimatorDriverAnimal>();
        Physics.IgnoreCollision(duplicateAI[0].GetComponent<Collider>(), transform.GetComponent<Collider>());
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        TransformToSelectedOption();
        CorrectRotationAndPositionOnError();
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
            filteredAnimals = TransformGUI.GetComponent<TransformGUI>().m_transforms;
            DetachCurrentChild();
            Physics.IgnoreCollision(filteredAnimals[0].GetComponent<Collider>(), transform.GetComponent<Collider>());
            filteredAnimals[0].transform.position = transform.position;
            filteredAnimals[0].transform.rotation = transform.rotation;
            filteredAnimals[0].transform.parent = this.transform;
            GetComponent<MimicMovemenment>().m_AnimatorDriverAnimal = transform.GetChild(0).GetComponent<AnimatorDriverAnimal>();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            filteredAnimals = TransformGUI.GetComponent<TransformGUI>().m_transforms;
            DetachCurrentChild();
            Physics.IgnoreCollision(filteredAnimals[1].GetComponent<Collider>(), transform.GetComponent<Collider>());
            filteredAnimals[1].transform.position = transform.position;
            filteredAnimals[1].transform.rotation = transform.rotation;
            filteredAnimals[1].transform.parent = this.transform;
            GetComponent<MimicMovemenment>().m_AnimatorDriverAnimal = transform.GetChild(0).GetComponent<AnimatorDriverAnimal>();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            filteredAnimals = TransformGUI.GetComponent<TransformGUI>().m_transforms;
            DetachCurrentChild();
            Physics.IgnoreCollision(filteredAnimals[2].GetComponent<Collider>(), transform.GetComponent<Collider>());
            filteredAnimals[2].transform.position = transform.position;
            filteredAnimals[2].transform.rotation = transform.rotation;
            filteredAnimals[2].transform.parent = this.transform;
            GetComponent<MimicMovemenment>().m_AnimatorDriverAnimal = transform.GetChild(0).GetComponent<AnimatorDriverAnimal>();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            filteredAnimals = TransformGUI.GetComponent<TransformGUI>().m_transforms;
            DetachCurrentChild();
            Physics.IgnoreCollision(filteredAnimals[3].GetComponent<Collider>(), transform.GetComponent<Collider>());
            filteredAnimals[3].transform.position = transform.position;
            filteredAnimals[3].transform.rotation = transform.rotation;
            filteredAnimals[3].transform.parent = this.transform;
            GetComponent<MimicMovemenment>().m_AnimatorDriverAnimal = transform.GetChild(0).GetComponent<AnimatorDriverAnimal>();
        }
    }

    public void CorrectAI(GameObject[] AI)
    {
        foreach(GameObject animal in AI)
        {
            animal.GetComponent<AnimatorDriverAnimal>().enabled = false;
        }
    }

    void CorrectRotationAndPositionOnError()
    {
        if(transform.rotation != transform.GetChild(0).rotation || transform.position.x != transform.GetChild(0).position.x || transform.position.z != transform.GetChild(0).position.z)
        {
            Transform child = transform.GetChild(0);
            child.rotation = transform.rotation;
            child.position = new Vector3(transform.position.x, child.position.y, transform.position.z);
        }
    }
}

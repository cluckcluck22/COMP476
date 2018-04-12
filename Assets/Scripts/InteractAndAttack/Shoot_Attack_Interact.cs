﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot_Attack_Interact : MonoBehaviour {

    public LayerMask interactables;
    public LayerMask layerOfCreatures;
    public float fireRate = 0.35f;
    float nextFire;
    Transform cam;
    RaycastHit hit;

    MimicMovemenment mimicController;

    Interactable currentInteractObject;

	// Use this for initialization
	void Start () {
        if(gameObject.tag == "Player")
        {
            cam = gameObject.GetComponent<BasicBehaviour>().playerCamera;
        }

        mimicController = GetComponent<MimicMovemenment>();
        currentInteractObject = null;
    }
	
	// Update is called once per frame
	void Update () {
        InteractWithWorld();
        KillTarget();
    }

    void InteractWithWorld()
    {
        if (currentInteractObject != null && mimicController.isMoving())
        {
            currentInteractObject.detach(gameObject.GetComponent<AnimalAI>());
            currentInteractObject = null;
            return;
        }

        if (Input.GetKey(KeyCode.Mouse1) || Input.GetMouseButtonDown(0))
        {
            Collider[] interactbleObjects = Physics.OverlapSphere(transform.position, 10f, interactables);
            foreach(Collider interactable in interactbleObjects)
            {
                Interactable interactObject;
                interactObject = interactable.GetComponent<Interactable>();
                if (interactObject != null)
                {
                    if (gameObject.tag == "Player")
                    {
                        Debug.Log("interactable Object " + interactObject.gameObject.name);
                        Debug.Log("HEY THE: " + gameObject.name);
                        interactObject.fill(interactObject.maxCount);
                    }

                    else if (gameObject.tag == "mimic" && !interactObject.isEmpty())
                    {
                        Debug.Log("HEY THE: " + gameObject.name + " TAG: " + gameObject.tag + " ATTACH/EAT");
                        interactObject.attach(gameObject.GetComponent<AnimalAI>());
                        if (interactObject.type == Interactable.Type.Food)
                            mimicController.Eating();
                        else
                            mimicController.Rest();
                        currentInteractObject = interactObject;
                    }
                }

                return;
            }
        }
    }

    //currently implemented for mimic
    void KillTarget()
    {
        if (Input.GetKey(KeyCode.Mouse0) && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            if (gameObject.tag == "mimic")
            {
                Collider[] creatures = Physics.OverlapSphere(transform.position, 5f, layerOfCreatures);
                foreach (Collider creature in creatures)
                {
                    if (Physics.Raycast(transform.position, transform.forward, out hit, 5f, layerOfCreatures))
                    {
                        //call kill script
                        Debug.Log("killed " + hit.collider.gameObject.name);
                        hit.collider.gameObject.GetComponent<KillAnimal>().KillAnimals();
                    }
                }
            }
            else if (gameObject.tag == "Player")
            {
                //need to shoot ray from gun
                if(Input.GetKey(KeyCode.Mouse1))
                {
                    if (Input.GetKey(KeyCode.Mouse0))
                    {
                        Vector3 shotDirection = cam.forward;
                        if(Physics.Raycast(cam.transform.position, shotDirection, out hit, 50f, layerOfCreatures))
                        {
                            Debug.Log("killed " + hit.collider.gameObject.name);
                            hit.collider.gameObject.GetComponent<KillAnimal>().KillAnimals();
                        }
                    }
                }
            }
            
        }
    }
}

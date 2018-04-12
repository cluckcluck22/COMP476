using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot_Attack_Interact : MonoBehaviour {

    public LayerMask interactables;
    public LayerMask layerOfCreatures;
    public float fireRate = 0.35f;
    float nextFire;
    Transform cam;
    RaycastHit hit;
    
	// Use this for initialization
	void Start () {
        if(gameObject.tag == "Player")
        {
            cam = gameObject.GetComponent<BasicBehaviour>().playerCamera;
        }
        
    }
	
	// Update is called once per frame
	void Update () {
        InteractWithWorld();
        KillTarget();
    }

    void InteractWithWorld()
    {
        if (Input.GetKey(KeyCode.Mouse1)|| Input.GetMouseButtonDown(0))
        {
            Collider[] interactbleObjects = Physics.OverlapSphere(transform.position, 5f, interactables);
            foreach(Collider interactable in interactbleObjects)
            {
                Vector3 direction = interactable.transform.position - transform.position;
                //call interact script if mimic, if farmer, fill food and stuff...
                if (Physics.Raycast(transform.position, transform.forward, out hit, 5f, interactables))
                {
                    //call interact script
                    Debug.Log("interactable Object " + hit.collider.gameObject.name);
                        Debug.Log("HEY THE: " + gameObject.name + " TAG: " + gameObject.tag);
                        hit.collider.gameObject.GetComponent<Interactable>().fill(hit.collider.gameObject.GetComponent<Interactable>().maxCount);
                }
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

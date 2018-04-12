using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot_Attack_Interact : MonoBehaviour {

    public LayerMask interactables;
    public LayerMask layerOfCreatures;
    RaycastHit hit;
	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        InteractWithWorld();
        KillTarget();
    }

    void InteractWithWorld()
    {
        if (Input.GetKey(KeyCode.Mouse1))
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
                }
            }
        }
    }

    //currently implemented for mimic
    void KillTarget()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (gameObject.tag == "mimic")
            {
                Collider[] creatures = Physics.OverlapSphere(transform.position, 5f, layerOfCreatures);
                foreach (Collider creature in creatures)
                {
                    if (Physics.Raycast(transform.position, transform.forward, out hit, 5f, layerOfCreatures))
                    {
                        //call kill script
                        Debug.Log("killed " + hit.collider.gameObject.name);
                    }
                }
            }
            else if (gameObject.tag == "Player")
            {
                //need to shoot ray from gun
            }
            
        }
    }
}

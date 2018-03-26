using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class BaseAI : MonoBehaviour {

	//public float speed = 2.0f;
	//public float acceleration = 2.0f;
	//public float deceleration = 0.5f;
    //public float maxSpeed = 2.0f;
	
	public float interactThreshold = 0.02f;
	//public float velocityThreshold = 2.00f;

    public GameObject target;
    public UnityEngine.AI.NavMeshAgent agent;

    public int restStat;
    public int eatStat;
    public int playStat;

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            if ((target.transform.position - transform.position).magnitude > interactThreshold)
            {
                GoTo(target.transform.position);
            }

        }
    }
    
			
	///Abstract methods to be implemented in child classes 
	public abstract void Eat(GameObject eatTarget);
	
	public abstract void Rest(GameObject restTarget);
		
	public abstract void Play(GameObject playMate);
	
	public abstract void Speak();
	
	
	public void GoTo(Vector3 target) //something will have to call GoTo 
	{
        agent.destination = target;
        //Do we make calls to the animators here?
    }
	
	
	///Movement-specific methods
	/*public void Arrive(Vector3 target)
	{
		if(target == null) { return; }//in case we eventually use a gameObject as a target
		
		// if in range to interact with object stop moving //(note: this might cause a problem if we are not aligned with the object we want to interact with)
        if((target - transform.position).magnitude < interactThreshold) 
		{ 
			GetComponent<Rigidbody>().velocity = Vector3.zero;	
		}
		
		// if soon to be in range to interact, slow down
		else if ((target - transform.position).magnitude < velocityThreshold)
		{
			GetComponent<Rigidbody>().velocity -=((target - transform.position).normalized * deceleration);
		}
		
		//else keep seeking
		else{ Seek(target); }
	}*/
	
	/*public void Seek(Vector3 target)
	{
		//look towards target
		transform.rotation = Quaternion.LookRotation(target - transform.position);
		
		//accelerate towards target
		if (GetComponent<Rigidbody>().velocity.magnitude < maxSpeed)
		{
			GetComponent<Rigidbody>().velocity += ((target - transform.position).normalized * acceleration);
		}
		else
		{
			GetComponent<Rigidbody>().velocity = ((target - transform.position).normalized * maxSpeed);
		}
	}*/
}

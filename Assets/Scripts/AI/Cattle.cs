using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using BaseAI.cs;

public class Cattle : BaseAI {

    // Use this for initialization
    void Start ()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (target != null)
        {
            if ((target.transform.position - transform.position).magnitude > interactThreshold)
            {
                GoTo(target.transform.position);
            }

        }
    }

    public override void Eat(GameObject eatTarget) { }

    public override void Rest(GameObject restTarget) { }

    public override void Play(GameObject playMate) { }

    public override void Speak() { }
}

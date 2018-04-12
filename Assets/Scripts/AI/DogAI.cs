using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DogAI : MonoBehaviour {

    public Transform owner;
    public AudioSource audioSource;

    public Vector3 followOffset;
    public float repathDistance=4.0f;
    public float repathAngle;
    public float followDistanceReached;

    public float lookAtExitDistance;
    public float lookAtExitAngle;
    public float lookAtDelay;

    // PA - have you tried making this a bit larger ?
    // Since you are pathing towards the middle of the interactable object he might get stuck
    // on the outside of it and never be able to reach the destination
    public float sniffDistance = 0.25f;
    public float sniffTime=1.0f;

    public float goToFarmerDistance=2.0f;
    public float leadDistance=1.0f;

    public float barkFrequency=5.0f;

    private ZoneManager zoneManager;

    private Interactable[] needsObjects;

    private AnimatorDriverAnimal animator;
    private NavMeshAgent navAgent;

    private Vector3 latestTargetPos;
    private Vector3 latestTargetFront;

    private DogState state;
    private DogSubState subState;
    private bool shouldFollow;

    private Interactable needsFilling;
    private int currentNeedsObjectIndex;

    private float sniffTimeEnd;
    private float startLookAtTime;

    private float barkTime;

    private enum DogState
    {
        Follow,
        Patrol,
        Lead
    }

    private enum DogSubState
    {
        FollowPath,
        FollowLookAt,
        FollowWait,
        PatrolGotoInteractable,
        PatrolSniff,
        LeadPath,
        GoToOwner,
        LeadLookat,
        LeadFillUp,
    }

    private void Awake()
    {
        animator = GetComponent<AnimatorDriverAnimal>();
        navAgent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        startLookAtTime = float.MaxValue;
        sniffTimeEnd = float.MinValue;
        shouldFollow = false;
    }

    void Start () {
        zoneManager = FindObjectOfType<ZoneManager>();
        needsObjects = zoneManager.getAllNeedsInteractables().ToArray();

        OnFollowEnter();
    }

    void Update ()
    {
	    switch (state)
        {
            case DogState.Follow:
                Follow();
                break;
            case DogState.Lead:
                Lead();
                break;
            case DogState.Patrol:
                Patrol();
                break;
        }	
	}

    private void OnFollowEnter()
    {
        navAgent.isStopped = false;
        state = DogState.Follow;
        subState = DogSubState.FollowPath;
        startLookAtTime = float.MaxValue;
        animator.PlayWalk();
        Repath(followOffset);
    }

    private void Follow()
    {
        if (shouldFollow)
        {
            Vector3 ownerPosition = owner.transform.position;
            Vector3 ownerForward = owner.transform.forward;
            switch (subState)
            {
                case DogSubState.FollowPath:
                    {
                        if ((latestTargetPos - ownerPosition).magnitude >= repathDistance ||
                             Vector3.Angle(latestTargetFront, ownerForward) >= repathAngle)
                        {
                            Repath(followOffset);
                        }

                        else if (!navAgent.pathPending && navAgent.remainingDistance <= followDistanceReached)
                        {
                            animator.PlayFullBodyState(States.AnimalFullBody.Idle);
                            latestTargetFront = ownerForward;
                            latestTargetPos = ownerPosition;
                            startLookAtTime = Time.time + lookAtDelay;
                            subState = DogSubState.FollowWait;
                        }

                        if (Time.time >= startLookAtTime)
                        {
                            subState = DogSubState.FollowLookAt;
                        }
                        break;
                    }
                case DogSubState.FollowWait:
                    {
                        if (Time.time >= startLookAtTime)
                        {
                            subState = DogSubState.FollowLookAt;
                        }

                        if ((transform.position - ownerPosition).magnitude >= lookAtExitDistance ||
                             Vector3.Angle(latestTargetFront, ownerForward) >= lookAtExitAngle)
                        {
                            startLookAtTime = float.MaxValue;
                            Repath(followOffset);
                            animator.PlayWalk();
                            subState = DogSubState.FollowPath;
                        }

                        break;
                    }
                case DogSubState.FollowLookAt:
                    {
                        Vector3 targetDir = owner.transform.position - transform.position;
                        LookAtTarget(targetDir);

                        if ((transform.position - ownerPosition).magnitude >= lookAtExitDistance ||
                             Vector3.Angle(latestTargetFront, ownerForward) >= lookAtExitAngle)
                        {
                            startLookAtTime = float.MaxValue;
                            Repath(followOffset);
                            animator.PlayWalk();
                            subState = DogSubState.FollowPath;
                        }
                        break;
                    }
            }
        }
        else
        {
            OnFollowExit();
            OnPatrolEnter();
        }
        
    }

    private void OnFollowExit()
    {
        animator.PlayFullBodyState(States.AnimalFullBody.Idle);
    }

    private void OnPatrolEnter()
    {
        navAgent.isStopped = false;
        state = DogState.Patrol;
       subState = DogSubState.PatrolGotoInteractable;
       Vector3 targetPos = needsObjects[currentNeedsObjectIndex].transform.position;
       navAgent.SetDestination(targetPos);
       animator.PlayWalk();
    }

    private void Patrol()
    {
        if (!shouldFollow && needsFilling == null)
        {
            //Patrol to first object in the needs list
            //Once there sniff it (check if it is empty)
            //If it is empty, lead owner back to object
            //If it is not empty, patrol to next object in list
            switch (subState)
            {
                case DogSubState.PatrolGotoInteractable:
                    {
                        if(!navAgent.pathPending && navAgent.remainingDistance <= sniffDistance)
                        {
                            // PA - I've been doing this for animal that are interacting with the interactable objects
                            /*
                            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                            */
                            // They always seem to spin/drag around when they are not told what to do (bastards)
                            subState = DogSubState.PatrolSniff; //dog checks out object  
                            navAgent.isStopped = true;
                            animator.CycleWalk();//called once to get to sniff animation
                            sniffTimeEnd = Time.time + sniffTime;
                        }
                        break;
                    }

                case DogSubState.PatrolSniff: //dog checks out object
                    {
                        if (Time.time >= sniffTimeEnd)
                        {
                            // PA - if you're doing the freeze thing...
                            /*
                            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                            */
                            if (needsObjects[currentNeedsObjectIndex].isEmpty())
                            {
                                needsFilling = needsObjects[currentNeedsObjectIndex]; //if empty save reference to lead owner back to it
                                OnLeadEnter();
                                state = DogState.Lead;
                                subState = DogSubState.GoToOwner;
                            }
                            else
                            {
                                currentNeedsObjectIndex++;
                                Vector3 targetPos = needsObjects[currentNeedsObjectIndex].transform.position;
                                navAgent.SetDestination(targetPos);
                                subState = DogSubState.PatrolGotoInteractable; //dog will go to next object                            
                            }
                            animator.CycleWalk();//called twice to get back to original walk animation
                            animator.CycleWalk();
                            navAgent.isStopped = false;
                        }

                        break;
                    }               
            }
        }
        else
        {
            OnPatrolExit();
            OnLeadEnter();
        }
    }

    private void OnPatrolExit()
    {
        animator.PlayFullBodyState(States.AnimalFullBody.Idle);
        needsFilling = null;
    }

    private void OnLeadEnter()
    {
        navAgent.isStopped = false;
        state = DogState.Lead;
        subState = DogSubState.GoToOwner;
        startLookAtTime = float.MaxValue;
        animator.PlayWalk();
        // PA - more on this a bit later
        /*
        Vector3 direction = (needsFilling.transform.position - owner.position).normalized;
        direction *= goToFarmerDistance;
        Repath(direction);
        */
    }

    private void Lead()
    {
        if (!shouldFollow && needsFilling != null)            
        {
            //Go to owner
            //Get owner to follow
            //Lead them back to object that needs filling
            //Once having successfully lead the farmer and watched them fillUp, continue patrolling

            Vector3 ownerPosition = owner.transform.position;
            switch (subState)
            {
                case DogSubState.GoToOwner:
                    {
                        if ((transform.position - ownerPosition).magnitude <= goToFarmerDistance)
                        {
                            subState = DogSubState.LeadLookat;
                        }
                        else /*if ( (ownerPosition - latestTargetPos).magnitude >= repathDistance )*/
                        {
                            //close distance with owner
                            // PA - here you only need to repath if the owner moves to far away from
                            // its last position. When you call repath, you are actually setting a target
                            // pos to the navMesh agent (no need to do this everyframe, it's super expensive)
                            // So you call repath once (onEnter), then update it if the owner moves too far away.
                            // You can use the same repath distance from the follow (it's sensible I think).
                            Vector3 direction = (needsFilling.transform.position - owner.position).normalized;
                            direction *= goToFarmerDistance;
                            Repath(direction);
                        }
                       
                        break;
                    }
                case DogSubState.LeadLookat:
                    {
                        if ((transform.position - ownerPosition).magnitude > repathDistance)
                        {
                            subState = DogSubState.GoToOwner;
                            // PA - Would probably get caught by the GoToOwner substate, but this is safer
                            /*
                            Vector3 direction = (needsFilling.transform.position - owner.position).normalized;
                            direction *= goToFarmerDistance;
                            Repath(direction);
                            */
                        }
                        else if ((transform.position - ownerPosition).magnitude <= leadDistance)
                        {
                            subState = DogSubState.LeadPath;
                            // PA - Here you only need to repath to the interactable that needs filling
                            // If all goes well you do it once and the farmer will follow the dog all the way
                            /*
                            Repath(needsFilling.transform.position); 
                            */ 
                        }
                        else
                        {
                            Vector3 targetDir = owner.transform.position - transform.position;
                            LookAtTarget(targetDir);
                            Bark();//TODO
                        }

                        break;
                    }

                case DogSubState.LeadPath:
                    {
                        if ((transform.position - needsFilling.transform.position).magnitude < sniffDistance)
                        {
                            subState = DogSubState.LeadFillUp;
                        }
                        else if ((transform.position - ownerPosition).magnitude > goToFarmerDistance)
                        {
                            subState = DogSubState.LeadLookat;
                        }
                        //else
                        //{
                        //    // PA - If all goes well, we're just walking to it, no need to repath
                        //    Vector3 direction = (this.transform.position - needsFilling.transform.position).normalized;
                        //    direction *= sniffDistance;//small offset
                        //    Repath(direction);
                        //}
                        
                        break;
                    }
                case DogSubState.LeadFillUp:
                    {
                        if ((transform.position - ownerPosition).magnitude > goToFarmerDistance)
                        {
                            subState = DogSubState.LeadLookat;
                        }
                        else 
                        {
                            if (needsFilling.isEmpty())
                            {
                                Vector3 targetDir = needsFilling.transform.position - transform.position;
                                LookAtTarget(targetDir);
                            }
                            else
                            {
                                //Keep patrolling
                                OnLeadExit();
                                OnPatrolEnter();
                            }
                        }
                        
                        break;
                    }
            }
        }
        else
        {
            OnLeadExit();
            OnFollowEnter();
        }
    }

    private void OnLeadExit()
    {
        animator.PlayFullBodyState(States.AnimalFullBody.Idle);
        needsFilling = null;
    }

    private void Repath(Vector3 offset)
    {
        latestTargetPos = owner.transform.position;
        latestTargetFront = owner.transform.forward;
        navAgent.SetDestination(latestTargetPos + offset);
    }

    private void Bark()
    {
        if (Time.time > barkTime)
        {
            barkTime = Time.time + barkFrequency;
            audioSource.Play();            
        }
    }

    private void LookAtTarget(Vector3 tDir)
    {
        Vector3 targetDir = tDir.normalized;

        float dot = Vector3.Dot(transform.forward, targetDir);
        if (dot <= 0.99f)
        {
            targetDir = Vector3.Normalize(targetDir);
            float step = navAgent.angularSpeed * Time.deltaTime;
            step = step / 180.0f * Mathf.PI;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0f);
            transform.rotation = Quaternion.LookRotation(newDir);
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(targetDir);
        }
    }

    public void TellDogToFollow()
    {
        shouldFollow = true;
    }

    public void TellDogToPatrol()
    {
        shouldFollow = false;
    }


}

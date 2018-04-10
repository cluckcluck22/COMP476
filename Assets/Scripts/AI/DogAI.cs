using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DogAI : MonoBehaviour {

    public Transform owner;

    public Vector3 followOffset;
    public float repathDistance;
    public float repathAngle;
    public float followDistanceReached;

    public float lookAtExitDistance;
    public float lookAtExitAngle;
    public float lookAtDelay;

    private ZoneManager zoneManager;

    private Interactable[] needsObjects;

    private AnimatorDriverAnimal animator;
    private NavMeshAgent navAgent;

    private Vector3 latestTargetPos;
    private Vector3 latestTargetFront;

    private DogState state;
    private DogSubState subState;

    private Interactable needsFilling;

    private float startLookAtTime;

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
        LeadLookat
    }

    private void Awake()
    {
        animator = GetComponent<AnimatorDriverAnimal>();
        navAgent = GetComponent<NavMeshAgent>();
        startLookAtTime = float.MaxValue;
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
        state = DogState.Follow;
        subState = DogSubState.FollowPath;
        startLookAtTime = float.MaxValue;
        animator.PlayWalk();
        Repath();
    }

    private void Follow()
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
                        Repath();
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
                        Repath();
                        animator.PlayWalk();
                        subState = DogSubState.FollowPath;
                    }

                    break;
                }
            case DogSubState.FollowLookAt:
                {
                    Vector3 targetDir = ownerPosition - transform.position;
                    targetDir = targetDir.normalized;

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

                    if ( (transform.position - ownerPosition).magnitude >= lookAtExitDistance ||
                         Vector3.Angle(latestTargetFront, ownerForward) >= lookAtExitAngle)
                    {
                        startLookAtTime = float.MaxValue;
                        Repath();
                        animator.PlayWalk();
                        subState = DogSubState.FollowPath;
                    }
                    break;
                }
        }
    }

    private void OnFollowExit()
    {
        animator.PlayFullBodyState(States.AnimalFullBody.Idle);
    }

    private void OnPatrolEnter()
    {

    }

    private void Patrol()
    {

    }

    private void OnPatrolExit()
    {

    }

    private void OnLeadEnter()
    {

    }

    private void Lead()
    {

    }

    private void OnLeadExit()
    {

    }

    private void Repath()
    {
        latestTargetPos = owner.transform.position;
        latestTargetFront = owner.transform.forward;
        navAgent.SetDestination(latestTargetPos + followOffset);
    }
}

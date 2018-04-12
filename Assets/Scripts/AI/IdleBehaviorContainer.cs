using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

using Coroutine = System.Collections.IEnumerator;
using BTCoroutine = System.Collections.Generic.IEnumerator<BTNodeResult>;

public class IdleBehaviorContainer : MonoBehaviour
{
    public BehaviorTree bt { get; private set; }

    public Interactable interactable { get; private set; }

    public AnimalAI user { get; private set; }


    private void Awake()
    {
        if (PhotonNetwork.isMasterClient || !PhotonNetwork.connected)
        {
            user = GetComponent<AnimalAI>();
            bt = new BehaviorTree(user.animalConfig.idleTree, this);
        }
        else
        {
            Destroy(this);
        }
    }

    public void setContext(Interactable interactable)
    {
        this.interactable = interactable;
    }

    public BTNode getRoot()
    {
        return bt.rootNode;
    }

    [BTLeaf("dynamic-goto")]
    public BTCoroutine dynamicGoto(Stopper stopper)
    {
        if (interactable == null)
        {
            yield return BTNodeResult.Failure;
            yield break;
        }

        user.navAgent.isStopped = false;
        user.animatorDriver.PlayWalk();
        user.navAgent.destination = interactable.transform.position;
        interactable.reserve(user);

        while (true)
        {
            if (stopper.shouldStop)
            {
                user.navAgent.isStopped = true;
                stopper.shouldStop = false;
                user.animatorDriver.PlayFullBodyState(States.AnimalFullBody.Idle);
                yield return BTNodeResult.Stopped;
                yield break;
            }

            if (dynamicReached())
            {
                user.animatorDriver.PlayFullBodyState(States.AnimalFullBody.Idle);
                yield return BTNodeResult.Success;
                yield break;
            }

            yield return BTNodeResult.Running;
        }
    }

    [BTLeaf("dynamic-reached")]
    public bool dynamicReached()
    {
        return !user.navAgent.pathPending && user.navAgent.remainingDistance <= user.navAgent.stoppingDistance;
    }

    [BTLeaf("dynamic-action")]
    public BTCoroutine dynamicAction(Stopper stopper)
    {

        Physics.IgnoreCollision(interactable.GetComponent<BoxCollider>(), GetComponent<BoxCollider>());
        interactable.attach(user);

        Vector3 targetDir = interactable.getInteractionPos(user).transform.forward;

        targetDir = Vector3.Normalize(targetDir);
        float step = user.navAgent.angularSpeed * Time.deltaTime;
        step = step / 180.0f * Mathf.PI;


        while (true)
        {
            if (stopper.shouldStop)
            {
                stopper.shouldStop = false;
                interactable.detach(user);
                Physics.IgnoreCollision(interactable.GetComponent<BoxCollider>(), GetComponent<BoxCollider>(), false);
                user.animatorDriver.PlayFullBodyState(States.AnimalFullBody.Idle);
                yield return BTNodeResult.Stopped;
                yield break;
            }

            // Rotate until we are "close enough"
            float dot = Vector3.Dot(transform.forward, targetDir);
            if (dot <= 0.99f)
            {
                Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0f);
                transform.rotation = Quaternion.LookRotation(newDir);
                yield return BTNodeResult.Running;
            }
            else
            {
                transform.rotation = Quaternion.LookRotation(targetDir);
                break;
            }
        }

        switch (interactable.type)
        {
            case Interactable.Type.Chill:
            case Interactable.Type.Talk:
                user.animatorDriver.PlayFullBodyState(States.AnimalFullBody.Rest);
                break;
        }

        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

        while (true)
        {
            if (stopper.shouldStop)
            {
                stopper.shouldStop = false;
                interactable.detach(user);
                Physics.IgnoreCollision(interactable.GetComponent<BoxCollider>(), GetComponent<BoxCollider>(), false);
                user.animatorDriver.PlayFullBodyState(States.AnimalFullBody.Idle);
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                yield return BTNodeResult.Stopped;
                yield break;
            }

            yield return BTNodeResult.Running;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using BTCoroutine = System.Collections.Generic.IEnumerator<BTNodeResult>;

public class AnimalAI : MonoBehaviour {

    public AnimalConfig animalConfig;
    public string animalName;
    public bool debugNav;
    public bool debugPerception;
    public bool debugBT;

    private string runningBT = "";

    private PerceptionModule perception;

    private AnimatorDriverAnimal animatorDriver;

    private NavMeshAgent navAgent;

    private Dictionary<string, object> blackboard;

    private BehaviorTree bt { get; set; }

    public float hunger { get; private set; }
    public float fatigue { get; private set; }
    public float health { get; private set; }

    private IEnumerator perceptionRoutine;

    private ZoneManager zoneManager;

    private void Awake()
    {
        bt = new BehaviorTree(animalConfig.xmlTree, this);
        debugBT = debugNav = debugPerception = true;

        zoneManager = FindObjectOfType<ZoneManager>();
    }

    void Start ()
    {
        float randomRatio;

        randomRatio = Random.Range(animalConfig.randomMinRatio, animalConfig.randomMaxRatio);
        hunger = animalConfig.maxHunger * randomRatio;

        randomRatio = Random.Range(animalConfig.randomMinRatio, animalConfig.randomMaxRatio);
        fatigue = animalConfig.maxFatigue * randomRatio;

        health = animalConfig.maxHealth;
        perception = new PerceptionModule(this);
        animatorDriver = GetComponent<AnimatorDriverAnimal>();
        navAgent = GetComponent<NavMeshAgent>();
        perceptionRoutine = PerceptionUpdater(0.3f, 0.6f);
        StartCoroutine(perceptionRoutine);
        bt.Start();
        blackboard = new Dictionary<string, object>();
	}
	
	void Update ()
    {
        if (health != 0)
        {
            hunger -= animalConfig.hungerLossRate * Time.deltaTime;
            hunger = Mathf.Max(hunger, 0.0f);
            fatigue -= animalConfig.fatigueLossRate * Time.deltaTime;
            fatigue = Mathf.Max(fatigue, 0.0f);
        }
	}

    private void OnDrawGizmosSelected()
    {
        if (debugPerception)
            perception.drawPerception();
        if (debugNav)
            drawPath(navAgent.path);
        if (debugBT)
            print(runningBT);
    }

    private IEnumerator PerceptionUpdater(float minWait, float maxWait)
    {
        while (true)
        {
            float waitTime = UnityEngine.Random.Range(minWait, maxWait);
            perception.updateInteractables();
            perception.updateEntries();
            perception.updateAllLkp();
            perception.updateFlockCenter();
            yield return new WaitForSeconds(waitTime);
        }
    }

    [BTLeaf("do-nothing")]
    public bool doNothing()
    {
        return true;
    }

    [BTLeaf("isHungry")]
    public bool isHungry(Scorer scorer)
    {
        scorer.score = animalConfig.hungerNeedCurve.Evaluate(hunger / animalConfig.maxHunger);
        return true;
    }
    [BTLeaf("isTired")]
    public bool isTired(Scorer scorer)
    {
        scorer.score = animalConfig.fatigueNeedCurve.Evaluate(fatigue / animalConfig.maxFatigue);
        return true;
    }
    [BTLeaf("isDead")]
    public bool isDead(Scorer scorer)
    {
        scorer.score = 2.0f;
        return isDead();
    }
    [BTLeaf("isFar")]
    public bool isFar(Scorer scorer)
    {
        if (perception.hasNoFlock)
            return false;

        float distance = (getPosition() - perception.flockCenter).magnitude;
        scorer.score = animalConfig.affinityProximityNeedCurve.Evaluate(distance);
        return true;
    }
    [BTLeaf("should-idle")]
    public bool shouldIdle(Scorer scorer)
    {
        scorer.score = animalConfig.idleScore;
        return true;
    }
    [BTLeaf("destination-reached")]
    public bool destinationReached()
    {
        return !navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance;
    }
    [BTLeaf("goto-food-area")]
    public BTCoroutine gotoFoodArea(Stopper stopper)
    {
        runningBT = "goto-food-area";

        BTCoroutine routine = gotoImplementation(stopper, zoneManager.findInteractableZone(this, Interactable.Type.Food).position);
        return routine;
    }

    [BTLeaf("goto-rest-area")]
    public BTCoroutine gotoRestArea(Stopper stopper)
    {
        runningBT = "goto-rest-area";

        BTCoroutine routine = gotoImplementation(stopper, zoneManager.findInteractableZone(this, Interactable.Type.Rest).position);
        return routine;
    }

    [BTLeaf("goto-food-item")]
    public BTCoroutine gotoFoodItem(Stopper stopper)
    {
        runningBT = "goto-food-item";

        Vector3 destination = ((Interactable)blackboard["InteractableTarget"]).getInteractionPos(this).position;

        BTCoroutine routine = gotoImplementation(stopper, destination);
        return routine;
    }

    [BTLeaf("goto-rest-item")]
    public BTCoroutine gotoRestItem(Stopper stopper)
    {
        runningBT = "goto-rest-item";

        Vector3 destination = ((Interactable)blackboard["InteractableTarget"]).getInteractionPos(this).position;

        BTCoroutine routine = gotoImplementation(stopper, destination);
        return routine;
    }

    [BTLeaf("food-found")]
    public bool isFoodAvailable()
    {
        return itemFoundImplementation(Interactable.Type.Food);
    }

    [BTLeaf("rest-found")]
    public bool isRestAvailable()
    {
        return itemFoundImplementation(Interactable.Type.Rest);
    }

    [BTLeaf("eat")]
    public BTCoroutine eat(Stopper stopper)
    {
        runningBT = "eat";

        Interactable target = (Interactable)blackboard["InteractableTarget"];
        blackboard.Remove("InteractableTarget");

        BTCoroutine routine = consumeImplementation(stopper, target);
        return routine;
    }

    [BTLeaf("rest")]
    public BTCoroutine rest(Stopper stopper)
    {
        runningBT = "rest";

        Interactable target = (Interactable)blackboard["InteractableTarget"];
        blackboard.Remove("InteractableTarget");

        BTCoroutine routine = consumeImplementation(stopper, target);
        return routine;
    }

    [BTLeaf("idle-wander")]
    public BTCoroutine idleWander(Stopper stopper)
    {
        runningBT = "idle-wander";

        navAgent.isStopped = true;

        while (true)
        {
            if (stopper.shouldStop)
            {
                navAgent.isStopped = false;
                stopper.shouldStop = false;
                yield return BTNodeResult.Stopped;
                yield break;
            }

            yield return BTNodeResult.Running;
        }

        //float angle = Random.Range(-35.0f, 35.0f);
        //float distance = Random.Range(3.0f, 7.0f);
        //Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;
        //Vector3 target = transform.position + direction;

        //return gotoImplementation(stopper, target);
    }


    private BTCoroutine consumeImplementation(Stopper stopper, Interactable target)
    {
        Physics.IgnoreCollision(target.GetComponent<BoxCollider>(), GetComponent<BoxCollider>());
        target.attach(this);
        switch (target.type)
        {
            case Interactable.Type.Food:
                animatorDriver.PlayFullBodyState(States.AnimalFullBody.Eat);
                break;
            case Interactable.Type.Rest:
                animatorDriver.PlayFullBodyState(States.AnimalFullBody.Rest);
                break;
        }

        Vector3 targetDir = target.getInteractionPos(this).transform.forward;

        targetDir = Vector3.Normalize(targetDir);
        float step = navAgent.angularSpeed * Time.deltaTime;
        step = step / 180.0f * Mathf.PI;

        // The animal does not have to do anything beyond this, the container "feeds" the animal
        // on update (like an observer pattern)
        while (true)
        {
            // Rotate until we are "close enough"
            float dot = Vector3.Dot(transform.forward, targetDir);
            if (dot <= 0.99f)
            {
                Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0f);
            }
            else
            {
                transform.rotation = Quaternion.LookRotation(targetDir);
            }


            if (stopper.shouldStop)
            {
                stopper.shouldStop = false;
                target.detach(this);
                Physics.IgnoreCollision(target.GetComponent<BoxCollider>(), GetComponent<BoxCollider>(), false);
                animatorDriver.PlayFullBodyState(States.AnimalFullBody.Idle);
                yield return BTNodeResult.Stopped;
                yield break;
            }

            if (target.isEmpty())
            {
                target.detach(this);
                animatorDriver.PlayFullBodyState(States.AnimalFullBody.Idle);
                yield return BTNodeResult.Failure;
                yield break;
            }

            yield return BTNodeResult.Running;
        }
    }

    private BTCoroutine gotoImplementation(Stopper stopper, Vector3 target)
    {
        navAgent.isStopped = false;
        animatorDriver.PlayWalk();
        navAgent.destination = target;
        while (true)
        {
            if (stopper.shouldStop)
            {
                if (blackboard.ContainsKey("InteractableTarget"))
                {
                    Interactable toRemove = (Interactable)blackboard["InteractableTarget"];
                    toRemove.unReserve(this);
                    blackboard.Remove("InteractableTarget");
                }
                navAgent.isStopped = true;
                stopper.shouldStop = false;
                animatorDriver.PlayFullBodyState(States.AnimalFullBody.Idle);
                yield return BTNodeResult.Stopped;
                yield break;
            }

            if (destinationReached())
            {
                animatorDriver.PlayFullBodyState(States.AnimalFullBody.Idle);
                yield return BTNodeResult.Success;
                yield break;
            }

            yield return BTNodeResult.Running;
        }
    }

    private void drawPath(NavMeshPath path)
    {
        for (int i = 0; i < path.corners.Length - 1; i++)
            Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
    }

    private bool itemFoundImplementation(Interactable.Type desiredType)
    {
        if (blackboard.ContainsKey("InteractableTarget"))
            return true;

        foreach (Interactable interactable in perception.interactablesInRange)
        {
            if (interactable.type == desiredType && interactable.isAvailable())
            {
                blackboard["InteractableTarget"] = interactable;
                interactable.reserve(this);
                return true;
            }
        }

        return false;
    }

    public bool isDead()
    {
        if (Mathf.Approximately(0.0f, health))
        {
            return true;
        }
        return false;
    }

    public Vector3 getPosition()
    {
        return transform.position;
    }

    public Vector3 getHeadPosition()
    {
        Vector3 output = transform.position;
        output.y += animalConfig.headOffset.y;
        output += transform.forward * animalConfig.headOffset.z;

        return output;
    }

    public Species getSpecies()
    {
        return animalConfig.species;
    }

    public float getFov()
    {
        return animalConfig.fov;
    }

    public float getSighDistance()
    {
        return animalConfig.sightDistance;
    }

    public float getLkpSightDistance()
    {
        return animalConfig.lkpSightDistance;
    }

    public float getLkpExpire()
    {
        return animalConfig.lkpExpire;
    }

    public float getConsumption(Interactable.Type type)
    {
        switch (type)
        {
            case Interactable.Type.Food:
                return animalConfig.hungerRecuperation * Time.deltaTime;
            case Interactable.Type.Rest:
                return animalConfig.fatigueRecuperation * Time.deltaTime;
            default:
                return 0.0f;
        }
    }

    public void giveFood(float amount)
    {
        hunger += amount;
        hunger = Mathf.Min(hunger, animalConfig.maxHunger);
    }

    public void giveRest(float amount)
    {
        fatigue += amount;
        fatigue = Mathf.Min(fatigue, animalConfig.maxFatigue);
    }
}

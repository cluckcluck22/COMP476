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

    public PerceptionModule perception { get; private set; }

    public AnimatorDriverAnimal animatorDriver { get; private set; }

    public NavMeshAgent navAgent { get; private set; }

    private Dictionary<string, object> blackboard;

    private BehaviorTree bt { get; set; }

    public float hunger { get;  set; }
    public float fatigue { get;  set; }
    public float health { get; private set; }

    private IEnumerator perceptionRoutine;

    private ZoneManager zoneManager;

    private IdleBehaviorContainer idleBehavior;

    public bool makeHungry = false;
    public bool makeSleepy = false;

    public bool isMimic = false;

    public void kill()
    {
        health = 0f;
    }

    private void Awake()
    {
        bt = new BehaviorTree(animalConfig.xmlTree, this);
        debugBT = debugNav = debugPerception = true;

        zoneManager = FindObjectOfType<ZoneManager>();

        idleBehavior = GetComponent<IdleBehaviorContainer>();
    }

    void Start ()
    {
        if (PhotonNetwork.isMasterClient || !PhotonNetwork.connected)
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
            if (!isMimic)
                bt.Start();
            blackboard = new Dictionary<string, object>();
        }
        else
        {
            Destroy(this);
        }
	}
	
	void Update ()
    {
        if (makeHungry)
        {
            hunger = 0f;
            makeHungry = false;
        }
        if (makeSleepy)
        {
            fatigue = 0f;
            makeSleepy = false;
        }

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

    [BTLeaf("play-dead-anim")]
    public bool playDeadAnim()
    {
        animatorDriver.PlayFullBodyState(States.AnimalFullBody.Dead);
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        return true;
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
    [BTLeaf("should-goto-rally")]
    public bool shouldGoToRally(Scorer scorer)
    {
        scorer.score = animalConfig.rallyScore;

        InteractableZone zone = zoneManager.findMyZone(this);
        if (zone != null && zone.type == InteractableZone.ZoneType.Rally)
            return false;

        Scorer dummy = new Scorer();
        return !isFar(dummy) || perception.hasNoFlock;
    }

    [BTLeaf("should-idle")]
    public bool shouldIdle(Scorer scorer)
    {
        scorer.score = animalConfig.idleScore;
        return zoneManager.requestIdleInteractable(this) != null;
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

    [BTLeaf("goto-flock-center")]
    public BTCoroutine gotFlockCenter(Stopper stopper)
    {
        runningBT = "goto-flock-center";

        Vector3 destination = perception.flockCenter;

        BTCoroutine routine = gotoImplementation(stopper, destination);
        return routine;
    }

    [BTLeaf("goto-nearest-rally")]
    public BTCoroutine gotoNearestRally(Stopper stopper)
    {
        runningBT = "goto-nearest-rally";

        Vector3 destination = zoneManager.findRallyZone(this).position;

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

        Interactable interactable = zoneManager.requestIdleInteractable(this); 

        if (interactable == null)
        {
 
        }

        idleBehavior.setContext(interactable);

        BTNode subRoot = idleBehavior.getRoot();

        subRoot.overrwriteStopper(stopper);

        BTCoroutine routine = subRoot.Procedure();
        return routine;
    }


    private BTCoroutine consumeImplementation(Stopper stopper, Interactable target)
    {
        target.attach(this);

        Vector3 targetDir = target.getInteractionPos(this).transform.forward;

        targetDir = Vector3.Normalize(targetDir);
        float step = navAgent.angularSpeed * Time.deltaTime;
        step = step / 180.0f * Mathf.PI;


        // The animal does not have to do anything beyond this, the container "feeds" the animal
        // on update (like an observer pattern)
        while (true)
        {
            if (stopper.shouldStop)
            {
                stopper.shouldStop = false;
                target.detach(this);
                Physics.IgnoreCollision(target.GetComponent<BoxCollider>(), GetComponent<BoxCollider>(), false);
                animatorDriver.PlayFullBodyState(States.AnimalFullBody.Idle);
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

        switch (target.type)
        {
            case Interactable.Type.Food:
                animatorDriver.PlayFullBodyState(States.AnimalFullBody.Eat);
                break;
            case Interactable.Type.Rest:
                animatorDriver.PlayFullBodyState(States.AnimalFullBody.Rest);
                break;
        }

        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

        while (true)
        { 
            if (stopper.shouldStop)
            {
                stopper.shouldStop = false;
                target.detach(this);
                Physics.IgnoreCollision(target.GetComponent<BoxCollider>(), GetComponent<BoxCollider>(), false);
                animatorDriver.PlayFullBodyState(States.AnimalFullBody.Idle);
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
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
        navAgent.destination = target;
        Rigidbody rb = GetComponent<Rigidbody>();
        while (true)
        {
            if (navAgent.hasPath)
            {
                animatorDriver.PlayWalk();
                if (rb.constraints == RigidbodyConstraints.FreezeAll)
                    rb.constraints = RigidbodyConstraints.None;
            }
            else
            {
                animatorDriver.PlayFullBodyState(States.AnimalFullBody.Idle);
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
            if (stopper.shouldStop)
            {
                if (blackboard.ContainsKey("InteractableTarget"))
                {
                    Interactable toRemove = (Interactable)blackboard["InteractableTarget"];
                    Physics.IgnoreCollision(toRemove.GetComponent<BoxCollider>(), GetComponent<BoxCollider>(), false);
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

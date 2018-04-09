using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Coroutine = System.Collections.IEnumerator;
using BTCoroutine = System.Collections.Generic.IEnumerator<BTNodeResult>;


public class Tester : MonoBehaviour {

    private BehaviorTree bt { get; set; }

    private int first { get; set; }
    private int second { get; set; }

    public int reach;
    public int max;

    private bool btStarted = false;

    private void Awake()
    {
        first = 0;
        second = 0;
        //bt = new BehaviorTree(Application.dataPath + "/Scripts/BTEngine/tester-behavior.xml", this);
        bt.Start();
    }

    void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}

    [BTLeaf("first-reached")]
    public bool isFirstReached(Scorer scorer)
    {
        print("Enter firstReached");
        scorer.score = 10;
        return first >= reach;
    }

    [BTLeaf("second-reached")]
    public bool isSecondReached(Scorer scorer)
    {
        print("Enter secondReached");
        scorer.score = 20;
        return second >= reach;
    }


    [BTLeaf("inc-first")]
    public BTCoroutine incFirstRun(Stopper stopper)
    {
        while(true)
        {
            print("Enter incFirst");
            if (stopper.shouldStop)
            {
                print("incFirst Stopped!");
                stopper.shouldStop = false;
                yield return BTNodeResult.Stopped;
                yield break;
            }
            if (first < max)
            {
                first++;
                yield return BTNodeResult.Running;
            }
            else if (first == max)
            {
                yield return BTNodeResult.Success;
                yield break;
            }
            else
            {
                // We should never reach this part
            }
        }
    }

    [BTLeaf("inc-second")]
    public BTCoroutine incSecondRun(Stopper stopper)
    {
        while(true)
        {
            print("Enter incSecond");
            if (stopper.shouldStop)
            {
                print("incSecond Stopped!");
                stopper.shouldStop = false;
                yield return BTNodeResult.Stopped;
                yield break;
            }
            if (second < max)
            {
                second++;
                yield return BTNodeResult.Running;
            }
            else if (second == max)
            {
                yield return BTNodeResult.Success;
                yield break;
            }
            else
            {
                // We should never reach this part
            }
        }
    }


    [BTLeaf("reset-first")]
    public bool resetFirst()
    {
        print("Enter resetFirst");
        first = 0;
        return true;
    }

    [BTLeaf("reset-second")]
    public bool resetSecond()
    {
        print("Enter resetSecond");
        second = 0;
        return true;
    }

    [BTLeaf("true")]
    public bool alwaysTrue(Scorer scorer)
    {
        print("Always true entered");
        scorer.score = 1;
        return true;
    }

}

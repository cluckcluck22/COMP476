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
        bt = new BehaviorTree(Application.dataPath + "/Scripts/BTEngine/tester-behavior.xml", this);
        bt.Start();
    }

    void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}

    [BTLeaf("is-first-reached")]
    public bool isFirstReached()
    {
        print("Enter isFirstReached");
        return first >= reach;
    }

    [BTLeaf("is-second-reached")]
    public bool isSecondReached()
    {
        print("Enter isSecondReached");
        return second >= reach;
    }

    [BTLeaf("inc-first")]
    public BTCoroutine incFirst()
    {
        if (first < max)
        {
            first++;
            yield return BTNodeResult.Success;
            yield break;
        }
        else
        {
            yield return BTNodeResult.Failure;
            yield break; 
        }
    }

    [BTLeaf("inc-second")]
    public BTCoroutine incSecond()
    {
        if (second < max)
        {
            second++;
            yield return BTNodeResult.Success;
            yield break;
        }
        else
        {
            yield return BTNodeResult.Failure;
            yield break;
        }
    }

    [BTLeaf("inc-first-run")]
    public BTCoroutine incFirstRun()
    {
        while(true)
        {
            print("Enter incFirstRun");
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

    [BTLeaf("inc-second-run")]
    public BTCoroutine incSecondRun()
    {
        while(true)
        {
            print("Enter incSecondRun");
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

    [BTLeaf("ping")]
    public BTCoroutine ping()
    {
        print("PING");
        yield return BTNodeResult.Success;
        yield break;
    }

    [BTLeaf("reset-first")]
    public BTCoroutine resetFirst()
    {
        print("Enter resetFirst");
        first = 0;
        yield return BTNodeResult.Success;
        yield break;
    }

    [BTLeaf("reset-second")]
    public BTCoroutine resetSecond()
    {
        print("Enter resetSecond");
        second = 0;
        yield return BTNodeResult.Success;
        yield break;
    }

}

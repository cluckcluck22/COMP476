using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

using Random = UnityEngine.Random;
using Coroutine = System.Collections.IEnumerator;
using BTCoroutine = System.Collections.Generic.IEnumerator<BTNodeResult>;

public class BTParallel : BTNode
{
    private BTNode[] subNodes;
    private BTParallelTypes type;

    public BTParallel(BTParallelTypes type, IEnumerable<BTNode> subNodes)
    {
        this.subNodes = subNodes.ToArray();
        this.type = type;
    }

    public BTParallel(BTParallelTypes type, params BTNode[] subNodes)
    {
        this.subNodes = subNodes;
        this.type = type;
    }

    public override BTCoroutine Procedure()
    {
        List<BTCoroutine> runningCoroutines = new List<BTCoroutine>();
        List<BTNode> runningNodes = new List<BTNode>();

        foreach (BTNode node in subNodes)
        {
            // First do one pass, then only iterate over 'running' nodes if needed
            BTCoroutine routine = node.Procedure();
            routine.MoveNext();
            BTNodeResult result = routine.Current;

            if (result == BTNodeResult.Success)
            {
                if (type == BTParallelTypes.FirstReturn ||
                    type == BTParallelTypes.FirstSuccess)
                {
                    yield return BTNodeResult.Success;
                    yield break;
                }
            }
            else if (result == BTNodeResult.Failure)
            {
                if (type == BTParallelTypes.FirstReturn ||
                    type == BTParallelTypes.FirstFailure)
                {
                    yield return BTNodeResult.Failure;
                    yield break;
                }
            }
            else if (result == BTNodeResult.Running)
            {
                runningCoroutines.Add(routine);
                runningNodes.Add(node);
            }
        }

        BTNodeResult returnValue = BTNodeResult.Running;

        while (runningCoroutines.Count() != 0)
        {
            // Keep running as long as there are running child nodes (even if they are supposed to stop)
            yield return BTNodeResult.Running;

            // On second pass iterate over running nodes
            // It is assumed that nodes that can run over more than one frame will 
            // yield Stopped on their next iteration
            if (GetStopper().shouldStop)
            {
                foreach (BTNode node in runningNodes)
                {
                    node.Stop();
                }
            }

            bool[] markedForDelete = new bool[runningCoroutines.Count()];

            for (int i = 0; i < runningCoroutines.Count(); i++)
            {
                runningCoroutines[i].MoveNext();

                if (runningCoroutines[i].Current == BTNodeResult.Success)
                {
                    markedForDelete[i] = true;

                    if (type == BTParallelTypes.FirstReturn ||
                        type == BTParallelTypes.FirstSuccess)
                    {
                        returnValue = BTNodeResult.Success;
                        SendStopToRunning(runningNodes, markedForDelete);
                    }
                }
                else if (runningCoroutines[i].Current == BTNodeResult.Failure)
                {
                    markedForDelete[i] = true;

                    if (type == BTParallelTypes.FirstReturn ||
                        type == BTParallelTypes.FirstFailure)
                    {
                        returnValue = BTNodeResult.Failure;
                        SendStopToRunning(runningNodes, markedForDelete);
                    }
                }
                else if (runningCoroutines[i].Current == BTNodeResult.Stopped)
                {
                    markedForDelete[i] = true;
                }
                else if (runningCoroutines[i].Current == BTNodeResult.Running)
                {
                    // Do noting
                }
            }

            // Do one more pass to let running nodes stop if they were notified to stop
            if (returnValue == BTNodeResult.Failure || returnValue == BTNodeResult.Success)
            {
                for (int i = 0; i < runningCoroutines.Count(); i++)
                {
                    runningCoroutines[i].MoveNext();
                    if (!markedForDelete[i] && runningCoroutines[i].Current == BTNodeResult.Stopped)
                    {
                        markedForDelete[i] = true;
                    }
                }
            }

            for (int i = markedForDelete.Count() - 1; i >= 0; i--)
            {
                if (markedForDelete[i])
                {
                    runningCoroutines.RemoveAt(i);
                    runningNodes.RemoveAt(i);
                }
            }
        }

        // At this point there are no more running nodes
        if (GetStopper().shouldStop)
        {
            GetStopper().shouldStop = false;
            yield return BTNodeResult.Stopped;
            yield break;
        }
        else
        {
            yield return returnValue;
            yield break;
        }
    }

    private void SendStopToRunning(List<BTNode> running, bool[] marked)
    {
        for (int i=0; i < running.Count(); i++)
        {
            if (!marked[i])
            {
                running[i].Stop();
            }
        }
    }
}

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
        List<BTCoroutine> runningNodes = new List<BTCoroutine>();

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
                runningNodes.Add(routine);
            }
        }
        while (runningNodes.Count() != 0)
        {
            yield return BTNodeResult.Running;

            foreach (BTCoroutine running in runningNodes.ToList())  // iterate over the copy and delete from the original
            {
                running.MoveNext();
                if (running.Current == BTNodeResult.Success)
                {
                    if (type == BTParallelTypes.FirstReturn ||
                        type == BTParallelTypes.FirstSuccess)
                    {
                        yield return BTNodeResult.Success;
                        yield break;
                    }
                    else
                    {
                        runningNodes.Remove(running);
                    }
                }
                else if (running.Current == BTNodeResult.Failure)
                {
                    if (type == BTParallelTypes.FirstReturn ||
                        type == BTParallelTypes.FirstFailure)
                    {
                        yield return BTNodeResult.Failure;
                        yield break;
                    }
                    else
                    {
                        runningNodes.Remove(running);
                    }
                }
                else if (running.Current == BTNodeResult.Running)
                {
                    // Do noting
                }
            }
        }

        // At this point there are no more running nodes
        yield return BTNodeResult.Success;
    }
}

﻿using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

using Random = UnityEngine.Random;
using Coroutine = System.Collections.IEnumerator;
using BTCoroutine = System.Collections.Generic.IEnumerator<BTNodeResult>;

public class BTSelector : BTNode
{
    private BTNode[] subNodes;

    public BTSelector(IEnumerable<BTNode> subNodes)
    {
        this.subNodes = subNodes.ToArray();
    }

    public BTSelector(params BTNode[] subNodes)
    {
        this.subNodes = subNodes;
    }

    public override BTCoroutine Procedure()
    {
        foreach (BTNode node in subNodes)
        {
            BTCoroutine routine = node.Procedure();

            while (routine.MoveNext())
            {
                BTNodeResult result = routine.Current;

                if (result == BTNodeResult.Stopped)
                {
                    GetStopper().shouldStop = false;
                    yield return BTNodeResult.Stopped;
                    yield break;
                }

                if (result == BTNodeResult.Success)
                {
                    yield return BTNodeResult.Success;
                    yield break;
                }
                else if (result == BTNodeResult.Failure)
                {
                    break;
                }
                else /*if (result == BTNodeResult.Running)*/
                {
                    yield return BTNodeResult.Running;
                    if (GetStopper().shouldStop)
                    {
                        node.Stop();
                    }
                }
            }
        }

        yield return BTNodeResult.Failure;
        yield break;
    }
}
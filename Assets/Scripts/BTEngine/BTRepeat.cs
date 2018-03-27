using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

using Random = UnityEngine.Random;
using Coroutine = System.Collections.IEnumerator;
using BTCoroutine = System.Collections.Generic.IEnumerator<BTNodeResult>;

public class BTRepeat : BTNode
{
    private BTNode childNode;
    private BTRepeatTypes type;

    public BTRepeat(BTRepeatTypes type, BTNode childNode)
    {
        this.childNode = childNode;
        this.type = type;
    }

    public override BTCoroutine Procedure()
    {
        while (true)
        {
            BTCoroutine routine = childNode.Procedure();

            while(routine.MoveNext())
            {
                BTNodeResult result = routine.Current;

                if (result == BTNodeResult.Success)
                {
                    if (type == BTRepeatTypes.UntilSuccess)
                    {
                        yield return BTNodeResult.Success;
                        yield break;
                    }
                }
                else if (result == BTNodeResult.Failure)
                {
                    if (type == BTRepeatTypes.UntilFailure)
                    {
                        yield return BTNodeResult.Failure;
                        yield break;
                    }
                }
            }

            yield return BTNodeResult.Running;
        }
    }
}

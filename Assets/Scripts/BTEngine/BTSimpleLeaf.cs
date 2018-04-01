using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

using Random = UnityEngine.Random;
using Coroutine = System.Collections.IEnumerator;
using BTCoroutine = System.Collections.Generic.IEnumerator<BTNodeResult>;

public class BTSimpleLeaf : BTNode
{
    private Func<bool> simpleLeafCallback;

    public BTSimpleLeaf(Func<bool> simpleLeafCallback)
    {
        this.simpleLeafCallback = simpleLeafCallback;
    }

    public override BTCoroutine Procedure()
    {
        yield return simpleLeafCallback() ? BTNodeResult.Success : BTNodeResult.Failure;
    }
}

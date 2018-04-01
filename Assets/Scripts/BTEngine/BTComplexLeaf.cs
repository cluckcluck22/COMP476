using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

using Random = UnityEngine.Random;
using Coroutine = System.Collections.IEnumerator;
using BTCoroutine = System.Collections.Generic.IEnumerator<BTNodeResult>;

public class BTComplexLeaf : BTNode
{
    private Func<Stopper, BTCoroutine> complexLeafCallback;

    public BTComplexLeaf()
    {
    }

    public void AttachHandler(Func<Stopper, BTCoroutine> complexLeafCallback)
    {
        this.complexLeafCallback = complexLeafCallback;
    }

    public override BTCoroutine Procedure()
    {
        return complexLeafCallback(GetStopper());
    }
}

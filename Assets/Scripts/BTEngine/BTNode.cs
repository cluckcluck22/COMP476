using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

using Random = UnityEngine.Random;
using Coroutine = System.Collections.IEnumerator;
using BTCoroutine = System.Collections.Generic.IEnumerator<BTNodeResult>;

public abstract class BTNode : IBTStoppable
{
    private Stopper stopper = new Stopper();

    public abstract BTCoroutine Procedure();

    public virtual void Stop()
    {
        stopper.shouldStop = true;
    }

    public Stopper GetStopper()
    {
        return stopper;
    }

    public void overrwriteStopper(Stopper newStopper)
    {
        this.stopper = newStopper;
    }
}

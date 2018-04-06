using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Time = UnityEngine.Time;

using Random = UnityEngine.Random;
using Coroutine = System.Collections.IEnumerator;
using BTCoroutine = System.Collections.Generic.IEnumerator<BTNodeResult>;

public class BTConditionLeaf : BTNode
{
    private Func<Scorer, bool> conditionLeafCallback;

    private Scorer scorer = new Scorer();

    private float evalCooldown;
    private float lastFailedEvaluation;

    public BTConditionLeaf()
        : this("2.0f")
    {

    }

    public BTConditionLeaf(string cooldown)
    {
        if (!float.TryParse(cooldown, out evalCooldown))
        {
            evalCooldown = 0.0f;
        }

        lastFailedEvaluation = float.MinValue;
    }

    public void AttachHandler(Func<Scorer, bool> conditionLeafCallback)
    {
        this.conditionLeafCallback = conditionLeafCallback;
    }

    public override BTCoroutine Procedure()
    {
        if (Time.time < lastFailedEvaluation + evalCooldown)
        {
            yield return BTNodeResult.Failure;
            yield break;
        }

        if (conditionLeafCallback(scorer))
        {
            yield return BTNodeResult.Success;
        }
        else
        {
            lastFailedEvaluation = Time.time;
            yield return BTNodeResult.Failure;

        }

        // yield break;
    }

    public Scorer GetScorer()
    {
        return scorer;
    }
}

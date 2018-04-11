using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BTCoroutine = System.Collections.Generic.IEnumerator<BTNodeResult>;

public class BTDecision : BTNode
{
    private BTConditionLeaf[] conditions;
    private BTNode[] actions;

    private int currentRunningNode;
    private int childCount;

    private int frameTicker;
    private int evalFrequency;
    private float switchThreshold;

    public BTDecision(string frequency, string switchThresholdParam, IEnumerable<BTNode> subNodes) 
        : this(frequency, switchThresholdParam, subNodes.ToArray())
    {

    }

    public BTDecision(string frequency, string switchThresholdParam, params BTNode[] subNodes)
    {
        childCount = subNodes.Count();

        conditions = new BTConditionLeaf[childCount / 2];
        actions = new BTNode[childCount / 2];

        for (int i = 0; i < childCount; i += 2)
        {
            conditions[i/2] = subNodes.ElementAt(i) as BTConditionLeaf;   // up casting :(
            actions[i/2] = subNodes.ElementAt(i + 1);
        }

        if (!float.TryParse(switchThresholdParam, out switchThreshold))
        {
            switchThreshold = 0.10f;
        }

        if (!int.TryParse(frequency, out evalFrequency))
        {
            evalFrequency = 5;
        }

        frameTicker = 0;
        currentRunningNode = -1;
    }

    public override BTCoroutine Procedure()
    {
        // First time initialization
        int bestCondition = ScoreConditions();

        if (bestCondition == -1)
        {
            // This shouldn't happen, there should always be a "default" condition that always succeeds at the end
            yield return BTNodeResult.Failure;
            yield break;
        }

        currentRunningNode = bestCondition;
        BTCoroutine routine = actions[currentRunningNode].Procedure();

        // Routine loop
        while (true)
        {
            frameTicker++;

            if (frameTicker >= evalFrequency)
            {
                bestCondition = ScoreConditions();
                if (bestCondition == -1)
                {
                    // This shouldn't happen, there should always be a "default" condition that always succeeds at the end
                    yield return BTNodeResult.Failure;
                    yield break;
                }
                frameTicker = 0;
            }

            if (currentRunningNode != bestCondition)
            {
                if (currentRunningNode != -1)
                {
                    actions[currentRunningNode].Stop();
                    routine.MoveNext();
                    if (routine.Current != BTNodeResult.Stopped)
                    {
                        throw new Exception("On stopping current node in decision");
                    }
                }

                routine = actions[bestCondition].Procedure();
                currentRunningNode = bestCondition;
            }

            routine.MoveNext();
            BTNodeResult result = routine.Current;

            if (result == BTNodeResult.Stopped)
            {
                throw new Exception("Current node got stopped in decision");
            }
            else if (result == BTNodeResult.Success)
            {
                currentRunningNode = -1;
                frameTicker = evalFrequency;
            }
            else if (result == BTNodeResult.Failure)
            {
                currentRunningNode = -1;
                frameTicker = evalFrequency;
            }

            yield return BTNodeResult.Running;

        }
    }

    private int ScoreConditions()
    {
        float bestScore = float.MinValue;
        int bestScoreIndex = -1;
        float currentScore = 0;
        float tempCurrentNodeScore = -1f;

        if (currentRunningNode != -1)
        {
            BTCoroutine routine = conditions[currentRunningNode].Procedure();
            routine.MoveNext();
            BTNodeResult result = routine.Current;

            if (result == BTNodeResult.Success)
            {
                tempCurrentNodeScore = conditions[currentRunningNode].GetScorer().score;
            }
        }

        for (int i = 0; i < conditions.Length; i++)
        {
            if (i == currentRunningNode)
                continue;

            BTCoroutine routine = conditions[i].Procedure();
            routine.MoveNext();
            BTNodeResult result = routine.Current;

            if (result == BTNodeResult.Success)
            {
                currentScore = conditions[i].GetScorer().score;
                if (currentScore > bestScore)
                {
                    bestScoreIndex = i;
                    bestScore = currentScore;
                }
            }
        }

        if (bestScore >= tempCurrentNodeScore + switchThreshold)
        {
            return bestScoreIndex;
        }
        else
        {
            return currentRunningNode;
        }
    }
}

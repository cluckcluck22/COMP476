using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AnimatorDriverDebug : System.Object
{
    [TextArea(19, 19)]
    public string Usage;

    [TextArea(2, 2)]
    public string StateInfo;

    [TextArea(5, 5)]
    public string BlendTreeValues;
}
[RequireComponent(typeof(Animator))]
public class AnimatorDriver : MonoBehaviour {

    public Animator m_animController;

    [Header("Debug Tools")]
    public bool m_useDebugKeyboardBindings = false;
    public AnimatorDriverDebug m_driverDebug;


    [Header("Blend Properties")]
    public int AttackVariations = 2;
    public int EatVariations = 2;
    public int HitReactionVariations = 4;
    public int IdleVariations = 2;
    [Tooltip("This should always be 2, one for Walk and another for Run")]
    public int MoveVariations = 2;
    public int WalkVariations = 3;
    public float BlendSpeed = 1.0f;

    private int m_blendAttack { get; set; }
    private int m_blendEat { get; set; }
    private int m_blendHitReaction { get; set; }
    private int m_BlendIdle { get; set; }
    private int m_blendMove { get; set; }
    private int m_blendWalk { get; set; }

    public States.FullBody m_currentFullBodyState { get; private set; }
    public States.Layered m_currentLayeredState { get; private set; }
    public States.MoveSpeed m_currentMoveSpeed { get; private set; }

    private float m_layeredUpdateTimeout;

    public void PlayFullBodyState(States.FullBody state)
    {
        // So C# can't convert implicitly from 1 to True...
        // There is no "flag" to set for Idle, since it is the default state
        m_animController.SetBool("IsDead", 1 == (1 & ((int)state >> 1)));
        m_animController.SetBool("IsEating", 1 == (1 & ((int)state >> 2)));
        m_animController.SetBool("IsHating", 1 == (1 & ((int)state >> 3)));
        m_animController.SetBool("IsMoving", 1 == (1 & ((int)state >> 4)));
        m_animController.SetBool("IsResting", 1 == (1 & ((int)state >> 5)));

        m_currentFullBodyState = state;
    }

    public void PlayLayeredState(States.Layered state)
    {
        switch (state)
        {
            case States.Layered.Attack:
                m_animController.SetTrigger("TriggerAttack");
                break;
            case States.Layered.Talk:
                m_animController.SetTrigger("TriggerTalk");
                break;
            case States.Layered.HitReaction:
                m_animController.SetTrigger("TriggerHitReaction");
                break;
            case States.Layered.None:
            default:
                break;
        }

        m_currentLayeredState = state;
        m_layeredUpdateTimeout = Time.time + 0.50f;
    }

    public void SetMoveSpeed(States.MoveSpeed speed)
    {
        if (m_currentMoveSpeed != speed)
        {
            CycleMove();
            m_currentMoveSpeed = speed;
        }
    }

    public void CycleAttack()
    {
        m_blendAttack = CycleBlendTreeValue(m_blendAttack, AttackVariations);
    }

    public void CycleEat()
    {
        m_blendEat = CycleBlendTreeValue(m_blendEat, EatVariations);
    }

    public void CycleIdle()
    {
        m_BlendIdle = CycleBlendTreeValue(m_BlendIdle, IdleVariations);
    }

    public void CycleHitReaction()
    {
        m_blendHitReaction = CycleBlendTreeValue(m_blendHitReaction, HitReactionVariations);
    }

    // This one is private, because we only want the outside world to set the move animation by calling SetMoveSpeed()
    private void CycleMove()
    {
        m_blendMove = CycleBlendTreeValue(m_blendMove, MoveVariations);
    }

    private void CycleWalk()
    {
        m_blendWalk = CycleBlendTreeValue(m_blendWalk, WalkVariations);
    } 

	// Use this for initialization
	void Start ()
    {
        m_currentFullBodyState = States.FullBody.Idle;
        m_currentLayeredState = States.Layered.None;

        m_blendAttack = 0;
        m_blendEat = 0;
        m_blendHitReaction = 0;
        m_BlendIdle = 0;
        m_blendMove = 0;

        m_driverDebug.Usage =
              "--- State Transitions Cheat Sheet --- \n"
            + "A : Idle\n"
            + "S : Dead\n"
            + "D : Eat\n"
            + "F : Hate\n"
            + "G : Move\n"
            + "H : Rest\n\n"
            + "--- Layered Action Cheat Sheet ------ \n"
            + "Z : Talk\n"
            + "X : Attack\n"
            + "C : Hit Reaction\n\n"
            + "--- Blend Tree Variations Cycling --- \n"
            + "Q : Attack\n"
            + "W : Eat\n"
            + "E : Hit Reaction\n"
            + "R : Idle\n"
            + "T : Move"
            + "Y : Walk";
    }

    private void LateUpdate()
    {
        if (m_currentLayeredState != States.Layered.None && Time.time >= m_layeredUpdateTimeout)
        {
            if (m_animController.GetBool("FlagNoLayered") == true)
            {
                m_currentLayeredState = States.Layered.None;
            }
        }
    }

    // Update is called once per frame
    void Update ()
    {
        m_driverDebug.StateInfo = "Current State -> " + m_currentFullBodyState.ToString() + "\nCurrent Action -> " + m_currentLayeredState.ToString();

        m_driverDebug.BlendTreeValues = ""
            + "Attack : " + m_blendAttack + "\n"
            + "Eat : " + m_blendEat + "\n"
            + "Hit Reaction : " + m_blendHitReaction + "\n"
            + "Idle : " + m_BlendIdle + "\n"
            + "Move : " + m_blendMove;

        // This call won't be necessary once the rest of the game is wired up
        if (m_useDebugKeyboardBindings)
            HandleUserInput();

        // This updates the blend values in the Blend Trees in order to smoothly transition from one to the other
        BlendInterpolation("BlendAttack", m_blendAttack);
        BlendInterpolation("BlendEat", m_blendEat);
        BlendInterpolation("BlendHitReaction", m_blendHitReaction);
        BlendInterpolation("BlendIdle", m_BlendIdle);
        BlendInterpolation("BlendMove", m_blendMove);

    }

    private int CycleBlendTreeValue(int current, int maxValue)
    {
        current++;
        current %= AttackVariations * 2;
        if (current >= maxValue)
        {
            current -= maxValue;
        }
        return current;
    }

    private void BlendInterpolation(string paramName, float targetValue)
    {
        float current = m_animController.GetFloat(paramName);
        if (Mathf.Approximately(targetValue, current))
        {
            return;
        }

        float step = Mathf.Min( Mathf.Abs(targetValue - current), Mathf.Abs(BlendSpeed * Time.deltaTime) );

        float direction = targetValue > current ? 1.0f : -1.0f;
        m_animController.SetFloat(paramName, current + step * direction);
    }

    private void HandleUserInput()
    {
        // Currently the only thing driving the driver is user input
        if (Input.GetKeyDown(KeyCode.A))
        {
            PlayFullBodyState(States.FullBody.Idle);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            PlayFullBodyState(States.FullBody.Dead);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            PlayFullBodyState(States.FullBody.Eat);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            PlayFullBodyState(States.FullBody.Hate);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            PlayFullBodyState(States.FullBody.Move);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            PlayFullBodyState(States.FullBody.Rest);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            PlayLayeredState(States.Layered.Talk);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            PlayLayeredState(States.Layered.Attack);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            PlayLayeredState(States.Layered.HitReaction);
        }

        // For Blend values, tests values go from min to max, then max to min
        // That's just the way the blend tree works, there is no interpolation beyond the max value that wraps around to the min
        // (Hope this makes sense...)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CycleAttack();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            CycleEat();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            CycleHitReaction();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            CycleIdle();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            CycleMove();
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            CycleWalk();
        }
    }
}

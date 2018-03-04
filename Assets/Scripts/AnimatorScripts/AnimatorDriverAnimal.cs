using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimatorDriverAnimalDebug : System.Object
{
    [TextArea(20, 20)]
    public string Usage;

    [TextArea(2, 2)]
    public string StateInfo;

    [TextArea(5, 5)]
    public string BlendTreeValues;
}
[RequireComponent(typeof(Animator))]
public class AnimatorDriverAnimal : MonoBehaviour {

    private Animator m_animController;

    [Header("Debug Tools")]
    public bool m_useDebugKeyboardBindings = false;
    public AnimatorDriverAnimalDebug m_debug;

    [Header("Supported States")]
    public States.AnimalFullBody[] m_fullbodyStatesAllowed;
    public States.AnimalLayered[] m_layeredStatesAllowed;

    [Header("Blend Properties")]
    public int AttackVariations = 2;
    public int EatVariations = 2;
    public int HitReactionVariations = 4;
    public int IdleVariations = 2;
    public int MoveVariations = 2;
    public int WalkVariations = 3;
    public int SitVariations = 3;
    public float BlendSpeed = 1.0f;

    private int m_blendAttack { get; set; }
    private int m_blendEat { get; set; }
    private int m_blendHitReaction { get; set; }
    private int m_BlendIdle { get; set; }
    private int m_blendMove { get; set; }
    private int m_blendWalk { get; set; }
    private int m_blendSit { get; set; }

    public States.AnimalFullBody currentFullBodyState { get; private set; }
    public States.AnimalLayered currentLayeredState { get; private set; }
    public States.MoveSpeed currentMoveSpeed { get; private set; }

    private float m_layeredUpdateTimeout;

    // For animal movement, use PlayRun() or PlayWalk() instead
    // Or set the movement speed after calling PlayFullBodyState(). The animals might have various variations
    // on their walking animations eventually (cyclebreakers).
    public void PlayFullBodyState(States.AnimalFullBody state)
    {
        // So C# can't convert implicitly from 1 to True...
        // There is no "flag" to set for Idle, since it is the default state
        m_animController.SetBool("IsDead", 1 == (1 & ((int)state >> 1)));
        m_animController.SetBool("IsEating", 1 == (1 & ((int)state >> 2)));
        m_animController.SetBool("IsHating", 1 == (1 & ((int)state >> 3)));
        m_animController.SetBool("IsMoving", 1 == (1 & ((int)state >> 4)));
        m_animController.SetBool("IsResting", 1 == (1 & ((int)state >> 5)));
        m_animController.SetBool("IsSitting", 1 == (1 & ((int)state >> 6)));

        currentFullBodyState = state;
    }

    public void PlayWalk()
    {
        PlayFullBodyState(States.AnimalFullBody.Move);
        m_blendMove = 0;
    }

    public void PlayRun()
    {
        PlayFullBodyState(States.AnimalFullBody.Move);
        m_blendMove = 1;
    }

    public void PlayLayeredState(States.AnimalLayered state)
    {
        switch (state)
        {
            case States.AnimalLayered.Attack:
                m_animController.SetTrigger("TriggerAttack");
                break;
            case States.AnimalLayered.Talk:
                m_animController.SetTrigger("TriggerTalk");
                break;
            case States.AnimalLayered.HitReaction:
                m_animController.SetTrigger("TriggerHitReaction");
                break;
            case States.AnimalLayered.None:
            default:
                break;
        }

        currentLayeredState = state;
        m_layeredUpdateTimeout = Time.time + 0.50f;
    }

    public void SetMoveSpeed(States.MoveSpeed speed)
    {
        if (currentMoveSpeed != speed)
        {
            CycleMove();
            currentMoveSpeed = speed;
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

    private void CycleSit()
    {
        m_blendSit = CycleBlendTreeValue(m_blendSit, SitVariations);
    }

    // Use this for initialization
    void Start ()
    {
        currentFullBodyState = States.AnimalFullBody.Idle;
        currentLayeredState = States.AnimalLayered.None;

        m_blendAttack = 0;
        m_blendEat = 0;
        m_blendHitReaction = 0;
        m_BlendIdle = 0;
        m_blendMove = 0;
        m_blendSit = 0;

        m_debug.Usage =
              "--- State Transitions Cheat Sheet --- \n"
            + "A : Idle\n"
            + "S : Dead\n"
            + "D : Eat\n"
            + "F : Hate\n"
            + "G : Move\n"
            + "H : Rest\n"
            + "J : Sit\n\n"
            + "--- Layered Action Cheat Sheet ------ \n"
            + "Z : Talk\n"
            + "X : Attack\n"
            + "C : Hit Reaction\n\n"
            + "--- Blend Tree Variations Cycling --- \n"
            + "Q : Attack\n"
            + "W : Eat\n"
            + "E : Hit Reaction\n"
            + "R : Idle\n"
            + "T : Move\n"
            + "Y : Walk\n"
            + "U : Sit";
    }

    private void Awake()
    {
        m_animController = GetComponent<Animator>();
    }

    private void LateUpdate()
    {
        if (currentLayeredState != States.AnimalLayered.None && Time.time >= m_layeredUpdateTimeout)
        {
            if (m_animController.GetBool("FlagNoLayered") == true)
            {
                currentLayeredState = States.AnimalLayered.None;
            }
        }
    }

    // Update is called once per frame
    void Update ()
    {
        m_debug.StateInfo = "Current State -> " + currentFullBodyState.ToString() + "\nCurrent Action -> " + currentLayeredState.ToString();

        m_debug.BlendTreeValues = ""
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
        if (maxValue == 0)
            return 0;

        current++;
        current %= maxValue * 2;
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
            PlayFullBodyState(States.AnimalFullBody.Idle);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            PlayFullBodyState(States.AnimalFullBody.Dead);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            PlayFullBodyState(States.AnimalFullBody.Eat);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            PlayFullBodyState(States.AnimalFullBody.Hate);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            PlayFullBodyState(States.AnimalFullBody.Move);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            PlayFullBodyState(States.AnimalFullBody.Rest);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            PlayFullBodyState(States.AnimalFullBody.Sit);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            PlayLayeredState(States.AnimalLayered.Talk);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            PlayLayeredState(States.AnimalLayered.Attack);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            PlayLayeredState(States.AnimalLayered.HitReaction);
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
        if (Input.GetKeyDown(KeyCode.U))
        {
            CycleSit();
        }
    }
}

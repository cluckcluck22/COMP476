using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimatorDriverFarmerDebug : System.Object
{
    [TextArea(9, 9)]
    public string Usage;

    [TextArea(2, 2)]
    public string StateInfo;
}

public class AnimatorDriverFarmer : MonoBehaviour {

    private Animator m_animController;

    [Header("Debug Tools")]
    public bool m_useDebugKeyboardBindings = false;
    public AnimatorDriverFarmerDebug m_debug;

    public States.FarmerFullBody currentFullBodyState { get; private set; }
    public States.FarmerLayered currentLayeredState { get; private set; }

    private float m_layeredUpdateTimeout;

    public void PlayFullBodyState(States.FarmerFullBody state)
    {
        m_animController.SetBool("IsWalking", 1 == (1 & ((int)state >> 1)));
        m_animController.SetBool("IsRunning", 1 == (1 & ((int)state >> 2)));
        m_animController.SetBool("IsDigging", 1 == (1 & ((int)state >> 3)));

        switch (state)
        {
            case States.FarmerFullBody.Dig:
                m_animController.SetTrigger("TriggerDig");
                break;
            case States.FarmerFullBody.Fire:
                m_animController.SetTrigger("TriggerFire");
                break;
            case States.FarmerFullBody.Interact:
                m_animController.SetTrigger("TriggerInteract");
                break;
            default:
                break;
        }

        currentFullBodyState = state;
    }

    public void PlayLayeredState(States.FarmerLayered state)
    {
        switch (state)
        {
            case States.FarmerLayered.Sign:
                m_animController.SetTrigger("TriggerSign");
                break;
            default:
                break;
        }

        currentLayeredState = state;
        m_layeredUpdateTimeout = Time.time + 0.50f;
    }

    // Use this for initialization
    void Start ()
    {
        currentFullBodyState = States.FarmerFullBody.Idle;
        currentLayeredState = States.FarmerLayered.None;

        m_debug.Usage =
          "--- State Transitions Cheat Sheet --- \n"
        + "A : Idle\n"
        + "S : Walk\n"
        + "D : Run\n"
        + "F : Dig\n"
        + "G : Fire\n"
        + "H : Interact\n\n"
        + "--- Layered Action Cheat Sheet ------ \n"
        + "Z : Sign\n";
    }

    private void Awake()
    {
        m_animController = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update ()
    {
        m_debug.StateInfo = "Current State -> " + currentFullBodyState.ToString() + "\nCurrent Action -> " + currentLayeredState.ToString();

        // This call won't be necessary once the rest of the game is wired up
        if (m_useDebugKeyboardBindings)
            HandleUserInput();
    }

    private void LateUpdate()
    {
        if (currentLayeredState != States.FarmerLayered.None && Time.time >= m_layeredUpdateTimeout)
        {
            if (m_animController.GetBool("FlagNoLayered") == true)
            {
                currentLayeredState = States.FarmerLayered.None;
            }
        }
    }

    private void HandleUserInput()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            PlayFullBodyState(States.FarmerFullBody.Idle);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            PlayFullBodyState(States.FarmerFullBody.Walk);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            PlayFullBodyState(States.FarmerFullBody.Run);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            PlayFullBodyState(States.FarmerFullBody.Dig);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            PlayFullBodyState(States.FarmerFullBody.Fire);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            PlayFullBodyState(States.FarmerFullBody.Interact);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            PlayLayeredState(States.FarmerLayered.Sign);
        }
    }
}

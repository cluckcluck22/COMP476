using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmerFireRotate : StateMachineBehaviour {

    private float m_startTime { get; set; }
    private Quaternion m_startOrientation { get; set; }
    private Vector3 m_startFront { get; set; }
    private Vector3 m_targetFront { get; set; }
    private float m_rotateOutStart { get; set; }
    private float m_clipLength { get; set; }
    private Transform m_transform { get; set; }
    private float m_rotationTimeNormalized { get; set; }

    public float m_rotationTime;
    public float m_rotationAmount;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_transform = animator.gameObject.GetComponent<Transform>();
        m_startOrientation = m_transform.rotation;
        m_startTime = Time.time;
        m_startFront = m_transform.forward;
        m_targetFront = Quaternion.AngleAxis(m_rotationAmount, Vector3.up) * m_transform.forward;
        m_rotationTimeNormalized = m_rotationTime / stateInfo.length;
        m_rotateOutStart = 1.0f - m_rotationTimeNormalized;

        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float normalizedTime = stateInfo.normalizedTime;
        if (normalizedTime <= m_rotationTimeNormalized)
        {
            float progresss = normalizedTime / m_rotationTimeNormalized;
            Vector3 newDirection = Vector3.RotateTowards(m_startFront, m_targetFront, progresss, 0.0f);
            m_transform.rotation = Quaternion.LookRotation(newDirection);
        }
        else if (normalizedTime >= m_rotateOutStart)
        {
            float progress = (normalizedTime - m_rotateOutStart) / m_rotationTimeNormalized;
            Vector3 newDirection = Vector3.RotateTowards(m_targetFront, m_startFront, progress, 0.0f);
            m_transform.rotation = Quaternion.LookRotation(newDirection);
        }

        base.OnStateMove(animator, stateInfo, layerIndex);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Snap to original orientation (just to be safe)
        m_transform.rotation = m_startOrientation;

        base.OnStateExit(animator, stateInfo, layerIndex);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}

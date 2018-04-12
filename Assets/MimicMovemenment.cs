using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicMovemenment : MonoBehaviour
{
    //Camera
    public Transform Camera_Mimic;
    // Movement Attributes
    private float horizontal;
    private float vertical;
    public float m_speed;
    public float m_WalkSpeed = 3.0f;
    public float m_RunSpeed = 8.0f;
    //public float mAngularSpeed = 180.0f;
    //public float mAngularSpeed = 180.0f;
    float speedSmoothVelocity;
    public float turnSmoothTime = 0.2f;

    //Animations
    public AnimatorDriverAnimal m_AnimatorDriverAnimal;
    public PhotonView mPhotonView;
    private bool isWalking;
    private bool isRunning;

    private bool isConsuming;


    void Start()
    {
        //m_AnimatorDriverAnimal = GetComponentInChildren<AnimatorDriverAnimal>();
        mPhotonView = GetComponent<PhotonView>();
        isConsuming = false;
    }

    void Update()
    {
        if (!PhotonNetwork.connected || mPhotonView.isMine)
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            //InputHandler();
        }
    }

    void FixedUpdate()
    {
        InputHandler();
    }

    private void SetPlayerRelativeToCameraForward()
    {
        Vector3 forward = GetCameraDirection();
        forward.y = 0.0f;
        forward = forward.normalized;
        // Calculate target direction based on Camera_Mimic forward and direction key.
        Vector3 right = new Vector3(forward.z, 0, -forward.x);
        Vector3 targetDirection;
        targetDirection = forward * vertical + right * horizontal;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSmoothTime);
    }

    private Vector3 GetCameraDirection()
    {
        Vector3 CameraDirection = Camera_Mimic.TransformDirection(Vector3.forward);
        return CameraDirection;
    }

    public bool isMoving()
    {
        return (horizontal != 0) || (vertical != 0);
    }

    private void Movement()
    {

        Vector3 input = new Vector3(horizontal, vertical, 0.0f);
        Vector3 inputDirection = input.normalized;
        if (inputDirection != Vector3.zero)
        {
            SetPlayerRelativeToCameraForward();
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            m_speed = m_RunSpeed * inputDirection.magnitude;   //Running
            isRunning = true;
            isWalking = false;
        }
        else
        {
            m_speed = m_WalkSpeed * inputDirection.magnitude;  //Walking
            isWalking = true;
            isRunning = false;
        }
        transform.Translate(transform.forward * m_speed * Time.deltaTime, Space.World);


        //Animate
        if (isWalking)
        {
            m_AnimatorDriverAnimal.PlayWalk();
        }
        if (isRunning)
        {
            m_AnimatorDriverAnimal.PlayRun();
        }
    }

    public void Eating()
    {
        m_AnimatorDriverAnimal.PlayFullBodyState(States.AnimalFullBody.Eat);
        isConsuming = true;
    }

    private void Attack()
    {
        m_AnimatorDriverAnimal.PlayLayeredState(States.AnimalLayered.Attack);
    }

    public void Rest()
    {
        m_AnimatorDriverAnimal.PlayFullBodyState(States.AnimalFullBody.Rest);
        isConsuming = true;
    }

    private void Talk()
    {
        m_AnimatorDriverAnimal.PlayLayeredState(States.AnimalLayered.Talk);
    }

    private void InputHandler()
    {
        if (isMoving())
        {
            isConsuming = false;
            Movement();
        }
        else if (!isMoving() && !isConsuming)
        {
            //If Not moving then idle
            m_AnimatorDriverAnimal.PlayFullBodyState(States.AnimalFullBody.Idle);
        }

        ////Eating
        //if (Input.GetKey(KeyCode.E))
        //{
        //    Eating();
        //}

        ////Attack
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    Attack();
        //}

        ////Sleep
        //if (Input.GetKey(KeyCode.F))
        //{
        //    Rest();
        //}

        ////Sleep
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    Talk();
        //}

    }

   

}

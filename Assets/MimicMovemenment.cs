using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicMovemenment : MonoBehaviour
{

    // Movement Attributes
    private float horizontal;
    private float vertical;
    public float m_speed;
    public float m_WalkSpeed = 5.0f;
    public float m_RunSpeed = 8.0f;
    public float mAngularSpeed = 180.0f;

    //Animations
    private AnimatorDriverAnimal m_AnimatorDriverAnimal;
    private bool isWalking;
    private bool isRunning;

    void Start()
    {
        // Keep a backup of the original scale
        m_AnimatorDriverAnimal = GetComponent<AnimatorDriverAnimal>();
    }

    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        InputHandler();
    }

    private bool isMoving()
    {
        return (horizontal != 0) || (vertical != 0);
    }

    private void Movement()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            m_speed = m_RunSpeed;   //Running
            isRunning = true;
            isWalking = false;
        }
        else
        {
            m_speed = m_WalkSpeed;  //Walking
            isWalking = true;
            isRunning = false;
        }

        Vector3 direction = new Vector3(horizontal, 0.0f, vertical);
        // Cap the magnitude of direction vector
        direction = Vector3.ClampMagnitude(direction, 1.0f);
        // Translate the game object in world space
        transform.Translate(direction * m_speed * Time.deltaTime, Space.World);
        // Rotate the game object
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), mAngularSpeed * Time.deltaTime);

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

    private void Eating()
    {
        m_AnimatorDriverAnimal.PlayFullBodyState(States.AnimalFullBody.Eat);
    }

    private void Attack()
    {
        m_AnimatorDriverAnimal.PlayLayeredState(States.AnimalLayered.Attack);
    }

    private void Rest()
    {
        m_AnimatorDriverAnimal.PlayFullBodyState(States.AnimalFullBody.Rest);
    }

    private void Talk()
    {
        m_AnimatorDriverAnimal.PlayLayeredState(States.AnimalLayered.Talk);
    }
    private void InputHandler()
    {
        if (isMoving())
        {
            Movement();
        }
        else if (!isMoving())
        {
            //Not moving idle
            m_AnimatorDriverAnimal.PlayFullBodyState(States.AnimalFullBody.Idle);
        }

        //Eating
        /**NOTE: There is a delay in the eating...you have to press E twice after Moving for it to eat.
        * Possibly a problem with animation blending
        */
        if (Input.GetKeyDown(KeyCode.E))
        {
            Eating();
        }
        
        //Attack
        if(Input.GetKeyDown(KeyCode.Q))
        {
            Attack();
        }

        //Sleep
        if (Input.GetKeyDown(KeyCode.F))
        {
            Rest();
        }

        //Sleep
        if (Input.GetKeyDown(KeyCode.R))
        {
            Talk();
        }

    }

    //SPEEED conditions, keeping track of the speed of the mimic, once it reaches a certain speed we do either playWalk(), playRun(), Idle

    //Transforming the mimic: mimic management. DestoryObject(),
    //Camera,destory anything below the camera
    //onAwake the character and attach to the camera, and when destoried detach the camera
    //Empty gameObject.
    //swapping transform - copy Bessie & real Bessie (storing the states  information)
}

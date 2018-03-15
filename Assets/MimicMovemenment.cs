using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicMovemenment : MonoBehaviour
{

    // Movement Attributes
    private float horizontal;
    private float vertical;
    public float m_WalkSpeed = 5.0f;
    public float m_RunSpeed = 8.0f;
    public float mAngularSpeed = 180.0f;
    
    //Animations
    private AnimatorDriverAnimal m_AnimatorDriverAnimal;

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

    private void Eating()
    {
            m_AnimatorDriverAnimal.PlayFullBodyState(States.AnimalFullBody.Eat);
    }

    private bool Attack()
    {
        return false;
    }

    private void InputHandler()
    {
        if (isMoving())
        {
                /**WALKING
                    The code below is only walking at the moment
                */
                Vector3 direction = new Vector3(horizontal, 0.0f, vertical);
                // Cap the magnitude of direction vector
                direction = Vector3.ClampMagnitude(direction, 1.0f);
                // Translate the game object in world space
                transform.Translate(direction * m_WalkSpeed * Time.deltaTime, Space.World);
                // Rotate the game object
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), mAngularSpeed * Time.deltaTime);
                //Animate
                m_AnimatorDriverAnimal.PlayWalk();
        }
        else if(!isMoving())
        {
            m_AnimatorDriverAnimal.PlayFullBodyState(States.AnimalFullBody.Idle);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Eating();
        }
    }

    //SPEEED conditions, keeping track of the speed of the mimic, once it reaches a certain speed we do either playWalk(), playRun(), Idle

    //Transforming the mimic: mimic management. DestoryObject(),
    //Camera,destory anything below the camera
    //onAwake the character and attach to the camera, and when destoried detach the camera
    //Empty gameObject.
    //swapping transform - copy Bessie & real Bessie (storing the states  information)
}

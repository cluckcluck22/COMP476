using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransformGUI : MonoBehaviour
{

    public Transform[] m_transforms;
    
    void Awake()
    {
        m_transforms = new Transform[4];
    }

    // Update is called once per frame
    void Update()
    {
        //if (m_transforms != null)
        //{
        //    Debug.Log("Index 0 (Selection 1-Cow): " + m_transforms[0]);
        //    Debug.Log("Index 1 (Selection 2-Pig):  " + m_transforms[1]);
        //    Debug.Log("Index 2 (Selection 3-Sheep): " + m_transforms[2]);
        //    Debug.Log("Index 3 (Selection 4-Ram): " + m_transforms[4]);
        //}
    }

    public Transform[] GetMimicAnimalList() 
    {
        return m_transforms;
    }

}

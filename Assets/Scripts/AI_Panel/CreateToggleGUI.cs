using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateToggleGUI : MonoBehaviour
{
//USELESS


    private ToggleGroup m_toggleGroup;

    GameObject[] ChildrenToggles;

    // Use this for initialization
    void Start()
    {
        m_toggleGroup = GetComponent<ToggleGroup>();
        ChildrenToggles = new GameObject[transform.childCount];
    }

    // Update is called once per frame
    void Update()
    {
        if (ChildrenToggles == null)
            GetListOfTogglesInChildren();
        
    }

    void GetListOfTogglesInChildren()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            ChildrenToggles[i] = transform.GetChild(i).gameObject;
        }
    }


}

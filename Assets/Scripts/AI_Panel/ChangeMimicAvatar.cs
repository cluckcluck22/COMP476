using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMimicAvatar : MonoBehaviour {

    GameObject[] mimicImg;
	
	void Awake () 
    {
        mimicImg = new GameObject[5];
        mimicImg[0] = transform.GetChild(0).gameObject;
        mimicImg[0].SetActive(true);
        for (int i = 1; i < mimicImg.Length; i++)   //Starting at 1 to avoid the panel object
        {
            mimicImg[i] = transform.GetChild(i).gameObject;
        }
    }

    void Start()
    {
        mimicImg[1].SetActive(true);
        mimicImg[2].SetActive(false);
        mimicImg[3].SetActive(false);
        mimicImg[4].SetActive(false);
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            mimicImg[1].SetActive(true);
            mimicImg[2].SetActive(false);
            mimicImg[3].SetActive(false);
            mimicImg[4].SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            mimicImg[1].SetActive(false);
            mimicImg[2].SetActive(true);
            mimicImg[3].SetActive(false);
            mimicImg[4].SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            mimicImg[1].SetActive(false);
            mimicImg[2].SetActive(false);
            mimicImg[3].SetActive(true);
            mimicImg[4].SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            mimicImg[1].SetActive(false);
            mimicImg[2].SetActive(false);
            mimicImg[3].SetActive(false);
            mimicImg[4].SetActive(true);
        }
    }
}

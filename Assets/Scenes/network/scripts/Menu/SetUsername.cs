using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetUsername : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void setUsername(GameObject text)
    {
        NetworkInformation.username = text.GetComponent<InputField>().text;
    }
}

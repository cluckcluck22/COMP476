using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEndGame : MonoBehaviour {
    bool end = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(!end && Input.GetKeyDown(KeyCode.O))
        {
            GameObject tmp = GameObject.FindGameObjectWithTag("NetworkManager");
            tmp.GetComponent<NetworkManager>().killAnimal();
            tmp.GetComponent<NetworkManager>().killAnimal();
            tmp.GetComponent<NetworkManager>().endGameFunction();
        }
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basicMovement : Photon.MonoBehaviour {

    Rigidbody mRigidbody;

	// Use this for initialization
	void Start () {
        mRigidbody = GetComponent<Rigidbody>();
        if(!PhotonNetwork.isMasterClient)
        {
            mRigidbody.isKinematic = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if(PhotonNetwork.isMasterClient)
        {
            if(Input.GetKeyDown(KeyCode.A))
            {
                mRigidbody.velocity = Vector3.left * 2.0f;
            }
            else if(Input.GetKeyDown(KeyCode.D))
            {
                mRigidbody.velocity = Vector3.right * 2.0f;
            }
        }
		
	}
}

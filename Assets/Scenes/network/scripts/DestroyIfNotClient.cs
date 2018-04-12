using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyIfNotClient : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    if(PhotonNetwork.connected && PhotonNetwork.isMasterClient)
        {
            Destroy(gameObject);
        }	
	}
}

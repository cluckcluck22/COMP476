using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonCorrecting : Photon.MonoBehaviour {

	// Use this for initialization
	void Start () {
        PhotonNetwork.sendRate = 10;
        PhotonNetwork.sendRateOnSerialize = 10;
	}
	
	// Update is called once per frame
	void Update () {
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncMimicNeeds : MonoBehaviour {

    public PhotonView mPhotonView;
    public AnimalAI mAnimalAI;

	// Use this for initialization
	void Start () {
        mPhotonView = GetComponent<PhotonView>();
        mAnimalAI = GetComponent<AnimalAI>();
	}
	
	// Update is called once per frame
	void Update () {
        if (PhotonNetwork.connected && !PhotonNetwork.isMasterClient)
        {
            mPhotonView.RPC("syncNeeds", PhotonTargets.Others, mAnimalAI.hunger, mAnimalAI.fatigue);
        }
	}

    [PunRPC]
    public void syncNeeds (float hung,float fat)
    {
        mAnimalAI.hunger = hung;
        mAnimalAI.fatigue = fat;
    }
}

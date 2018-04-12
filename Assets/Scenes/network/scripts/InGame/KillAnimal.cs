using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillAnimal : MonoBehaviour {

	public void KillAnimalFunction()
    {
        if (!PhotonNetwork.connected)
        {
            KillAnimalNetwork();
        }
        else
        {
            GetComponent<PhotonView>().RPC("KillAnimalNetwork", PhotonTargets.All);
        }
    }

    [PunRPC]
    public void KillAnimalNetwork()
    {
        gameObject.AddComponent<FadeOut>();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillAnimal : MonoBehaviour {

	public void KillAnimals()
    {
        GetComponent<PhotonView>().RPC("KillAnimalNetwork", PhotonTargets.All);
    }

    [PunRPC]
    public void KillAnimalNetwork()
    {
        gameObject.AddComponent<FadeOut>();
    }
}

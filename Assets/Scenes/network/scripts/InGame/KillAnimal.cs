using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillAnimal : MonoBehaviour {

	public void KillAnimals()
    {
        if(transform.parent != null)
        {
            //TODO end game
            if (gameObject.GetComponent<FadeOut>() == null)
            {
                GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>().endGameFunction();
            }
        }
        else
        {
            if (gameObject.GetComponent<FadeOut>() == null)
            {
                GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>().killAnimal();
            }
        }
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

        AnimalAI aiScript = GetComponent<AnimalAI>();
        if (aiScript != null)
        {
            aiScript.kill();
        }
    }
}

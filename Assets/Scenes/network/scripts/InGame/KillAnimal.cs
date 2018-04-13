/* Class: KillAnimal.cs
 * Programmer: Eric Davies
 * Date: 12/4/2018
 * Description: A class that is attached to any animal in the FarmNetwork scene. When called it will attempt to kill selected animal by attaching a
 *  Fade out script component to it, causing the animal to fade out then get destroyed from all scene instances. If this script is attached to the mimic
 *  and is called, the game will end and the game will return to the lobby with the help of the Netowork manager.
 * 
 * */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillAnimal : MonoBehaviour {

	public void KillAnimals()
    {
        //Check if target is a mimic(mimic look is a child of the mimic and a direct hit can be checked with the mimic tag.)
        if(transform.parent != null || transform.tag.Equals("mimic"))
        {
            //Only trigger if the fadeOut script(the object that destroys the animal) has not already been attached.
            if (gameObject.GetComponent<FadeOut>() == null)
            {
                GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>().endGameFunction();
            }
        }
        else
        {
            //Only trigger if the fadeOut script(the object that destroys the animal) has not already been attached.
            if (gameObject.GetComponent<FadeOut>() == null)
            {
                GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>().killAnimal();
            }
        }
        
        if (!PhotonNetwork.connected)
        {
            //Offline kill animal call
            KillAnimalNetwork();
        }
        else
        {
            //Online kill animal call
            GetComponent<PhotonView>().RPC("KillAnimalNetwork", PhotonTargets.All);
        }
    }

    /* Function KillAnimalNetwork
     * Description: A function that is used to attach the fade out script and to tell the AnimalAI to die.
     * */
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

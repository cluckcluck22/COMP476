/* Calss: DestroyIfNotClient.cs
 * Programmer: Eric Davies
 * Date: 12/4/2018
 * Description: A class that will destroy any game object if there is a connection and this game instance is the master client instance.
 * */

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

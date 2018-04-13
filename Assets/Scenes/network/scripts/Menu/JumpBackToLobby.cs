/* Class: JumpBackToLobby.cs
 * Programmer: Eric Davies
 * Date: 12/4/2018
 * Description: A class that sets the lobby canvas to active and the set canvas to inactive. Used when returning to the menu scene from a network game.
 * */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBackToLobby : MonoBehaviour {
    public GameObject lobbyCanvas;
    public GameObject currentCanvas;

	// Use this for initialization
	void Start () {
        if (PhotonNetwork.room != null)
        {
            currentCanvas.active = false;
            lobbyCanvas.active = true;
        }
	}
}

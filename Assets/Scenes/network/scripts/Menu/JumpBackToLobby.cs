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

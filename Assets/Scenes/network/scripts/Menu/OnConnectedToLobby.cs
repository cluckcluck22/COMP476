/* Class: OnConnectedToLobby.cs
 * Programmer: Eric Davies
 * Date: 12/4/2018
 * Description: A class is to be attached to the loading canvas, will load the target canvas when the onjoinedroom function is called.
 * */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnConnectedToLobby : MonoBehaviour {
    public GameObject currentCanvas;
    public GameObject targetCanvas;

    public virtual void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby called, connected to a lobby");
    }

    public void OnJoinedRoom()
    {
        if (PhotonNetwork.isNonMasterClientInRoom)
        {
            Debug.Log("Connector");
        }
        Debug.Log("OnJoinedRoom called, now in a room for the game");
        currentCanvas.active = false;
        targetCanvas.active = true;
    }
}

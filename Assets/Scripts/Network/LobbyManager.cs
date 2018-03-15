using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : Photon.MonoBehaviour {

    public byte Version = 1;
    RoomInfo[] rooms = null;

    public string roomName = "";
    private string message = "";
    PhotonView photonView;
    private string[] chat = new string[30];

	// Use this for initialization
	void Start () {
        photonView = PhotonView.Get(this);
        //PhotonNetwork.player.NickName = "filler";
        PhotonNetwork.autoJoinLobby = false;
        Debug.Log("Attempting to start connection to master server");
        PhotonNetwork.ConnectUsingSettings(Version + "." + SceneManagerHelper.ActiveSceneBuildIndex);
	}

    void OnGUI()
    {
        if(!PhotonNetwork.connected)
        {
            GUI.Box(new Rect(0, 0, 100, 25), "NotConnected");
        }
        else
        {
            if (!PhotonNetwork.inRoom)
            {
                GUI.Box(new Rect(0, 0, 100, 25), "Connected");
                roomName = GUI.TextField(new Rect(0, 50, 200, 25), roomName);
                GUI.Box(new Rect(0, 100, 100, 25), "NickName");
                PhotonNetwork.player.NickName = GUI.TextField(new Rect(0, 125, 200, 25), PhotonNetwork.player.NickName);
                if (GUI.Button(new Rect(250, 50, 100, 25), "Create"))
                {
                    PhotonNetwork.player.UserId = PhotonNetwork.player.NickName;
                    Debug.Log(PhotonNetwork.player.UserId);
                    Debug.Log(PhotonNetwork.player.NickName);
                    PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions() { MaxPlayers = 4 }, null);
                }
            }
            else
            {
                //Show players and room name
                GUI.Box(new Rect(0, 0, 100, 25), PhotonNetwork.room.Name);
                for(int i = 0; i < PhotonNetwork.playerList.Length;i++)
                {
                    GUI.Box(new Rect(0, 100 + i*50, 300, 25), PhotonNetwork.playerList[i].NickName);
                }

                //Show Chat
                GUI.TextArea(new Rect(Screen.width - 400, Screen.height - 700, 400, 600), fillChat());
                message = GUI.TextField(new Rect(Screen.width - 400, Screen.height - 100, 350, 100), message);
                if(GUI.Button(new Rect(Screen.width - 50,Screen.height - 100,50,100),"Send"))
                {
                    //Send message
                    photonView.RPC("SendMessage", PhotonTargets.All, PhotonNetwork.player.NickName + " : " + message);
                    message = "";
                }

                //Start Game
                if(PhotonNetwork.isMasterClient)
                {
                    if(GUI.Button(new Rect(0,Screen.height - 100,100,25),"Start Game"))
                    {
                        //Start Game
                        photonView.RPC("ChangeScene", PhotonTargets.All);
                    }
                }
            }
        }
    }
    

    // Update is called once per frame
    void Update () {
		
	}

    public virtual void OnPhotonJoinRoomFailed()
    {
        Debug.Log("OnPhotonJoinRoomFailed called, unable to connect to the room from the lobby");
    }

    public virtual void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby called, connected to a lobby");
    }

    public void OnJoinedRoom()
    {
        if(PhotonNetwork.isNonMasterClientInRoom)
        {
            Debug.Log("Connector");
        }
        Debug.Log("OnJoinedRoom called, now in a room for the game");
    }

    public virtual void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() called, now linked to the master server");
    }

    public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        Debug.LogError("Cause: " + cause);
    }

    [PunRPC]
    void SendMessage(string message)
    {
        Array.Copy(chat, 1, chat, 0, chat.Length - 1);
        chat[chat.Length-1] = message;
    }
    [PunRPC]
    void ChangeScene()
    {
        SceneManager.LoadScene("inGame");
    }

    private string fillChat()
    {
        string tmp = "";
        for(int i = 0; i < chat.Length; i ++)
        {
            tmp += (chat[i] + "\n");
        }
        return tmp;
    }
}

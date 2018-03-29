using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyHandler : MonoBehaviour {

    public GameObject LobbyName;
    public GameObject PlayerList;
    public GameObject chatBox;
    private Text mChatText;
    private Text mPlayerText;
    private PhotonView mPhotonView;

    private string[] chat = new string[10];

    // Use this for initialization
    void Start () {

        //might get null references, need to check
        LobbyName.GetComponent<Text>().text += PhotonNetwork.room.Name;
        mPlayerText = PlayerList.GetComponent<Text>();
        mChatText = chatBox.GetComponent<Text>();
        mPhotonView = GetComponent<PhotonView>();

		
	}
	
	// Update is called once per frame
	void Update () {
        mPlayerText.text = generatePlayerList();
        mChatText.text = generateChatList();
    }

    private string generateChatList()
    {
        string tmp = "";
        for (int i = 0; i < chat.Length; i++)
        {
            tmp += (chat[i] + "\n");
        }
        return tmp;
    }

    private string generatePlayerList()
    {
        string box = PhotonNetwork.playerList[0].NickName;
        for(int i = 1; i< PhotonNetwork.playerList.Length;i++)
        {
            box += "\n" +PhotonNetwork.playerList[i].NickName;
        }
        return box;
    }

    public void SendMessage(GameObject inputBox)
    {
        string text = inputBox.GetComponent<InputField>().text;
        inputBox.GetComponent<InputField>().text = "";
        mPhotonView.RPC("receiveMessage", PhotonTargets.All, PhotonNetwork.player.NickName + " : " + text);
    }

    [PunRPC]
    void receiveMessage(string message)
    {
        Debug.Log("received Message");
        Array.Copy(chat, 1, chat, 0, chat.Length - 1);
        chat[chat.Length - 1] = message;
    }
}

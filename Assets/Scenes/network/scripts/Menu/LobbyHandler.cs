/* Class: LobbyHandler.cs
 * Programmer: Eric Davies
 * Date: 12/4/2018
 * Description: A class that handles the general lobby of the game. Handles the chat box, the player list, the ability for the host to launch the game
 *  and to display the room name.
 * */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyHandler : MonoBehaviour {

    public GameObject LobbyName;
    public GameObject PlayerList;
    public GameObject chatBox;
    public GameObject startButton;

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
        if(!PhotonNetwork.isMasterClient)
        {
            startButton.SetActive(false);
        }
		
	}
	
	// Update is called once per frame
	void Update () {
        mPlayerText.text = generatePlayerList();
        mChatText.text = generateChatList();
        if(PhotonNetwork.isMasterClient)
        {
            if(PhotonNetwork.playerList.Length == 2)
            {
                startButton.GetComponent<Button>().interactable = true;
            }
            else
            {
                startButton.GetComponent<Button>().interactable = false;
            }
        }
    }

    /* Function: generateChatList
     * Description: A function that generates a string of all the chat lines into a a line separated string
     * */
    private string generateChatList()
    {
        string tmp = "";
        for (int i = 0; i < chat.Length; i++)
        {
            tmp += (chat[i] + "\n");
        }
        return tmp;
    }
    /* Function: generatePlayerList
     * Description: A function that creates a string of all the player nicknames in the lobby.
     * */
    private string generatePlayerList()
    {
        string box = PhotonNetwork.playerList[0].NickName;
        for(int i = 1; i< PhotonNetwork.playerList.Length;i++)
        {
            box += "\n" +PhotonNetwork.playerList[i].NickName;
        }
        return box;
    }
    /* Function: SendMessage
     * Description: A function called by the send button that allows the player to send a message over the network to other players.
     * */
    public void SendMessage(GameObject inputBox)
    {
        string text = inputBox.GetComponent<InputField>().text;
        inputBox.GetComponent<InputField>().text = "";
        mPhotonView.RPC("receiveMessage", PhotonTargets.All, PhotonNetwork.player.NickName + " : " + text);
    }
    /* Function receiveMessage
     * Description: The function called with an RPC call in the SendMessage function. shifts the message array by one and inserts the new value into the now free space.
     * */
    [PunRPC]
    void receiveMessage(string message)
    {
        Debug.Log("received Message");
        Array.Copy(chat, 1, chat, 0, chat.Length - 1);
        chat[chat.Length - 1] = message;
    }
}

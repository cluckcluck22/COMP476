using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobby : MonoBehaviour {

    public byte Version = 1;
    PhotonView mPhotonView;
    public GameObject joinButton;

    // Use this for initialization
    void Start () {
        mPhotonView = GetComponent<PhotonView>();
        PhotonNetwork.autoJoinLobby = false;
        PhotonNetwork.ConnectUsingSettings(Version + "." + SceneManagerHelper.ActiveSceneBuildIndex);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void joinGame(GameObject inputField)
    {
        PhotonNetwork.player.NickName = NetworkInformation.username;

        PhotonNetwork.player.UserId = PhotonNetwork.player.NickName;

        //hard set to 4 for the moment, should be 2 for the final product
        PhotonNetwork.JoinOrCreateRoom(inputField.GetComponent<InputField>().text, new RoomOptions() { MaxPlayers = 4 }, null);
    }

    public virtual void OnConnectedToMaster()
    {
        joinButton.active = true;
    }

    
}

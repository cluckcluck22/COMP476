using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour {

	public void startGame()
    {
        GetComponent<PhotonView>().RPC("loadLevel", PhotonTargets.All);
    }

    [PunRPC]
    void loadLevel()
    {
        SceneManager.LoadScene("FarmNetwork");
    }
}

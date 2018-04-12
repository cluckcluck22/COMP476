using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour {
    public GameObject Mimic;
    public GameObject Farmer;
    public GameObject Camera;
    public GameObject canvas;


    public GameObject endCanvas;

    private bool endGame = false;

    public int numOfKilledAnimals = 0;

    public bool useDogOffline;

    private int ReadyClients = 0;

    PhotonView mPhotonView;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            canvas.active = !canvas.active;
        }
        if(PhotonNetwork.isMasterClient)
        {
            if(PhotonNetwork.playerList.Length != 2 && !endGame)
            {
                endGame = true;
                endGameLocal();
            }
        }
    }

	// Use this for initialization
	void Awake () {
        mPhotonView = GetComponent<PhotonView>();
        if (PhotonNetwork.connected)
        {
            //if we are master client
            if(PhotonNetwork.isMasterClient)
            {
                //basic setup to assign ownership of farmer to player and make the dog kinematic
                Mimic.SetActive(true);
                Farmer.SetActive(true);
                Camera.GetComponent<ThirdPersonOrbitCamBasic>().player = Farmer.transform;
                Farmer.GetComponent<BasicBehaviour>().playerCamera = Camera.transform;

                //Take over the farmer
                Farmer.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.player);
                //Stop simulation of Dog
                Destroy(Mimic.GetComponent<Rigidbody>());
            }
            else
            {
                //basic setup to assign ownership of dog to player and make the farmer kinematic
                Mimic.SetActive(true);
                Farmer.SetActive(true);
                Camera.GetComponent<ThirdPersonOrbitCamBasic>().player = Mimic.transform;
                Mimic.GetComponent<MimicMovemenment>().Camera_Mimic = Camera.transform;

                //Take over the Dog
                Mimic.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.player);
                //Stop simulation of Farmer
                Destroy(Farmer.GetComponent<Rigidbody>());
            }

            dogDisable(Mimic);
            farmerDisable(Farmer);
            //Tell host that we are ready
            mPhotonView.RPC("tellReady", PhotonTargets.MasterClient);
        }
        else
        {
            //Offline, mostly used for testing.
            if(useDogOffline)
            {
                Mimic.SetActive(true);
                Farmer.SetActive(false);
                Camera.GetComponent<ThirdPersonOrbitCamBasic>().player = Mimic.transform;
                Mimic.GetComponent<MimicMovemenment>().Camera_Mimic = Camera.transform;
            }
            else
            {
                Mimic.SetActive(false);
                Farmer.SetActive(true);
                Camera.GetComponent<ThirdPersonOrbitCamBasic>().player = Farmer.transform;
                Farmer.GetComponent<BasicBehaviour>().playerCamera = Camera.transform;
            }
            Destroy(mPhotonView);
        }

    }

    [PunRPC]
    void tellReady()
    {
        ReadyClients++;
        if(ReadyClients == 2)
        {
            mPhotonView.RPC("startGame", PhotonTargets.All);
        }
    }

    public void endGameLocal()
    {
        if (PhotonNetwork.connected)
        {
            mPhotonView.RPC("endGameNetwork", PhotonTargets.All);
        }
        else
        {
            SceneManager.LoadScene("Menu");
        }
    }

    [PunRPC]
    void endGameNetwork()
    {
        SceneManager.LoadScene("Menu");
    }


    [PunRPC]
    void startGame()
    {
        Debug.Log("Start Game");
        if(PhotonNetwork.isMasterClient)
        {
            farmerEnable(Farmer);
        }
        else
        {
            dogEnable(Mimic);
        }
        //Unlock all movement controls
    }

    private void dogDisable(GameObject dog)
    {
        dog.GetComponent<MimicMovemenment>().enabled = false;
    }
    private void dogEnable(GameObject dog)
    {
        dog.GetComponent<MimicMovemenment>().enabled = true;
    }

    private void farmerDisable(GameObject farmer)
    {
        farmer.GetComponent<MoveBehaviour>().enabled = false;
        farmer.GetComponent<AimBehaviourBasic>().enabled = false;
        farmer.GetComponent<BasicBehaviour>().enabled = false;
    }
    private void farmerEnable(GameObject farmer)
    {
        
        farmer.GetComponent<AimBehaviourBasic>().enabled = true;
        farmer.GetComponent<BasicBehaviour>().enabled = true;
        farmer.GetComponent<MoveBehaviour>().enabled = true;
    }
    public void endGameFunction()
    {
        if (PhotonNetwork.connected && PhotonNetwork.isMasterClient)
        {
            mPhotonView.RPC("endGameFunctionNetwork", PhotonTargets.All);
        }
        else
        {
            endGameFunctionNetwork();
        }
    }
    [PunRPC]
    public void endGameFunctionNetwork()
    {
        endCanvas.active = true;
        endCanvas.GetComponentInChildren<Text>().text = "Game Over\n Number of Animals Killed:\n" + numOfKilledAnimals.ToString();
        Invoke("goBackToMenu",5f);

    }

    void goBackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }


    public void killAnimal()
    {
        if (PhotonNetwork.connected && PhotonNetwork.isMasterClient)
        {
            mPhotonView.RPC("killAnimalNetwork", PhotonTargets.All);
        }
        else
        {
            killAnimalNetwork();

        }
    }

    [PunRPC]
    void killAnimalNetwork()
    {
        numOfKilledAnimals++;
    }
}

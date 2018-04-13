/* Class: NetworkManager.cs
 * Programmer: Eric Davies
 * Date: 12/4/2018
 * Description: A class that manages most of the overhead network needs of the FarmNetwork scene. Used to set up the game and to handle the end of the game.
 * */
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

    //Allows the testers to specify which player object to turn on for testing.
    public bool useDogOffline;

    private int ReadyClients = 0;

    PhotonView mPhotonView;

    void Update()
    {
        //Open up the escape menu
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            canvas.active = !canvas.active;
            Cursor.visible = !Cursor.visible;
        }

        //Host check for instable game and end it if needed
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

    /* Function: tellReady
     * Description: A function that is called by clients for the host when a player is loaded into the scene. Calls the startgame rpc when 2 players ready.
     * */
    [PunRPC]
    void tellReady()
    {
        ReadyClients++;
        if(ReadyClients == 2)
        {
            mPhotonView.RPC("startGame", PhotonTargets.All);
        }
    }

    /* Function: endGameLocal
     * Description: A function that can be called to end the game from any client. Can handle online and offline games.
     * */
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

    /* Function: endGameNetwork
     * Description: A function called by a network client to end the game in all instances, or at least in the target machine.
     * */
    [PunRPC]
    void endGameNetwork()
    {
        SceneManager.LoadScene("Menu");
    }

    /* Function: startGame
     * Description: A function called by the host to all clients to start the game. Enables each players respective player objects.
     * */
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
    }

    /* Function: dogDisable
     * Description: A function that turns off the dogs ability to move.
     * */
    private void dogDisable(GameObject dog)
    {
        dog.GetComponent<MimicMovemenment>().enabled = false;
    }
    /*Function: dogEnable
     * Description: A function that lets the dog move again, or nothing if already enabled.
     * */
    private void dogEnable(GameObject dog)
    {
        dog.GetComponent<MimicMovemenment>().enabled = true;
    }
    /* Function: farmerDisable
     * Description: A function that stops all visible behavior of the farmer player object
     * */
    private void farmerDisable(GameObject farmer)
    {
        farmer.GetComponent<MoveBehaviour>().enabled = false;
        farmer.GetComponent<AimBehaviourBasic>().enabled = false;
        farmer.GetComponent<BasicBehaviour>().enabled = false;
    }

    /* Function: farmerEnable
     * Description: A function that starts all visible behavior on the farmer player object
     * */
    private void farmerEnable(GameObject farmer)
    {
        
        farmer.GetComponent<AimBehaviourBasic>().enabled = true;
        farmer.GetComponent<BasicBehaviour>().enabled = true;
        farmer.GetComponent<MoveBehaviour>().enabled = true;
    }
    /* Function endGameFunction
     * Description: A function called by the host to formally end the game when an end game condition is met. Tells all network object to do something.
     *  will do the same on the local level if not connected.
     * */
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
    /* Function: endGameFunctionNetwork
     * Descriotion: a function that sets up the formal end of a game. Sets the number of killed animals to be visible to the players and will swap the game
     *  back to the lobby in 5 seconds.
     * */
    [PunRPC]
    public void endGameFunctionNetwork()
    {
        endCanvas.active = true;
        endCanvas.GetComponentInChildren<Text>().text = "Game Over\n Number of Animals Killed:" + numOfKilledAnimals.ToString();
        Invoke("goBackToMenu",5f);

    }

    /* Function: goBackToMenu
     * Description: A function called in the endGameFunctionNetwork Invoke() method to return both players back to the lobby in x seconds.
     * */
    void goBackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    /* Function killAnimal
     * Description: A function that kills
     * */
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

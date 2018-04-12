using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuplicateAnimals : MonoBehaviour {

    public GameObject[] duplicateAI;
    public Transform cloneSpawn;
    public Transform[] filteredAnimals;
    public GameObject TransformGUI;
    public PhotonView mPhotonView;
	// Use this for initialization
	void Start () {
        mPhotonView = GetComponent<PhotonView>();
        CorrectAI(duplicateAI);
        duplicateAI[0].transform.parent = this.transform;
        duplicateAI[0].transform.position = transform.position;
        duplicateAI[0].transform.rotation = transform.rotation;
        GetComponent<MimicMovemenment>().m_AnimatorDriverAnimal = transform.GetChild(0).GetComponent<AnimatorDriverAnimal>();
        Physics.IgnoreCollision(duplicateAI[0].GetComponent<Collider>(), transform.GetComponent<Collider>());
        Cursor.visible = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        TransformToSelectedOption();
        CorrectRotationAndPositionOnError();
    }

    void DetachCurrentChild()
    {
        if(transform.childCount == 1)
        {
            Transform child = transform.GetChild(0);
            transform.DetachChildren();
            child.position = cloneSpawn.position;
        }
    }

    void TransformToSelectedOption()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (PhotonNetwork.connected)
            {
                mPhotonView.RPC("swapAnimal", PhotonTargets.All, 0);
            }
            else
            {
                swapAnimal(0);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (PhotonNetwork.connected)
            {
                mPhotonView.RPC("swapAnimal", PhotonTargets.All, 1);
            }
            else
            {
                swapAnimal(1);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (PhotonNetwork.connected)
            {
                mPhotonView.RPC("swapAnimal", PhotonTargets.All, 2);
            }
            else
            {
                swapAnimal(2);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (PhotonNetwork.connected)
            {
                mPhotonView.RPC("swapAnimal", PhotonTargets.All, 3);
            }
            else
            {
                swapAnimal(3);
            }
        }
    }

    [PunRPC]
    public void swapAnimal(int animalIndex)
    {
        filteredAnimals = TransformGUI.GetComponent<TransformGUI>().m_transforms;
        DetachCurrentChild();
        Physics.IgnoreCollision(filteredAnimals[animalIndex].GetComponent<Collider>(), transform.GetComponent<Collider>());
        filteredAnimals[animalIndex].transform.position = transform.position;
        filteredAnimals[animalIndex].transform.rotation = transform.rotation;
        filteredAnimals[animalIndex].transform.parent = this.transform;
        GetComponent<MimicMovemenment>().m_AnimatorDriverAnimal = transform.GetChild(0).GetComponent<AnimatorDriverAnimal>();
    }

    public void CorrectAI(GameObject[] AI)
    {
        foreach(GameObject animal in AI)
        {
            animal.GetComponent<AnimatorDriverAnimal>().enabled = false;
            if (PhotonNetwork.connected && !PhotonNetwork.isMasterClient)
            {
                mPhotonView.TransferOwnership(PhotonNetwork.player);
            }
        }
    }

    void CorrectRotationAndPositionOnError()
    {
        if(transform.rotation != transform.GetChild(0).rotation || transform.position.x != transform.GetChild(0).position.x || transform.position.z != transform.GetChild(0).position.z)
        {
            Transform child = transform.GetChild(0);
            child.rotation = transform.rotation;
            child.position = new Vector3(transform.position.x, child.position.y, transform.position.z);
        }
    }
}

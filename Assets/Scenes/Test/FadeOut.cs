using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour {

    public float fadeTime = 5f;
    private float timeSinceStart = 0f;
    private Material m_mat;
    public Shader tmpShader;
	// Use this for initialization
	void Start () {
        m_mat = GetComponentInChildren<Renderer>().material;
        tmpShader = Shader.Find("Legacy Shaders/Transparent/Bumped Diffuse");
        m_mat.shader = tmpShader;
	}
	
	// Update is called once per frame
	void Update () {
        timeSinceStart += Time.deltaTime;
        if (timeSinceStart >= fadeTime)
        {
            if (PhotonNetwork.connected && PhotonNetwork.isMasterClient)
            {
                GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.player);
                    PhotonNetwork.Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            m_mat.color =  new Color(1f, 1f,1f, (fadeTime - timeSinceStart) / fadeTime);
        }

	}
}

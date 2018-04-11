using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_CanvasRotate : MonoBehaviour {

    Transform cameraTransform;

    void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - cameraTransform.position);
    }
}

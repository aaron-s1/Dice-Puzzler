using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCurrentCamera : MonoBehaviour
{

    [SerializeField] Transform testObj;
    [SerializeField] Vector3 targetCameraPos;
    [SerializeField] Vector3 offset;
    public Camera currentCamera;
    


    void Start() =>
        currentCamera = ChangeCameras.Instance.currentCamera;
    
    void FixedUpdate() {
        // Vector3 pos = currentCamera.WorldToScreenPoint(testObj.position); 

        // if (transform.position != pos)
        //     transform.position = pos;
    }
}

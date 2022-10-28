using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCameras : MonoBehaviour
{
    public Camera currentCamera;

    public static ChangeCameras Instance { get; private set; }

    [Space(10)]
    [SerializeField] Camera mainCamera;
    [SerializeField] Camera backCamera;
    [SerializeField] Camera rightCamera;
    [SerializeField] Camera leftCamera;
    

    void Awake() {
        Instance = this;
        currentCamera = mainCamera = Camera.main;
    }

    void Update() {
        // if (Input.GetKeyDown(KeyCode.Keypad5) || (Input.GetKeyDown(KeyCode.Keypad2)) )
        //     SwapToCamera(mainCamera);

        // if (Input.GetKeyDown(KeyCode.Keypad8))
        //     SwapToCamera(backCamera);

        // if (Input.GetKeyDown(KeyCode.Keypad6))
        //     SwapToCamera(rightCamera);

        // if (Input.GetKeyDown(KeyCode.Keypad4))
        //     SwapToCamera(leftCamera);
    }

    
    void SwapToCamera(Camera newCamera) {
        if (newCamera == currentCamera)
            return;
            
        newCamera.enabled = true;
        currentCamera.enabled = false;
        currentCamera = newCamera;        
    }
   
}

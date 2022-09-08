using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideTouchedGround : MonoBehaviour
{

    public GameManager gameManager;

    void Awake()
    {
        // gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // void OnTriggerEnter(Collider other) {

    // }

    void OnTriggerEnter(Collider other)
    {
        // if (other.gameObject.tag.Contains("Ground")) {
        //     Debug.Log("side hit ground");
        //     gameManager.sideHitValue = int.Parse(gameObject.name);
        //     Debug.Log(gameManager.sideHitValue);
        // }
    }
}

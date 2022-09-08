using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideCornerTouchedGround : MonoBehaviour
{
    public GameManager gameManager;

    void Awake() 
    {
        
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == 6)
        {
            Debug.Log("Tomato");
        }
    }
}

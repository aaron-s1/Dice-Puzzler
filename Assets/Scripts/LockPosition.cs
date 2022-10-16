using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPosition : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] Vector3 offsetFromPlayer;

    Transform playerPos;


    void FixedUpdate() 
    {
        playerPos = player.transform;
        transform.position = new Vector3(playerPos.position.x + offsetFromPlayer.x, playerPos.position.y + offsetFromPlayer.y, playerPos.position.z + offsetFromPlayer.z);
    }
}

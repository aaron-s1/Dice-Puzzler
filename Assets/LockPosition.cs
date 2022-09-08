using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPosition : MonoBehaviour
{
    public Vector3 offsetFromPlayer;
    public GameObject player;
    public Transform playerPos;
    public Renderer playerRenderer;

    void Awake()
    {
        // player = transform.parent.gameObject;
        // playerRenderer = player.GetComponent<Renderer>();
    }

    void Update()
    {
    }

    void FixedUpdate() 
    {
        playerPos = player.transform;
        transform.position = new Vector3(playerPos.position.x + offsetFromPlayer.x, playerPos.position.y + offsetFromPlayer.y, playerPos.position.z + offsetFromPlayer.z);
        // transform.position = new Vector3 (playerRenderer.bounds.center.x, playerRenderer.bounds.center.y + 1.207f, playerRenderer.bounds.center.z);
        // transform.position = new Vector3 (0, -0.75f, 0);
    }
}

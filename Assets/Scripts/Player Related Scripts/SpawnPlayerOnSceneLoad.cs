using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayerOnSceneLoad : MonoBehaviour
{
    GameObject player;
    Player playerMove;    
    Rigidbody playerRigid;
    
    RaycastHit hit;

    void Start() 
    {
        // player = PlayerMove.Instance.gameObject;
        // player.SetActive(true);
        // playerMove = player.GetComponent<PlayerMove>();
        // playerRigid = player.GetComponent<Rigidbody>();
        // FlipRigidbody();
        


        // activate spawn particle.
        // playerMove = player.GetComponent<PlayerMove>();

        // if (playerMove.RaycastSeesNormalTile(new Vector3 (0, -1f, 0), 1.5f))
        // {

        // }




        // if (Physics.Raycast(transform.position, new Vector3 (0, -1f, 0), out hit, 1.5f, 1 << 6)) {
        //     var centerTile = hit.transform.gameObject;
        //     FlipRigidbody();            
        // }        




        // if (playerMove.RaycastSeesNormalTile(new Vector3(0f, -1f, 0), 5f)) 
        // {
        //     var tileHit = hit.transform.gameObject;
        //     playerMove.DisableAndReplaceTile()
        //     player.GetComponent<Renderer>().enabled = true;
        // }
    }


    void FlipRigidbody()
    {
        playerRigid.useGravity = !playerRigid.useGravity;
        playerRigid.isKinematic = !playerRigid.isKinematic;
    }
}

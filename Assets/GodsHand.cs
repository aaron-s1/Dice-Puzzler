using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodsHand : MonoBehaviour
{    
    Vector3 newTilePos;
    float y_offset = 0.5f;

    // PlayerMove playerMove;

    // void Awake() {
    //     playerMove = PlayerMove.Instance;
    // }

    void Update() {
        if (Input.GetKey(KeyCode.Space)) {   // change if condition later.   
            Debug.Log(PlayerMove.Instance.movePlayer); 
            if (PlayerMove.Instance.movePlayer == null) {
                // StopCoroutine(PlayerMove.Instance.movePlayer);
                FindRandomDestroyableTile();
            }
        }
    }


    void FindRandomDestroyableTile()
    {
        var totalTiles = GameManager.Instance.listOfDestroyableTiles.Count;
        int randomNumber = Random.Range(0, totalTiles);
        newTilePos = GameManager.Instance.listOfDestroyableTiles[randomNumber].transform.position;
        newTilePos = new Vector3 (newTilePos.x, newTilePos.y + y_offset, newTilePos.z);
        SetNewPlayerPos();        
        // Note: 'cube' names do not match array value when testing --> Debug.Log(newTile); 
    }

    void SetNewPlayerPos() {
        //  \/ these are public just for now. clean up later.
        if (PlayerMove.Instance.LandedOnNormalTile())
            PlayerMove.Instance.ReplaceWithSpecialTile();
        PlayerMove.Instance.transform.position = newTilePos;    
    }
}

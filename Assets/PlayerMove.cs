using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    [SerializeField] float timeToMove = 1f;
    [SerializeField] float timeToRotate = 0.5f;

    [SerializeField] GameObject specialTile;    // rename later.
    [SerializeField] Transform bottomOfPlayer;

    Vector3 originalPos;
    Vector3 targetPos;
    Vector3 moveDirection;
    Vector3 raycastDownwards;

    Quaternion originalRotation;
    Quaternion targetRotation;

    RaycastHit hit;

    bool isMoving;
    bool fireDestroyTilesRay;

    int destroyTilesRayLength;
    int totalTilesToDestroy;
    int tilesHitCount = 0;

    int validMovementTileLayers;

    
    void Start()
    {        
        validMovementTileLayers = 1 << 6 | 1 << 9;

        moveDirection = new Vector3(0,0,0);
        raycastDownwards = new Vector3 (0, -1f, 0);

        destroyTilesRayLength = 1;
    }


    #region Movement.

    void Update() => MovementInputs();


    void MovementInputs()
    {
        if (!isMoving)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                originalRotation = transform.rotation;
                targetRotation = Quaternion.Euler(-90f, 0, 0) * transform.rotation;

                StartCoroutine(MovePlayer(moveDirection = Vector3.back));
            }

            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                originalRotation = transform.rotation;
                targetRotation = Quaternion.Euler(90f, 0, 0) * transform.rotation;

                StartCoroutine(MovePlayer(moveDirection = Vector3.forward));
            }

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                originalRotation = transform.rotation;
                targetRotation = Quaternion.Euler(0, 0, -90f) * transform.rotation;

                StartCoroutine(MovePlayer(moveDirection = Vector3.right));
            }

            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                originalRotation = transform.rotation;
                targetRotation = Quaternion.Euler(0, 0, 90f) * transform.rotation;

                StartCoroutine(MovePlayer(moveDirection = Vector3.left));
            }
        }
    }


    IEnumerator MovePlayer(Vector3 direction)
    {
        if (!Physics.Raycast(bottomOfPlayer.position, moveDirection, out hit, 1f, validMovementTileLayers))        
        {
            Debug.DrawRay(bottomOfPlayer.position, moveDirection, Color.red);
            Debug.Log("MOVE PLAYER DID NOT FIND VALID TILE");
            yield break;
        }
        
        isMoving = true;

        float elapsedTime = 0;
        originalPos = transform.position;
        targetPos = originalPos + direction;
   
        while (elapsedTime < timeToMove)
        {            
            transform.position = Vector3.Lerp(originalPos, targetPos, (elapsedTime / timeToMove));            
            transform.rotation = Quaternion.Lerp(originalRotation, targetRotation, (elapsedTime / timeToRotate));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // Arrived at destination.
        transform.position = targetPos;
        transform.rotation = targetRotation;


        // Only destroy/change tiles if landing on a regular tile.
        if (Physics.Raycast(transform.position, raycastDownwards, out hit, 2f, 1 << 6)) {
            fireDestroyTilesRay = true;
            yield return new WaitUntil(() => !fireDestroyTilesRay);
        }



        isMoving = false;
        // Debug.Log("movement ended.");
        yield return null;
    }

    #endregion


    void FixedUpdate() {
        if (fireDestroyTilesRay)
        {
            totalTilesToDestroy = GetNumberOfSideTouchingTile();    // prevent mass re-caching later.

            // rayLength starts at 1 to first check if an appropriate tile is hit.
            // if it is, *then* raycast towards numerical value of dice side.

            if (Physics.Raycast(bottomOfPlayer.position, moveDirection, out hit, destroyTilesRayLength, 1 << 6))
            {
                var tileHit = hit.transform.gameObject;
                RemoveTile(tileHit);
                // GameManager.Instance.RemoveFromToTileList(tileHit);
                // tileHit.SetActive(false);
                tilesHitCount++;

                if (destroyTilesRayLength < totalTilesToDestroy)
                    destroyTilesRayLength++;

                if (tilesHitCount == totalTilesToDestroy)
                    Instantiate(specialTile, tileHit.transform.position, tileHit.transform.rotation);

                
                if (Physics.Raycast(transform.position, raycastDownwards, out hit, 2f, 1 << 6)) {
                    tileHit = hit.transform.gameObject;
                    RemoveTile(tileHit);
                    // GameManager.Instance.RemoveFromToTileList(tileHit);
                    // tileHit.SetActive(false);
                    Instantiate(specialTile, tileHit.transform.position, tileHit.transform.rotation);
                }
                
            }

            else
            {
                fireDestroyTilesRay = false;
                destroyTilesRayLength = 1;
                tilesHitCount = 0;
            }
        }
    }


    int GetNumberOfSideTouchingTile()
    {
        // Currently assumes names of dice sides  are just "4", "3", etc.
        if (Physics.Raycast(transform.position, raycastDownwards, out hit, 2f, 1 << 7))
        {
            totalTilesToDestroy = int.Parse(hit.transform.gameObject.name);
            return totalTilesToDestroy;
        }
        return 0;
    }


    void RemoveTile(GameObject tile)
    {
        GameManager.Instance.RemoveFromTileList(tile);
        tile.SetActive(false);
    }    


    // bool TouchingSpecialTile()
    // {
    //     if (Physics.Raycast(transform.position, raycastDownwards, out hit, 2f, 1 << 9))
    //         return true;
    //     else return false;        
    // }


    IEnumerator RotatePlayer(float elapsedTime)
    {
        transform.rotation = Quaternion.Lerp(originalRotation, targetRotation, (elapsedTime / timeToRotate));
        yield return null;
    }
}

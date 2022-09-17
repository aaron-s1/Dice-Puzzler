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
    int destroyableTilesHitCount = 0;

    int validMovementTileLayers;

    
    void Start()
    {        
        validMovementTileLayers = 1 << 6 | 1 << 9;

        moveDirection = new Vector3(0,0,0);
        raycastDownwards = new Vector3 (0, -1f, 0);

        destroyTilesRayLength = 1;
    }


    #region Movement.

    void Update() => MovementInput();


    void MovementInput()
    {
        if (!isMoving)
        {
            // reduce lines later.
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
        if (!ValidMovementFound())
            yield break;
        
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

        
        if (LandedOnNormalTile())
        {
            fireDestroyTilesRay = true;
            yield return new WaitUntil(() => !fireDestroyTilesRay);
        }



        isMoving = false;
        // Debug.Log("movement ended.");
        yield return null;
    }


    bool ValidMovementFound() 
    {
        if (Physics.Raycast(bottomOfPlayer.position, moveDirection, out hit, 1f, validMovementTileLayers)) {
            Debug.Log("MOVE PLAYER DID NOT FIND VALID TILE");
            return true;
        }
        return false;
    }    

    #endregion

    bool LandedOnNormalTile()
    {
        if (Physics.Raycast(transform.position, raycastDownwards, out hit, 2f, 1 << 6))
            return true;
        return false;
    }

    
    bool RaycastSeesNormalTile(Vector3 direction, float rayLength) {
        if (Physics.Raycast(bottomOfPlayer.position, direction, out hit, rayLength, 1 << 6))
            return true;
        return false;
    }


    void FixedUpdate() {
        if (fireDestroyTilesRay)
        {
            totalTilesToDestroy = GetNumberOfSideTouchingTile();    // prevent mass re-caching later.

            // destroyTilesRayLength starts at 1 in order to first check if any tile is hit.
            // if so, increment it until totalTilesToDestroy is reached.
            if (RaycastSeesNormalTile(moveDirection, destroyTilesRayLength))
            {
                var tileHit = hit.transform.gameObject;

                // if (Physics.Raycast(transform.position, raycastDownwards, out hit, 2f, 1 << 6)) {
                if (LandedOnNormalTile()) {
                    var tileLandedOn = hit.transform.gameObject;
                    GameManager.Instance.RemoveFromTileList(tileLandedOn, true);     // becomes special tile
                    Instantiate(specialTile, tileLandedOn.transform.position, tileLandedOn.transform.rotation);
                }
                
                destroyableTilesHitCount++;

                if (destroyTilesRayLength < totalTilesToDestroy)
                    destroyTilesRayLength++;

                if (destroyableTilesHitCount == totalTilesToDestroy) {
                    Instantiate(specialTile, tileHit.transform.position, tileHit.transform.rotation);
                    GameManager.Instance.RemoveFromTileList(tileHit, true);
                }

                else GameManager.Instance.RemoveFromTileList(tileHit);
            }

            else
            {
                fireDestroyTilesRay = false;
                destroyTilesRayLength = 1;
                destroyableTilesHitCount = 0;
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


    IEnumerator RotatePlayer(float elapsedTime)
    {
        transform.rotation = Quaternion.Lerp(originalRotation, targetRotation, (elapsedTime / timeToRotate));
        yield return null;
    }
}

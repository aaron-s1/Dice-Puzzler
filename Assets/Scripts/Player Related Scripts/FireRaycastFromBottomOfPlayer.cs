using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRaycastFromBottomOfPlayer : MonoBehaviour
{
    public GameManager gameManager;
    // Vector3 direction;    
    public float rayLength;


    RaycastHit hit;
    // List<RaycastHit> hits;
    RaycastHit[] hits;
    public List<GameObject> modifiableTiles;

    public bool canFireRay;

    Vector3 raycastDirection;

    IEnumerator TestingRaycastJump()
    {
        yield return null;
    }

    public IEnumerator FireRay2(Vector3 direction)
    {
        raycastDirection = direction;
        canFireRay = true;
        yield return null;
    }


    [SerializeField] GameObject replacementTile;
    
    bool rayInfoIsNeeded;

    bool checkIfMovementIsValid;

    public IEnumerator FireRay(Vector3 direction, bool checkingForValidMovement = false)
    {
        Debug.Log("ray METHOD fired");
        raycastDirection = direction;

        checkIfMovementIsValid = checkingForValidMovement;
        rayInfoIsNeeded = true;
        
        yield return new WaitUntil(() => raycastHasEnded);

        // rayLength = 0;
        yield return null;

    }

    // public Vector3 raycastDirection;

    void FixedUpdate() 
    {
        // if (canFireRay)
        // {
        //     Debug.DrawRay(transform.position, raycastDirection * rayLength, Color.green);
        //     // if (Physics.Raycast(transform.position, testRayVector3, 1f, 6))
        //     if (Physics.Raycast(transform.position, raycastDirection * rayLength, out hit, 6))
        //     {
        //         if (checkIfMovementIsValid) {
                    
        //         }
        //         Debug.Log("hit a tile");                
        //         hit.transform.gameObject.SetActive(false);
        //         Debug.Log("tile deleted = " + hit.transform.gameObject);
        //     }           

        //     else {
        //         rayLength = 0;

        //         currentlyTouchedFloorTile.layer = 6;

        //         canFireRay = false;
        //         raycastHasEnded = true;
        //         // return;
        //     }
        // }
    }

    GameObject currentlyTouchedFloorTile;
    int currentSideNumber;

    bool raycastHasEnded;


    
    void OnTriggerStay(Collider other)
    {
        // if (rayInfoIsNeeded)
        // {
        //     Debug.Log("stay triggered");
        //     GameObject touchedObject = other.gameObject;

        //     // Make currently touched tile immune to raycasts.
        //     if (touchedObject.layer == 6)       // DestroyableTile
        //     {                
        //         currentlyTouchedFloorTile = touchedObject;
        //         currentlyTouchedFloorTile.layer = 2;
        //         // currentlyTouchedFloorTile.GetComponent<Collider>().enabled = false;

        //         Debug.Log("current tile touched new layer = " + currentlyTouchedFloorTile.layer);
        //     }
            
        //     if (checkIfMovementIsValid)
        //         rayLength = 1; // change to 2?

        //     else if (other.gameObject.layer == 7)    // DiceSide
        //     {
        //         currentSideNumber = int.Parse(touchedObject.name);  // (assumes side names are just "5", 6", etc)
        //         rayLength = currentSideNumber;
        //         Debug.Log("current side number = " + currentSideNumber);
        //     }
                
        //     rayInfoIsNeeded = false;
        //     canFireRay = true;

        // }
            // rayInfoIsNeeded = false;
        // if (gameManager.canCastRay) {        
        //     if (other.gameObject.layer == LayerMask.NameToLayer("Ground")) {
        //         Debug.Log("touched ground");
        //         currentTileTouched = other.gameObject;

        //         // hits = Physics.RaycastAll(transform.position, direction, rayLength);
        //     }
        // }
    }

    // void OnTriggerEnter(Collider other)
    // {
    //     if (other.gameObject.layer == LayerMask.NameToLayer("Ground")) {
    //         Debug.Log("touched ground");
    //         currentTileTouched = other.gameObject;
    //         // hits = Physics.RaycastAll(transform.position, direction, rayLength);
    //     }
    // }

    // void OnTriggerExit(Collider other)
    // {
    //     // if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
    //     //     currentTileTouched = null;
    // }
}


        // float totalTilesHit = 0;

        // for (float i = 0; i < rayLength; i++)
        // {
        //     GameObject tileHit;
        //     Debug.Log("for loop reached");

        //     // If there are no tiles in that direction, return.
        //     if (!Physics.Raycast(transform.position, raycastDirection, out hit, 1f, 6))
        //         yield return null;

        //     // Otherwise, cast in that direction with a length equal to the side that's landed on.            
        //     else if (Physics.Raycast(transform.position, raycastDirection, out hit, rayLength)) {
        //         tileHit = hit.transform.gameObject;

        //         // Debug.DrawRay(transform.position, passedInDirection * rayLength, Color.green);

        //         // modifiableTiles.Add(tileHit);
        //         tileHit.SetActive(false);
        //         totalTilesHit++;
        //         // currentTileTouched.layer = 6;
        //         // hit.transform.gameObject.SetActive(false);

        //         if (totalTilesHit == rayLength)
        //         {
        //             Instantiate(replacementTile, tileHit.transform.position, tileHit.transform.rotation);
        //             Debug.Log("replaced tile");
        //         }
        //         // Debug.Log("tile hit = " + hit.transform.gameObject);
        //         // continue;
        //     }

        //     currentlyTouchedFloorTile.layer = 6;
        //     // currentlyTouchedFloorTile.GetComponent<Collider>().enabled = false;
        //     totalTilesHit = 0;
        //         // else StopCoroutine("FireRay");    

        //     // else {
        //     //     currentTileTouched.layer = 6;
        //     //     yield break;
        //     // }
        //     //////////////////


        //     // currentTilePosition = hit.transform.position;
            
        //     // else {
        //     // Debug.Log("for loop broke");
        //     // break;
        //     // }
        // }

        // if (Physics.Raycast(transform.position, direction, 1f, LayerMask.NameToLayer("Ground")))
        // {
        // }

        // foreach (RaycastHit h in hits)
        // {
        //     GameObject tile = h.transform.gameObject;

            
        //     if (tile != currentTileTouched)     // wouldn't want to disable the floor below us, would we?
        //     {
        //         // account for if touching last thing ray hits later.
        //         modifiableTiles.Add(hit.transform.gameObject);
        //         Debug.Log("hit tiles = " + modifiableTiles);
        //     }
        // }

        // rayLength = 0;

        // yield return null;
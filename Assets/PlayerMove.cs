using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // Vector3 originalPos, targetPos;

    [SerializeField] FireRaycastFromBottomOfPlayer raycastFromBottomOfPlayer;
    [SerializeField] Transform bottomOfPlayer;
    [SerializeField] GameObject specialTile; // rename later.
    public GameManager gameManager;
    public Renderer renderer;
    // public float multiplierOffset;
    // public float additiveOffset;
    Vector3 center;

    public bool isMoving;
    public Vector3 originalPos, targetPos;
    public float timeToMove = 1f;
    public float timeToRotate = 0.5f;

    Vector3 moveDirection;
    
    void Start()
    {        
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        center = transform.position;
        renderer = GetComponent<Renderer>();

        moveDirection = new Vector3(0,0,0);
        raycastDownwardDirection = new Vector3 (0, -1f, 0);
    }

    public Quaternion rotationVector;
    


    void Update()
    {
        // transform.rotation *= Quaternion.AngleAxis(1f, Vector3.right);
        if (!isMoving) {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {                
                originalRotation = transform.rotation;                
                targetRotation =  Quaternion.Euler(-90f, 0, 0) * transform.rotation;

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
                targetRotation =  Quaternion.Euler(0, 0, 90f) * transform.rotation;

                StartCoroutine(MovePlayer(moveDirection = Vector3.left));
            }
        }
    }

    bool canMove;

    Vector3 raycastDownwardDirection;

    bool findingCurrentlyTouchedTile;

    bool checkingForValidMovement;
    GameObject currentlyTouchedTile;


    int diceSideNumberHit;
    int lengthOfDestroyTilesRaycast;


    int GetTileSideNumber()
    {
        // Layer 7 is DiceSide. Cast a ray from center, downward, to find current side hitting ground.
        // Assumes side names are just "4", "3", etc.
        if (Physics.Raycast(transform.position, raycastDownwardDirection, out hit, 2f, 1 << 7))
        {
            lengthOfDestroyTilesRaycast = int.Parse(hit.transform.gameObject.name);
            return lengthOfDestroyTilesRaycast;
        }
        return 0;
    }

    void AllowRaycastsToHitCurrentlyTouchedTile(bool allow) {

    }

    void FixedUpdate() {
        // if (checkForAllowedMovement && !canMove) {
        if (checkingForValidMovement)
        {
            // if (Physics.Raycast(bottomOfPlayer.position, raycastDownwardDirection, out hit, 1f, 6))
            // {
            //     currentlyTouchedTile = hit.transform.gameObject;
            //     currentlyTouchedTile.layer = 2;
            // }            
            
            // Time.timeScale = 0.025f;
            // Debug.DrawRay(bottomOfPlayer.position, moveDirection, Color.red);

            
            // if (Physics.Raycast(bottomOfPlayer.position, moveDirection, 
            if (Physics.Raycast(bottomOfPlayer.position, moveDirection, out hit, 1f, 1 << 6))  // increase length up from 1?
            {
                Debug.Log(hit.transform.gameObject);
                // hit.transform.gameObject.SetActive(false);
                canMove = true;
            }
            checkingForValidMovement = false;
            Debug.Log("checking for valid movement false");
        }
                // if (Physics.Raycast(bottomOfPlayer.position, raycastDownwardDirection, out hit, 1f, 6)) {
                    // currentlyTouchedTile = hit.transform.gameObject;
                    // currentlyTouchedTile.layer = 2;
                // }

        //

        if (fireDestroyTilesRay)
        {
            // fireDestroyTilesRay = false;
            if (Physics.Raycast(bottomOfPlayer.position, raycastDownwardDirection, out hit, 1f, 1 << 6))
            {
                Debug.Log("tile layer changed");
                currentlyTouchedTile = hit.transform.gameObject;
                currentlyTouchedTile.layer = 2;
            }


            int tilesHitCount = 0;
            lengthOfDestroyTilesRaycast = GetTileSideNumber();
            Debug.Log("length of tiles ray = " + lengthOfDestroyTilesRaycast);

            if (Physics.Raycast(bottomOfPlayer.position, moveDirection, out hit, lengthOfDestroyTilesRaycast, 1 << 6))
            {
                Debug.Log("side raycast triggered");
                var tileHit = hit.transform.gameObject;
                tileHit.SetActive(false);
                tilesHitCount++;

                if (tilesHitCount == lengthOfDestroyTilesRaycast) {
                    Debug.Log("instantiate at end triggered");
                    Instantiate(specialTile, tileHit.transform.position, tileHit.transform.rotation);
                }
            }

            else {
                Debug.Log("fire destroy ray Else triggered");
                tilesHitCount = 0;
                fireDestroyTilesRay = false;
                currentlyTouchedTile.layer = 6;
            }
        }
    }

    
    
    Quaternion originalRotation;
    Quaternion targetRotation;

    
    // public Vector3 rotationDirection;

    Coroutine bottomOfPlayerFireRay;
    
    bool checkForAllowedMovement;

    bool fireDestroyTilesRay;

    IEnumerator MovePlayer(Vector3 direction)
    {
        checkingForValidMovement = true;
        yield return new WaitUntil(() => !checkingForValidMovement);
        Debug.Log("move player checking for valid movement returned false");

        if (!canMove) {
            yield break;
        }

        Debug.Log("can move");
        
        
        
        // checkForAllowedMovement = true;

        // if (!canMove) {
        //     checkIfCanMove = true;
        // }
        //     yield return null;
        
        // StartCoroutine(raycastFromBottomOfPlayer.CheckForValidMovement(moveDirection));

        // bottomOfPlayerFireRay = StartCoroutine(raycastFromBottomOfPlayer.FireRay(direction, true));
        // raycastFromBottomOfPlayer.GetComponent<FireRaycastFromBottomOfPlayer>().FireRay(moveDirection, true));
        // targetRotation = direction * 90f;
        isMoving = true;

        float elapsedTime = 0;
        originalPos = transform.position;
        targetPos = originalPos + direction;
   
        // originalRotation = transform.rotation;
        // targetRotation = originalRotation * Quaternion.Euler(rotationDirection * 90f);


        while (elapsedTime < timeToMove)
        {            
            // RotatePlayer(elapsedTime);
            transform.position = Vector3.Lerp(originalPos, targetPos, (elapsedTime / timeToMove));            
            // transform.rotation *= targetRotation;            
            transform.rotation = Quaternion.Lerp(originalRotation, targetRotation, (elapsedTime / timeToRotate));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        
        transform.position = targetPos;
        transform.rotation = targetRotation;

        // Arrived at destination.
        fireDestroyTilesRay = true;
        Debug.Log("fire destroy rays");
        yield return new WaitUntil(() => !fireDestroyTilesRay);
        Debug.Log("fire destroy rays finished waiting");

        
        // yield return bottomOfPlayer.GetComponent<FireRaycastFromBottomOfPlayer>().FireRay(direction);

        // Debug.Log("before component to fire ray fired");
        // bottomOfPlayerFireRay = StartCoroutine(raycastFromBottomOfPlayer.FireRay(direction, false));
        // bottomOfPlayer.GetComponent<FireRaycastFromBottomOfPlayer>().FireRay(moveDirection);
        
                
        // Debug.Log("after component to fire ray fired");
        // bottomOfPlayer.GetComponent<FireRaycastFromBottomOfPlayer>().FireRay2(moveDirection);

        // canCastRay = true;
        // rayLength = 1f;

        // specifically only allow ray to cast for one frame.
        // hits = Physics.RaycastAll(transform.position, direction, rayLength, LayerMask.NameToLayer("Ground"));
        //     foreach (var h in hits)
        //     {
        //         hitTiles.Add(hit.transform.gameObject);
        //         Debug.Log("hit tiles = " + hitTiles);                
        //     }

        Debug.Log("is moving is false");

        isMoving = false;

        yield return null;
    }

    RaycastHit hit;
    RaycastHit[] hits;
    List<GameObject> hitTiles;

    float rayLength;
    bool canCastRay;

    // void FixedUpdate()
    // {
    //     // if (canCastRay)
    //     // {
    //     //     hits = Physics.RaycastAll(transform.position, direction, rayLength, LayerMask.NameToLayer("Ground"));
    //     //     foreach (var h in hits)
    //     //     {
    //     //         hitTiles.Add(hit.transform.gameObject);
    //     //         Debug.Log("hit tiles = " + hitTiles);                
    //     //     }

    //     //     canCastRay = false;
    //     // }
    // }

    IEnumerator RotatePlayer(float elapsedTime)
    {
        transform.rotation = Quaternion.Lerp(originalRotation, targetRotation, (elapsedTime / timeToRotate));
        yield return null;
    }

    void OnDrawGizmosSelected()
    {
        // Gizmos.color = new Color(1, 0, 0, 0.5f);
        // Gizmos.DrawCube(new Vector3(renderer.bounds.center.x, (renderer.bounds.center.y * multiplierOffset) + additiveOffset, renderer.bounds.center.z), new Vector3(0.04f, 0.04f, 0.04f));

        // Gizmos.DrawCube(transform.position, new Vector3(1.01f, 1.01f, 1.01f));
        // Gizmos.DrawCube(new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z), new Vector3(0.1f, 0.1f, 0.1f));
    }


    
}

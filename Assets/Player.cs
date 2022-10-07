using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public static Player Instance { get; private set; }

    [SerializeField] [Range(0.05f, 1f)] float timeToMove = 1f;
    [SerializeField] [Range(0.05f, 1f)] float timeToRotate = 0.5f;

    [SerializeField] GameObject specialTile;    // rename later.
    [SerializeField] Transform raycastDirectionOrigin;

    public Coroutine movePlayer;

    Vector3 originalMovePos;
    Vector3 targetMovePos;
    Vector3 moveDirection;

    Vector3 raycastDownwards;

    Quaternion originalRotation;
    Quaternion targetRotation;

    public RaycastHit hit;

    public bool isMoving;
    bool fireDestroyTilesRay;

    int destroyTilesRayLength;
    int totalTilesToDestroy;
    int destroyableTilesHitCount = 0;

    Rigidbody rigid;

    int validMovementTileLayers;

    void Awake() =>
        Instance = this;

    
    void Start()
    {
        
        validMovementTileLayers = 1 << 6 | 1 << 9;

        moveDirection = new Vector3(0, 0, 0);
        raycastDownwards = new Vector3 (0, -1f, 0);

        destroyTilesRayLength = 1;
        tilesDestroyed = 0; // CHANGE LATER.

        // GetComponent<Renderer>().enabled = true;
        // rigid = GetComponent<Rigidbody>();
        // FlipRigidbody();
    }


    #region Movement.

    void Update() =>
        MovementInput();


    void MovementInput()
    {
        if (!isMoving)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {   
                RotateTowards(new Vector3 (-90f, 0, 0));             
                movePlayer = StartCoroutine(MovePlayer(moveDirection = Vector3.back));
            }

            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                RotateTowards(new Vector3 (90f, 0, 0));
                movePlayer = StartCoroutine(MovePlayer(moveDirection = Vector3.forward));
            }

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                RotateTowards(new Vector3 (0, 0, -90f));
                movePlayer = StartCoroutine(MovePlayer(moveDirection = Vector3.right));
            }

            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                RotateTowards(new Vector3 (0, 0, 90f));
                movePlayer = StartCoroutine(MovePlayer(moveDirection = Vector3.left));          
            }
        }
    }


    void RotateTowards(Vector3 rotation)
    {
        originalRotation = transform.rotation;
        targetRotation = Quaternion.Euler(rotation) * transform.rotation;
    }


    IEnumerator MovePlayer(Vector3 direction)
    {
        if (!ValidMovementFound())
            yield break;
        
        isMoving = true;

        float elapsedTime = 0;
        originalMovePos = transform.position;
        targetMovePos = originalMovePos + direction;
   
        while (elapsedTime < timeToMove)
        {            
            transform.position = Vector3.Lerp(originalMovePos, targetMovePos, (elapsedTime / timeToMove));            
            transform.rotation = Quaternion.Lerp(originalRotation, targetRotation, (elapsedTime / timeToRotate));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // Arrived at destination.

        transform.position = targetMovePos;
        transform.rotation = targetRotation;

        
        
        // Only flip tile landed on if 'fireDestroyTilesRay' would kill tiles.
        if (RaycastSeesNormalTile(moveDirection, destroyTilesRayLength))
        {
            if (LandedOnNormalTile()) {
                DisableAndReplaceTile(tileHit, true);

                fireDestroyTilesRay = true;
                yield return new WaitUntil(() => !fireDestroyTilesRay);
            }
        }

        movePlayer = null;
        isMoving = false;

        yield return null;
    }


    bool ValidMovementFound() 
    {
        if (Physics.Raycast(raycastDirectionOrigin.position, moveDirection, out hit, 1f, validMovementTileLayers))
            return true;
            
        Debug.Log("PLAYER DID NOT FIND VALID TILE");
        return false;
    }    

    #endregion

    public bool LandedOnNormalTile()
    {
        if (Physics.Raycast(transform.position, raycastDownwards, out hit, 2f, 1 << 6)) {            
            tileHit = hit.transform.gameObject;
            // DisableTile(tileHit, true);
            // GameManager.Instance.currentTileTouched = hit.transform.gameObject;
            return true;
        }
        return false;
    }

    
    public bool RaycastSeesNormalTile(Vector3 direction, float rayLength) {
        if (Physics.Raycast(raycastDirectionOrigin.position, direction, out hit, rayLength, 1 << 6)) {
            tileHit = hit.transform.gameObject;
            return true;
        }
        else {
            // tileHit = null;
            return false;
        }
    }

    GameObject tileHit;


    int tilesDestroyed;

    void DisableTileDestructionRaycast() 
    {
        tileHit = null;
        destroyTilesRayLength = 1;                    
        tilesDestroyed = 0;
        fireDestroyTilesRay = false;        
    }


    // sort of movement related.

    void FixedUpdate() {
        if (fireDestroyTilesRay)
        {
            totalTilesToDestroy = SideNumberMinusAdjustment();    // prevent mass re-caching later.            

            // Destroy tiles, consecutively, in moveDirection.
            // If 'totalTilesToDestroy' is reached, replace the last tile touched with a special one.
            if (RaycastSeesNormalTile(moveDirection, destroyTilesRayLength))
            {
                tileHit = hit.transform.gameObject;

                if (tilesDestroyed == totalTilesToDestroy)
                {
                    DisableAndReplaceTile(tileHit, true);
                    tileHit.GetComponent<FirePoofParticles>().Fire(true);

                    DisableTileDestructionRaycast();                    
                }

                else if (tilesDestroyed < totalTilesToDestroy)
                {
                    DisableAndReplaceTile(tileHit, false);
                    tileHit.GetComponent<FirePoofParticles>().Fire(false);

                    tilesDestroyed++;
                    destroyTilesRayLength++;
                }
            }

            else {
                tileHit = null;
                destroyTilesRayLength = 1;                    
                tilesDestroyed = 0;
                fireDestroyTilesRay = false;
            }
        }
    }


    // not movement related.

    public void DisableAndReplaceTile(GameObject tile, bool replaceWithSpecialTile = false) {
        Debug.Log("replaced");
        // tile.GetComponent<FirePoofParticles>().Fire(replaceWithSpecialTile);
        GameManager.Instance.RemoveFromDestroyableTilesList(tile);

        tile.SetActive(false);

        if (replaceWithSpecialTile) 
        {
            Debug.Log("should spawn special tile");
            var spawnedSpecialTile = Instantiate(specialTile, tile.transform.position, tile.transform.rotation);
            spawnedSpecialTile.transform.parent = GameManager.Instance.replacedTiles.transform;
        }
    }


    // sort of movement related.

    int SideNumberMinusAdjustment()
    {
        // assumes names of dice sides are just "4", "3", etc.
        if (Physics.Raycast(transform.position, raycastDownwards, out hit, 2f, 1 << 7))
        {
            totalTilesToDestroy = int.Parse(hit.transform.gameObject.name);
            totalTilesToDestroy--;  
            return totalTilesToDestroy;
        }
        return 0;
    }

    // movement related.
    IEnumerator RotatePlayer(float elapsedTime)
    {
        transform.rotation = Quaternion.Lerp(originalRotation, targetRotation, (elapsedTime / timeToRotate));
        yield return null;
    }

    // not movement related.
    // void FlipRigidbody()
    // {
    //     playerRigid.useGravity = !playerRigid.useGravity;
    //     playerRigid.isKinematic = !playerRigid.isKinematic;
    // }    
}
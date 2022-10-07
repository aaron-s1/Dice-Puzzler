using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodTeleportsPlayer : MonoBehaviour
{
    Player player;

    public int remainingTeleportMoves;
    // [SerializeField] int remainingTeleportMoves;
    [SerializeField] ParticleSystem teleportParticle;
    // [SerializeField] AudioClip teleportSoundEffect;
    [Space(10)]
    [SerializeField] float teleportTime;
    [SerializeField] float postTeleportMovementLockoutTime;

    Coroutine teleportPlayer;

    Vector3 newTilePos;

    bool isTeleporting;

    float ground_y_offset = 0.5f;

    int phasedCount;


    void Awake() {
        player = Player.Instance;
        teleportParticle = Instantiate(teleportParticle, transform.position, transform.rotation);
        followPlayer = true;
    }

    bool followPlayer;

    void FixedUpdate() {
        if (followPlayer)
            teleportParticle.transform.position = Player.Instance.transform.position;
    }


    void Update() {
        if (remainingTeleportMoves > 0) {
            if (Input.GetKeyDown(KeyCode.Space)) {   // change input key later?
                if (Player.Instance.movePlayer == null) {
                    if (!isTeleporting) {
                        FindRandomDestroyableTile();
                    }
                }
            }
        }
    }


    void FindRandomDestroyableTile()
    {
        // Note: "Cube(#)" GameObjects aren't sequentially ordered, so they won't match up with the found newTile #
        var totalTiles = GameManager.Instance.listOfDestroyableTiles.Count;
        int randomNumber = Random.Range(0, totalTiles);
        newTilePos = GameManager.Instance.listOfDestroyableTiles[randomNumber].transform.position;
        newTilePos = new Vector3 (newTilePos.x, newTilePos.y + ground_y_offset, newTilePos.z);
        
        StartCoroutine(Teleport());
    }

    

    IEnumerator PhaseOutParticlesOverMovementLockout(float divisor)
    {
        phasedCount++;
        yield return new WaitForSeconds(postTeleportMovementLockoutTime / divisor);
        teleportParticle.transform.position += new Vector3(0,  -2.5f / divisor, 0);

        if (phasedCount >= divisor) {
            phasedCount = 0;
            yield return null;
        }
        else yield return StartCoroutine(PhaseOutParticlesOverMovementLockout(divisor));
    }


    IEnumerator Teleport()
    {
        isTeleporting = true;
        GetComponent<AudioSource>().Play();
        
        followPlayer = true;
        remainingTeleportMoves--;
        player.isMoving = true;
        teleportParticle.Play();

        yield return new WaitForSeconds(teleportTime);
        SetNewPlayerPos();
        
        followPlayer = false;
        teleportParticle.transform.position = player.transform.position;
        
        teleportParticle.Stop(true);

        yield return StartCoroutine(PhaseOutParticlesOverMovementLockout(20));

        player.isMoving = false;
        isTeleporting = false;

        yield return null;
    }


    void SetNewPlayerPos() {
        // if (Player.Instance.LandedOnNormalTile())
            // Player.Instance.DisableAndReplaceTile(player.hit.transform.gameObject, true);
        Player.Instance.transform.position = newTilePos;
    }
}

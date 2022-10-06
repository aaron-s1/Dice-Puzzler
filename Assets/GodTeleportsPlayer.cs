using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodTeleportsPlayer : MonoBehaviour
{
    Player player;

    [SerializeField] int remainingTeleportMoves;
    [SerializeField] ParticleSystem teleportParticle;
    [SerializeField] AudioClip teleportSoundEffect;
    [Space(10)]
    [SerializeField] float teleportTime;
    [SerializeField] float postTeleportMovementLockoutTime;

    Coroutine teleportPlayer;

    Vector3 newTilePos;

    float y_offset = 0.5f;



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
                if (Player.Instance.movePlayer == null)
                    FindRandomDestroyableTile();
            }
        }
    }


    void FindRandomDestroyableTile()
    {
        // Note: "Cube(#)" gameObjects aren't sequentially ordered, and will not match up with the # that newTile finds
        var totalTiles = GameManager.Instance.listOfDestroyableTiles.Count;
        int randomNumber = Random.Range(0, totalTiles);
        newTilePos = GameManager.Instance.listOfDestroyableTiles[randomNumber].transform.position;
        newTilePos = new Vector3 (newTilePos.x, newTilePos.y + y_offset, newTilePos.z);

        StartCoroutine(Teleport());
    }

    Coroutine testParticleMove;

    // IEnumerator TestParticleMove()
    // {
    //     yield return new WaitForSeconds(postTeleportMovementLockoutTime/5);
    //     teleportParticle.transform.position += new Vector3(0, -0.5f, 0);
    //     // teleportParticle.transform.GetChild(0).gameObject.transform.position += new Vector3(0, 0, -0.5f);

    //     yield return TestParticleMove();
    // }

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

    int phasedCount;

    IEnumerator Teleport() 
    {
        // add sound effects later.

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

        yield return null;
    }


    void SetNewPlayerPos() {
        if (Player.Instance.LandedOnNormalTile())
            Player.Instance.DisableAndReplaceTile(player.hit.transform.gameObject, true);
        Player.Instance.transform.position = newTilePos;        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GodTeleportsPlayer : MonoBehaviour
{
    // Refactor + put this elsewhere later.

    Player player;

    public int teleportsRemaining;
    [SerializeField] ParticleSystem teleportParticle;
    [SerializeField] TextMeshProUGUI teleportsRemainingNumberUI;
    [Space(10)]
    [SerializeField] float teleportTime;
    [SerializeField] float postTeleportMovementLockoutTime;

    Coroutine teleportPlayer;

    Vector3 newTilePos;

    bool isTeleporting;
    bool teleportParticleFollowsPlayer;

    static float ground_y_offset = 0.5f;

    int phasingOutParticlesCount;


    void Awake() {
        teleportsRemainingNumberUI.text = teleportsRemaining.ToString();
        player = Player.Instance;
        teleportParticle = Instantiate(teleportParticle, transform.position, transform.rotation);
        teleportParticleFollowsPlayer = true;
    }


    void FixedUpdate() {
        if (teleportParticleFollowsPlayer)
            teleportParticle.transform.position = Player.Instance.transform.position;
    }


    void Update() {
        if (teleportsRemaining > 0) {
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

    

    // make less.... stupidly coded... later.
    
    IEnumerator PhaseOutParticlesOverMovementLockout(float divisor)
    {
        phasingOutParticlesCount++;
        yield return new WaitForSeconds(postTeleportMovementLockoutTime / divisor);
        teleportParticle.transform.position += new Vector3(0,  -2.5f / divisor, 0);

        if (phasingOutParticlesCount >= divisor) {
            phasingOutParticlesCount = 0;
            yield return null;
        }
        else yield return StartCoroutine(PhaseOutParticlesOverMovementLockout(divisor));
    }


    IEnumerator Teleport()
    {
        isTeleporting = true;

        var playerAudio = player.GetComponent<AudioSource>();
        playerAudio.PlayOneShot(playerAudio.clip);

        
        teleportParticleFollowsPlayer = true;

        teleportsRemaining--;
        teleportsRemainingNumberUI.text = teleportsRemaining.ToString();

        player.isMoving = true;
        teleportParticle.Play();

        yield return new WaitForSeconds(teleportTime);
        SetNewPlayerPos();
        
        teleportParticleFollowsPlayer = false;
        teleportParticle.transform.position = player.transform.position;
        
        teleportParticle.Stop(true);

        yield return StartCoroutine(PhaseOutParticlesOverMovementLockout(20));

        player.isMoving = false;
        isTeleporting = false;

        yield return null;
    }


    void SetNewPlayerPos() =>
        Player.Instance.transform.position = newTilePos;
}

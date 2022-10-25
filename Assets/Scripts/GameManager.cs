using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] TextMeshProUGUI destroyableTilesText;

    [SerializeField] AudioSource ambienceAudioSource;

    public GameObject replacedTiles;
    [SerializeField] GameObject destroyableTiles;
    [HideInInspector] public List<GameObject> listOfDestroyableTiles;

    [HideInInspector] public GameObject storingFiredParticles;


    public int minimumTilesRemainingToWin;

    public bool canReloadScene;
    
    int score;
    float originalTimeScale;

    bool devModeEnabled;

    void Awake()
    {
        Instance = this;

        listOfDestroyableTiles = new List<GameObject>();
        storingFiredParticles = new GameObject("Fired Tile Particles");     // i don't wanna make a pool :(

        originalTimeScale = Time.timeScale;
        
        // ensures that initial string is parseable
        destroyableTilesText.text = "0";

        foreach (Transform child in destroyableTiles.transform)
            AddToDestroyableTileList(child.gameObject);
    }


    void Update() {
        CheckForDevMode();
        EscapeKeyPausesGame();
        I_KeyReloadsScene();
        M_KeyTogglesAmbience();
        // F_KeyEndsGame();
        // G_KeyForfeitsGame();
    }

    
    // ONLY used by self.
    private void AddToDestroyableTileList(GameObject tile)
    {
        listOfDestroyableTiles.Add(tile);
        AdjustTilesRemaining(1);
    }

    // ONLY used by other scripts.
    public void RemoveFromDestroyableTilesList(GameObject tile)
    {
        listOfDestroyableTiles.Remove(tile);
        AdjustTilesRemaining(-1);
    }

    public int remainingTiles;


    void AdjustTilesRemaining(int adjustment) {
        remainingTiles = (int.Parse(destroyableTilesText.text));
        remainingTiles += adjustment;
                        
        destroyableTilesText.text = remainingTiles.ToString();
        // CheckIfWonGame(remaining);
    }

    public void CheckIfWonGame(int remaining) {
        if (minimumTilesRemainingToWin >= remaining)
        {
            // game won. do stuff.
        }
    }

    // public int ReturnScore() {
    //     return score;
    // }


    void I_KeyReloadsScene()
    {
        if (canReloadScene)
            if (Input.GetKeyDown(KeyCode.I))
                SceneManager.LoadScene("Dice Thing");
    }

    void M_KeyTogglesAmbience()
    {
        if (Input.GetKeyDown(KeyCode.M)) {
            if (ambienceAudioSource.isPlaying)
                ambienceAudioSource.Pause();
            else ambienceAudioSource.UnPause();
        }
    }

    // void G_KeyForfeitsGame()
    // {
    //     if (Input.GetKeyDown(KeyCode.G))
    //     {
    //         // (create new UI menu)
    //         // UI menu asks if player wants to forfeit
    //     }
    // }

    void EscapeKeyPausesGame() 
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == originalTimeScale)
                Time.timeScale = 0;
            else if (Time.timeScale == 0)
                Time.timeScale = originalTimeScale;
        }
    }

    // Q -> W -> E -> R
    void CheckForDevMode() 
    {        
        if (Input.GetKey(KeyCode.Q))
            if (Input.GetKey(KeyCode.W))
                if (Input.GetKey(KeyCode.E))
                    if (Input.GetKey(KeyCode.R))
                        Debug.Log("devModeEnabled = " + devModeEnabled);
        
        GetComponent<GodTeleportsPlayer>().teleportsRemaining = 5000;
    }
}

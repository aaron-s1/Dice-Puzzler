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

    public GameObject replacedTiles;
    [SerializeField] GameObject destroyableTiles;
    [HideInInspector] public List<GameObject> listOfDestroyableTiles;


    [HideInInspector] public GameObject storingFiredParticles;

    
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
    }


    // ONLY used by self.
    private void AddToDestroyableTileList(GameObject tile)
    {
        listOfDestroyableTiles.Add(tile);
        ListTilesRemaining(1);
    }

    // ONLY used by other scripts.
    public void RemoveFromDestroyableTilesList(GameObject tile)
    {
        listOfDestroyableTiles.Remove(tile);
        ListTilesRemaining(-1);
    }


    void ListTilesRemaining(int adjustment) =>
        destroyableTilesText.text = (int.Parse(destroyableTilesText.text) + adjustment).ToString();
    



    void I_KeyReloadsScene()
    {
        if (Input.GetKeyDown(KeyCode.I))
            SceneManager.LoadScene("Dice Thing");
    }

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
                    if (Input.GetKey(KeyCode.R)) {
        
            Debug.Log(devModeEnabled = true);
            GetComponent<GodTeleportsPlayer>().remainingTeleportMoves = 5000;
        }
    }
}

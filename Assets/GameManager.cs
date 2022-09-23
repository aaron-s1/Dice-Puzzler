using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<GameObject> listOfDestroyableTiles;   // make private later?
    [SerializeField] GameObject destroyableTiles;
    [SerializeField] TMP_Text destroyableTilesText;

    // public GameObject currentTileTouched;

    float originalTimeScale;

    bool devModeEnabled;


    void Awake()
    {
        Instance = this;

        listOfDestroyableTiles = new List<GameObject>();        
        originalTimeScale = Time.timeScale;
        destroyableTilesText.text = "0";

        foreach (Transform child in destroyableTiles.transform)
            AddToTileList(child.gameObject);
    }


    void Update() {
        CheckForDevMode();
        EscapeKeyPausesGame();
        I_KeyReloadsScene();
    }


    

    // ONLY used by self.
    public void AddToTileList(GameObject tile)
    {
        listOfDestroyableTiles.Add(tile);
        ListTilesRemaining(1);
    }

    // ONLY used by other scripts.
    public void RemoveFromTileList(GameObject tile, bool specialTileSpawned = false)
    {
        tile.SetActive(false);
        FireTilePoofParticles(tile, specialTileSpawned);
        listOfDestroyableTiles.Remove(tile);
        ListTilesRemaining(-1);
    }


    void FireTilePoofParticles(GameObject tile, bool specialTileSpawned) 
    {
        var particlesObj = tile.transform.GetChild(0).gameObject;
        var particles = particlesObj.GetComponentInChildren<ParticleSystem>();        

        if (particles != null)
        {            
            if (specialTileSpawned) {
                var particlesMain = particles.main;
                particlesMain.startColor = new Color (0f, 255f, 255f);     // teal-ish color
            }

            particlesObj.transform.parent = null;
            particlesObj.SetActive(true);
            particles.Play();
        }
    }


    void ListTilesRemaining(int adjustment) =>
        destroyableTilesText.text = (int.Parse(destroyableTilesText.text) + adjustment).ToString();



    //////////////////////////////////////////////////////////////////////

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

    void CheckForDevMode() 
    {
        // Q -> W -> E -> R
        if (Input.GetKey(KeyCode.Q))
            if (Input.GetKey(KeyCode.W))
                if (Input.GetKey(KeyCode.E))
                    if (Input.GetKey(KeyCode.R))                        
                        Debug.Log(devModeEnabled = true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] GameObject destroyableTiles;
    [SerializeField] List<GameObject> listOfDestroyableTiles;

    float originalTimeScale;


    void Awake()
    {
        Instance = this;

        listOfDestroyableTiles = new List<GameObject>();
        originalTimeScale = Time.timeScale;

        foreach (Transform child in destroyableTiles.transform)
            AddToTileList(child.gameObject);
    }


    void Update() =>
        EscapeKeyPausesGame();
    

    // ONLY used by self.
    public void AddToTileList(GameObject tile) =>
        listOfDestroyableTiles.Add(tile);

    // ONLY used by other scripts.
    public void RemoveFromTileList(GameObject tile, bool specialTileSpawned = false)
    {
        tile.SetActive(false);
        FireTilePoofParticles(tile, specialTileSpawned);
        listOfDestroyableTiles.Remove(tile);
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
}

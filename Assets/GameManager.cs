using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // public int sideHitValue;
    // public Vector3 direction;
    // public bool canCastRay;

    [SerializeField] GameObject destroyableTiles;
    [SerializeField] List<GameObject> listOfDestroyableTiles;

    public static GameManager Instance { get; private set; }

    void Awake()
    {
        listOfDestroyableTiles = new List<GameObject>();
        Instance = this;

        foreach (Transform child in destroyableTiles.transform)
            AddToTileList(child.gameObject);
    }

    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1)
                Time.timeScale = 0;
            else if (Time.timeScale == 0)
                Time.timeScale = 1;
        }
    }
    
    public void AddToTileList(GameObject tile) => listOfDestroyableTiles.Add(tile);

    // used by other scripts
    public void RemoveFromTileList(GameObject tile) {
        var particleObj = tile.transform.GetChild(0).gameObject;

        if (particleObj != null) {
            particleObj.transform.parent = null;
            particleObj.SetActive(true);
            particleObj.GetComponent<ParticleSystem>().Play();
        }

        listOfDestroyableTiles.Remove(tile);
    }
}

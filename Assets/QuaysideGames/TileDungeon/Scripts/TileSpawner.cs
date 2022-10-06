using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{

    public TileMatchSimple getTiles;
    public TileTypeMatchSimple getPrefab;
    public int TestNorth = 1;
    public int TestEast = 1;
    public int TestSouth = 1;
    public int TestWest = 1;
    GameObject tilePrefab;
    float rotator;
    string tiletype;
    
    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x < getTiles.Tiles.Length; x++)
        {
            if(getTiles.Tiles[x].NorthTile == TestNorth && getTiles.Tiles[x].EastTile == TestEast && getTiles.Tiles[x].SouthTile == TestSouth && getTiles.Tiles[x].WestTile == TestWest)
            {
                rotator = getTiles.Tiles[x].Rotation;
                tiletype = getTiles.Tiles[x].TileType;

                for (int i = 0; i < getPrefab.Tiles.Length; i++)
                {
                    if(tiletype == getPrefab.Tiles[i].TileType)
                    {
                        tilePrefab = getPrefab.Tiles[i].Prefab;
                    }
                }
            }
                
        }
        Instantiate(tilePrefab);
        tilePrefab.transform.position = new Vector3(0,0,0);
        tilePrefab.transform.rotation = Quaternion.Euler(0, rotator, 0);

    }


}

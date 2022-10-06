using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TileSpawnerExtended : MonoBehaviour
{

    public TileMatchExtended getTiles;
    public TileTypeMatchSimple getPrefab;
    public int tileWidth = 8;
    public int TestNorthWest = 1;
    public int TestNorth = 1;
    public int TestNorthEast = 1;
    public int TestWest = 1;
    public int TestEast = 1;
    public int TestSouthWest = 1;
    public int TestSouth = 1;
    public int TestSouthEast = 1;
    GameObject tilePrefab;
    public GameObject textPrefab;
    float tileRotation;
    string tiletype;
    int[,] directionArray;

    // Start is called before the first frame update
    void Start()
    {
        //setup direction array
        directionArray = new int[8,8] 
        {
{0,0,0,1,0,1,1,0},
{0,0,0,1,0,1,1,1},
{0,0,1,1,0,1,1,0},
{0,0,1,1,0,1,1,1},
{1,0,0,1,0,1,1,0},
{1,0,0,1,0,1,1,1},
{1,0,1,1,0,1,1,0},
{1,0,1,1,0,1,1,1},

        };

        for (int i = 0; i <= directionArray.GetUpperBound(0); i++)
        {
            //setup directions from array
            TestNorthWest = directionArray[i, 0];
            TestNorth = directionArray[i, 1];
            TestNorthEast = directionArray[i, 2];
            TestWest = directionArray[i, 3];
            TestEast = directionArray[i, 4];
            TestSouthWest = directionArray[i, 5];
            TestSouth = directionArray[i, 6];
            TestSouthEast = directionArray[i, 7];

            //save matching tile
            for (int z = 0; z < getTiles.Tiles.Length; z++)
            {

                if (getTiles.Tiles[z].NorthWestTile == TestNorthWest &&
                    getTiles.Tiles[z].NorthTile == TestNorth &&
                    getTiles.Tiles[z].NorthEastTile == TestNorthEast &&
                    getTiles.Tiles[z].WestTile == TestWest &&
                    getTiles.Tiles[z].EastTile == TestEast &&
                    getTiles.Tiles[z].SouthWestTile == TestSouthWest &&
                    getTiles.Tiles[z].SouthTile == TestSouth &&
                    getTiles.Tiles[z].SouthEastTile == TestSouthEast
                    )
                {
                    tiletype = getTiles.Tiles[z].TileType;
                    tileRotation = getTiles.Tiles[z].Rotation;

                    //get prefab for tile type
                    for (int x = 0; x < getPrefab.Tiles.Length; x++)
                    {
                        if (tiletype == getPrefab.Tiles[x].TileType)
                        {
                            tilePrefab = getPrefab.Tiles[x].Prefab;
                        }
                    }
                }
            }

            Instantiate(tilePrefab, new Vector3(i * tileWidth + (i*0.2f), 0, 0), Quaternion.Euler(0, tileRotation, 0));

            //spawn the tile type and rotation into the scene
            GameObject textD = Instantiate(textPrefab, new Vector3(i * tileWidth + (i * 0.2f), 1, 0), textPrefab.transform.rotation);
            textD.GetComponent<TextMeshPro>().text = tiletype.ToString() + " - " + tileRotation.ToString();

            GameObject textD2 = Instantiate(textPrefab, new Vector3(i * tileWidth + (i * 0.2f), 1, 1), textPrefab.transform.rotation);
            textD2.GetComponent<TextMeshPro>().text = directionArray[i, 0].ToString() + ","
                                                    + directionArray[i, 1].ToString() + ","
                                                    + directionArray[i, 2].ToString() + ","
                                                    + directionArray[i, 3].ToString() + ","
                                                    + directionArray[i, 4].ToString() + ","
                                                    + directionArray[i, 5].ToString() + ","
                                                    + directionArray[i, 6].ToString() + ","
                                                    + directionArray[i, 7].ToString();

            GameObject textD3 = Instantiate(textPrefab, new Vector3(i * tileWidth + (i * 0.2f), 1, 2), textPrefab.transform.rotation);
            textD3.GetComponent<TextMeshPro>().text = "NW-"
                                                    + "N-"
                                                    + "NE-"
                                                    + "W-"
                                                    + "E-"
                                                    + "SW-"
                                                    + "S-"
                                                    + "SE";
        }
    }


}

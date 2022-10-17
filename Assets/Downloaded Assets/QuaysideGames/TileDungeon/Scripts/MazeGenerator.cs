using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MazeGenerator : MonoBehaviour
{
    NodeGrid nodegrid;
    public int width, height;
    public int tileWidth;
    public int noRooms;
    public bool showRoof;
    private List<Vector2> endCaps = new List<Vector2>();
    public int noLoops;
    private List<Vector2> loopPaths = new List<Vector2>();
    public GameObject pathPrefab;
    public GameObject wallPrefab;
    public GameObject textPrefab;
    public GameObject roofPrefab;
    private int[,] Maze;
    private Node[,] NodeInfoMap;
    private Stack<Vector2> _tiletoTry = new Stack<Vector2>();
    private List<Vector2> offsets = new List<Vector2> { new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0) };
    private System.Random rnd = new System.Random();
    private Vector2 _currentTile;
    private int maxDistance;

    public int maxDist()
    {

        foreach (Node n in NodeInfoMap)
        {
            if (n.distance > maxDistance && n.walkable == true)
            {
                maxDistance = n.distance;
            }
        }
        return maxDistance;
    }

    public Vector2 CurrentTile
    {
        get { return _currentTile; }
        private set
        {
            if (value.x < 1 || value.x >= this.width - 1 || value.y < 1 || value.y >= this.height - 1)
            {
                throw new ArgumentException("CurrentTile must be within the one tile border all around the maze");
            }
            if (value.x % 2 == 1 || value.y % 2 == 1)
            { _currentTile = value; }
            else
            {
                throw new ArgumentException("The current square must not be both on an even X-axis and an even Y-axis, to ensure we can get walls around all tunnels");
            }
        }
    }
    // Start is called before the first frame update
    void Awake()
    {
        nodegrid = GetComponent<NodeGrid>();
    }

    //Call Nodegrid using the generated maze to create the node grid NodeInfoMap, this holds more information about each node
    public Node[,] GetDepthMap(int[,] Maze, Vector3 startPos)
    {
        NodeInfoMap = nodegrid.MazeDepth(Maze, startPos);
        return NodeInfoMap;
    }

    void Start()
    {
        GenerateMaze();
        GetDepthMap(Maze, new Vector3(1, 0, 1));
        maxDist();

        foreach (Node n in NodeInfoMap)
        {
            if (n.walkable == false)
            {
                if (showRoof == true)
                {
                    Instantiate(roofPrefab, new Vector3(n.gridX * tileWidth, 4.5f, n.gridY * tileWidth), Quaternion.Euler(0, 0, 0));
                }
            }

            if (n.walkable == true)
            {
                GameObject textD = Instantiate(textPrefab, new Vector3(n.gridX * tileWidth, 1, n.gridY * tileWidth), textPrefab.transform.rotation);
                textD.GetComponent<TextMeshPro>().text = n.distance.ToString();
                //set tile text color by type
                if (n.tilePrefab.name == "tile_end")
                {
                    textD.GetComponent<TextMeshPro>().faceColor = new Color32(0, 105, 255, 255);
                }
                if (n.tilePrefab.name == "tile_tjunc")
                {
                    textD.GetComponent<TextMeshPro>().faceColor = new Color32(254, 224, 0, 255);
                }
                if (n.tilePrefab.name == "tile_crossroads")
                {
                    textD.GetComponent<TextMeshPro>().faceColor = new Color32(0, 254, 111, 255);
                }
                //set last maxDistance tile text to red
                if (n.distance == maxDistance)
                {
                    textD.GetComponent<TextMeshPro>().faceColor = new Color32(250, 63, 63, 255);
                }


                //Blue - (0,105,255,255), yellow (new Color32( 254 , 224 , 0, 255 ), green - new Color32( 0 , 254 , 111, 255))
                Instantiate(n.tilePrefab, new Vector3(n.gridX * tileWidth, 0, n.gridY * tileWidth), Quaternion.Euler(0, n.tileRotation, 0));
                //write out spawned tile info
                Debug.Log(n.walkable + " " + n.position + " " + n.distance + " " + n.tileRotation + " " + n.tilePrefab.name);
            }

        }

    }
    void GenerateMaze()
    {
        Maze = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Maze[x, y] = 0;
            }
        }
        CurrentTile = Vector2.one;
        _tiletoTry.Push(CurrentTile);
        Maze = CreateMaze();
        SaveEndCaps();
        MakeRooms();
        SaveLoopCandidates();
        MakeLoops();

    }

    public int[,] CreateMaze()
    {
        //local variable to store neighbors to the current square
        //as we work our way through the maze
        List<Vector2> neighbors;
        //as long as there are still tiles to try
        while (_tiletoTry.Count > 0)
        {
            //excavate the square we are on
            Maze[(int)CurrentTile.x, (int)CurrentTile.y] = 1;

            //get all valid neighbors for the new tile
            neighbors = GetValidNeighbors(CurrentTile);

            //if there are any interesting looking neighbors
            if (neighbors.Count > 0)
            {
                //remember this tile, by putting it on the stack
                _tiletoTry.Push(CurrentTile);
                //move on to a random of the neighboring tiles
                CurrentTile = neighbors[rnd.Next(neighbors.Count)];
            }
            else
            {
                //if there were no neighbors to try, we are at a dead-end
                //toss this tile out
                //(thereby returning to a previous tile in the list to check).
                CurrentTile = _tiletoTry.Pop();
            }
        }

        return Maze;
    }
    /// <summary>
    /// Get all the prospective neighboring tiles
    /// </summary>
    /// <param name="centerTile">The tile to test</param>
    /// <returns>All and any valid neighbors</returns>
    private List<Vector2> GetValidNeighbors(Vector2 centerTile)
    {

        List<Vector2> validNeighbors = new List<Vector2>();

        //Check all four directions around the tile
        foreach (var offset in offsets)
        {
            //find the neighbor's position
            Vector2 toCheck = new Vector2(centerTile.x + offset.x, centerTile.y + offset.y);

            //make sure the tile is not on both an even X-axis and an even Y-axis
            //to ensure we can get walls around all tunnels
            if (toCheck.x % 2 == 1 || toCheck.y % 2 == 1)
            {
                //if the potential neighbor is unexcavated (==0)
                //and still has three walls intact (new territory)
                if (Maze[(int)toCheck.x, (int)toCheck.y] == 0 && HasThreeWallsIntact(toCheck))
                {
                    //add the neighbor
                    validNeighbors.Add(toCheck);
                }
            }
        }

        return validNeighbors;
    }


    /// <summary>
    /// Counts the number of intact walls around a tile
    /// </summary>
    /// <param name="Vector2ToCheck">The coordinates of the tile to check</param>
    /// <returns>Whether there are three intact walls (the tile has not been dug into earlier.</returns>
    private bool HasThreeWallsIntact(Vector2 Vector2ToCheck)
    {
        int intactWallCounter = 0;

        //Check all four directions around the tile
        foreach (var offset in offsets)
        {
            //find the neighbor's position
            Vector2 neighborToCheck = new Vector2(Vector2ToCheck.x + offset.x, Vector2ToCheck.y + offset.y);

            //make sure it is inside the maze, and it hasn't been dug out yet
            if (IsInside(neighborToCheck) && Maze[(int)neighborToCheck.x, (int)neighborToCheck.y] == 0)
            {
                intactWallCounter++;
            }
        }

        //tell whether three walls are intact
        return intactWallCounter == 3;

    }

    private bool IsInside(Vector2 p)
    {
        return p.x >= 0 && p.y >= 0 && p.x < width && p.y < height;
    }

    void SaveEndCaps()
    {
        //walk through the generated maze and find wall end caps and save.
        for (int i = 0; i <= Maze.GetUpperBound(0); i++)
        {
            for (int j = 0; j <= Maze.GetUpperBound(1); j++)
            {
                if (Maze[i, j] == 0)
                {
                    Vector2 Endcap = new Vector2(i, j);
                    if (HasThreePaths(Endcap))
                    {
                        endCaps.Add(new Vector2(i, j));
                    }
                }
            }
        }
        Debug.Log("No of endcaps " + endCaps.Count);
    }

    void MakeRooms()
    {
        //pick a random endcap from the list and make into path tile, repeat for the number of rooms required
        //ensure you cannot cant create more rooms than identified end caps, remove a cap from the list once removed.
        if (noRooms > endCaps.Count)
        {
            noRooms = endCaps.Count;
        }

        for (int x = 1; x <= noRooms; x++)
        {
            Vector2 capToPath = endCaps[rnd.Next(endCaps.Count)];
            Maze[(int)capToPath.x, (int)capToPath.y] = 1;
            Debug.Log(capToPath + "make room");
            endCaps.Remove(capToPath);
        }
    }

    private bool HasThreePaths(Vector2 Vector2ToCheck)
    {
        int pathCounter = 0;

        //Check all four directions around the tile
        foreach (var offset in offsets)
        {
            //find the neighbor's position
            Vector2 neighborToCheck = new Vector2(Vector2ToCheck.x + offset.x, Vector2ToCheck.y + offset.y);

            //make sure it is inside the maze, and it is surrounded by path
            if (IsInside(neighborToCheck) && Maze[(int)neighborToCheck.x, (int)neighborToCheck.y] == 1)
            {
                pathCounter++;
            }
        }

        //has it got paths on three sides?
        return pathCounter == 3;

    }

    void SaveLoopCandidates()
    {
        //walk through the generated maze ignoring the outer cells and find candidates for making loop paths.
        for (int i = 1; i <= Maze.GetUpperBound(0) - 1; i++)
        {
            for (int j = 1; j <= Maze.GetUpperBound(1) - 1; j++)
            {
                if (Maze[i, j] == 0)
                {
                    //A loop path must have a path tile to the N & S or E & W
                    Vector2 NorthTile = new Vector2(i + 0, j + 1);
                    Vector2 EastTile = new Vector2(i + 1, j + 0);
                    Vector2 SouthTile = new Vector2(i + 0, j - 1);
                    Vector2 WestTile = new Vector2(i - 1, j + 0);
                    Vector2 LoopPath = new Vector2(i, j);

                    if (Maze[(int)NorthTile.x, (int)NorthTile.y] == 1
                        && Maze[(int)SouthTile.x, (int)SouthTile.y] == 1
                        && Maze[(int)EastTile.x, (int)EastTile.y] == 0
                        && Maze[(int)WestTile.x, (int)WestTile.y] == 0)
                    {
                        loopPaths.Add(new Vector2(i, j));
                    }
                    else if (Maze[(int)NorthTile.x, (int)NorthTile.y] == 0
                        && Maze[(int)SouthTile.x, (int)SouthTile.y] == 0
                        && Maze[(int)EastTile.x, (int)EastTile.y] == 1
                        && Maze[(int)WestTile.x, (int)WestTile.y] == 1)
                    {
                        loopPaths.Add(new Vector2(i, j));
                    }

                }
            }
        }
        Debug.Log("No of loop paths " + loopPaths.Count);
    }

    void MakeLoops()
    {
        //pick a random loop path from the list and make into path tile, repeat for the number of loops required
        //ensure you cannot cant create more loops than identified, remove a loop from the list once removed.
        if (noLoops > loopPaths.Count)
        {
            noLoops = loopPaths.Count;
        }

        for (int x = 1; x <= noLoops; x++)
        {
            Vector2 loopToPath = loopPaths[rnd.Next(loopPaths.Count)];
            Maze[(int)loopToPath.x, (int)loopToPath.y] = 1;
            Debug.Log(loopToPath + "make loop");
            loopPaths.Remove(loopToPath);
        }
    }
}

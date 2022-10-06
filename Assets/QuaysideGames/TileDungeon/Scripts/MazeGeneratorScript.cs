using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

//This vesion get called using the build editor button script
public class MazeGeneratorScript : MonoBehaviour
{
    NodeGridExtended nodegrid;
    public int xDimension, yDimension;
    public int tileWidth;
    public int noRooms;
    public int noLoops;
    [Range(2, 4)]
    public int noSections = 3;
    public bool spawnRoof;
    public bool spawnBreadcrumbs;
    private List<Vector2> endCaps = new List<Vector2>();
    private List<Vector2> loopPaths = new List<Vector2>();
    public GameObject roofPrefab;
    public float roofHeight;
    public GameObject textPrefab;
    public GameObject breadcrumb;
    public GameObject dividerDoor;
    public GameObject dividerArch;
    public GameObject dividerWindows;
    public GameObject doorKey;
    private int[,] Maze;
    private Node[,] NodeInfoMap;
    private Node startNode;
    private Node endNode;
    private Node section2Node;
    private Node section1Node;
    private Node section3Node;
    private List<Node> mazePath;
    private Stack<Vector2> _tiletoTry = new Stack<Vector2>();
    private List<Vector2> offsets = new List<Vector2> { new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0) };
    private System.Random rnd = new System.Random();
    private Vector2 _currentTile;
    private int maxDistance = 0;
    //Pass in maze start position (default is 1,0,1)
    private Vector3 startPos = new Vector3(1, 0, 1);

    public void BuildMaze()
    {
        GenerateMaze();
        SetupNodegrid();
        GetDepthMap(Maze, startPos);
        maxDist();
        selectStart();
        selectFinish();
        ParentPath(startNode, endNode);
        SetupObstacles();
        CreateSections();
        SaveNodeLoopCandidates();
        MakeNodeLoops();
        nodegrid.CallPickTile();
        MakeLoopsDividers();
        MakeKeyHolders();
        Spawnmaze();
    }
    void GenerateMaze()
    {
        //create a basic array of integers to use for the inital maze creation.
        Maze = new int[xDimension, yDimension];
        for (int x = 0; x < xDimension; x++)
        {
            for (int y = 0; y < yDimension; y++)
            {
                Maze[x, y] = 0;
            }
        }
        CurrentTile = Vector2.one;
        _tiletoTry.Push(CurrentTile);
        Maze = CreateMaze();
        SaveEndCaps();
        MakeRooms();
    }

    public int[,] CreateMaze()
    {
        //local variable to store Neighbours to the current square
        //as we work our way through the maze
        List<Vector2> Neighbours;
        //as long as there are still tiles to try
        while (_tiletoTry.Count > 0)
        {
            //excavate the square we are on
            Maze[(int)CurrentTile.x, (int)CurrentTile.y] = 1;

            //get all valid Neighbours for the new tile
            Neighbours = GetValidNeighbours(CurrentTile);

            //if there are any interesting looking Neighbours
            if (Neighbours.Count > 0)
            {
                //remember this tile, by putting it on the stack
                _tiletoTry.Push(CurrentTile);
                //move on to a random of the neighboring tiles
                CurrentTile = Neighbours[rnd.Next(Neighbours.Count)];
            }
            else
            {
                //if there were no Neighbours to try, we are at a dead-end
                //toss this tile out
                //(thereby returning to a previous tile in the list to check).
                CurrentTile = _tiletoTry.Pop();
            }
        }

        return Maze;
    }

    /// Get all the prospective neighboring tiles
    /// <param name="centerTile">The tile to test</param>
    /// <returns>All and any valid Neighbours</returns>
    private List<Vector2> GetValidNeighbours(Vector2 centerTile)
    {

        List<Vector2> validNeighbours = new List<Vector2>();

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
                    validNeighbours.Add(toCheck);
                }
            }
        }

        return validNeighbours;
    }

    //calculate the max distance from the start node.
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

    //parse infomap and store reference to start node
    void selectStart()
    {
        foreach (Node n in NodeInfoMap)
        {
            if (n.isStart == true)
            {
                startNode = n;
            }
        }

    }
    //find all nodes at max distance and select one to be the finsh
    void selectFinish()
    {


        List<Node> finishPaths = new List<Node>();
        List<Node> finishPathsEnd = new List<Node>();

        //Create a 2 list of nodes that are max distance from start, one that is endcaps
        foreach (Node n in NodeInfoMap)
        {
            if (n.distance == maxDistance && n.tileType == "tile_end")
            {
                finishPathsEnd.Add(n);
            }
        }

        foreach (Node m in NodeInfoMap)
        {
            if (m.distance == maxDistance && m.tileType != "tile_end")
            {
                finishPaths.Add(m);
            }
        }

        //if we have endcaps pick one and set to finish, else pick from the other list
        if (finishPathsEnd.Count > 0)
        {
            endNode = finishPathsEnd[UnityEngine.Random.Range(0, finishPathsEnd.Count)];
            endNode.isFinish = true;
            endNode.section = noSections;
        }
        else
        {
            endNode = finishPaths[UnityEngine.Random.Range(0, finishPaths.Count)];
            endNode.isFinish = true;
            endNode.section = noSections;
        }
    }

    public Vector2 CurrentTile
    {
        get { return _currentTile; }
        private set
        {
            if (value.x < 1 || value.x >= this.xDimension - 1 || value.y < 1 || value.y >= this.yDimension - 1)
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

    void SetupNodegrid()
    {
        nodegrid = GetComponent<NodeGridExtended>();
    }

    //Call Nodegrid using the generated maze to create the node grid NodeInfoMap, this holds more information about each node 
    //and will be used by subsequent steps to fill out the maze details and features
    public Node[,] GetDepthMap(int[,] Maze, Vector3 startPos)
    {
        NodeInfoMap = nodegrid.MazeDepth(Maze, startPos);
        return NodeInfoMap;
    }

    void SetupObstacles()
    {
        //Read the number of sections required - default is 3 max 4
        //Multi act structure, make X obstacles spread roughly at even sections along the critical path
        //to form X sections, set section no for further use.
        //check if no sections required is greater than 1 before beginning.
        if (noSections > 1)
        {
            int obstacleInterval = endNode.distance / noSections;
            int obstacle1 = obstacleInterval;
            int obstacle2 = obstacleInterval * 2;
            int obstacle3 = obstacleInterval * 3;

            bool obstacle1set = false;
            bool obstacle2set = false;
            bool obstacle3set = false;

            List<Node> straights = new List<Node>();

            foreach (Node m in mazePath)
            {
                if (m.tileType == "tile_straight")
                {
                    straights.Add(m);
                }
            }
            //set obstacle1
            while (obstacle1set == false)
            {
                foreach (Node s in straights)
                {
                    if (s.distance == obstacle1)
                    {
                        s.hasDivider = true;
                        s.hasObstacle = true;
                        s.section = 1;
                        obstacle1set = true;
                        section1Node = s;
                    }
                }
                obstacle1 = obstacle1 - 1;
            }

            //set obstacle2 if 3 or 4 sections
            if (noSections >= 3)
            {
                while (obstacle2set == false)
                {
                    foreach (Node s in straights)
                    {
                        if (s.distance == obstacle2)
                        {
                            s.hasDivider = true;
                            s.hasObstacle = true;
                            s.section = 2;
                            obstacle2set = true;
                            section2Node = s;
                        }
                    }
                    obstacle2 = obstacle2 - 1;
                }
            }

            //set obstacle3 if 4 sections
            if (noSections == 4)
            {
                while (obstacle3set == false)
                {
                    foreach (Node s in straights)
                    {
                        if (s.distance == obstacle3)
                        {
                            s.hasDivider = true;
                            s.hasObstacle = true;
                            s.section = 3;
                            obstacle3set = true;
                            section3Node = s;
                        }
                    }
                    obstacle3 = obstacle3 + 1;
                }
            }
        }
    }

    void CreateSections()
    {
        //Create the section identifiers for the maze starting at the end node.
        //and setting for all neighbours until you cannot find a valid neighbour that is not already set.

        switch (noSections)
        {
            case 4:
                NodeGridSetSection(endNode.position, noSections);
                NodeGridSetSection(section3Node.position, 3);
                NodeGridSetSection(section2Node.position, 2);
                NodeGridSetSection(section1Node.position, 1);
                break;
            case 3:
                NodeGridSetSection(endNode.position, noSections);
                NodeGridSetSection(section2Node.position, 2);
                NodeGridSetSection(section1Node.position, 1);
                break;
            case 2:
                NodeGridSetSection(endNode.position, noSections);
                NodeGridSetSection(section1Node.position, 1);
                break;
        }
    }

    //spawn the maze and add additional debug details - distance text and colour coding 
    void Spawnmaze()
    {
        //Create empty gameObjects for organising hirearchy - Maze Parts, Debug Text, Breadcrumbs
        GameObject MazeParts = new GameObject("MazeParts");
        MazeParts.transform.parent = this.gameObject.transform;
        MazeParts.transform.position = this.gameObject.transform.position;

        GameObject MazeTiles = new GameObject("MazeTiles");
        MazeTiles.transform.parent = MazeParts.transform;
        MazeTiles.transform.position = MazeParts.transform.position;

        GameObject MazeRoof = new GameObject("MazeRoof");
        MazeRoof.transform.parent = MazeParts.transform;
        MazeRoof.transform.position = MazeParts.transform.position;

        GameObject MazeExtras = new GameObject("MazeExtras");
        MazeExtras.transform.parent = MazeParts.transform;
        MazeExtras.transform.position = MazeParts.transform.position;

        GameObject DebugText = new GameObject("DebugText");
        DebugText.transform.parent = this.gameObject.transform;
        DebugText.transform.position = this.gameObject.transform.position;

        GameObject Breadcrumbs = new GameObject("Breadcrumbs");
        Breadcrumbs.transform.parent = this.gameObject.transform;
        Breadcrumbs.transform.position = this.gameObject.transform.position;

        foreach (Node n in NodeInfoMap)
        {
            //set spawnposition offsets using grid position * tilewidth + MazeGenerator origin (World space offset)

            Vector3 origin = this.transform.position;
            float xOffset = (n.gridX * tileWidth) + origin.x;
            float zOffset = (n.gridY * tileWidth) + origin.z;

            if (n.walkable == false)
            {
                if (spawnRoof == true)
                {
                    GameObject newRoof = PrefabUtility.InstantiatePrefab(roofPrefab) as GameObject;
                    newRoof.transform.position = new Vector3(xOffset, roofHeight, zOffset);
                    newRoof.transform.rotation = Quaternion.Euler(0, 0, 0);
                    newRoof.transform.parent = MazeRoof.transform;
                }
            }

            if (n.walkable == true)
            {
                //spawn the tile info text into the scene: section + distance + basetype (End, Tjunc, Xroads, Straight) 
                GameObject textD = Instantiate(textPrefab, (new Vector3(xOffset, 1, zOffset)), textPrefab.transform.rotation, DebugText.transform);
            
                //set tile text by generic type
                if (n.tileType == "tile_end")
                {
                    textD.GetComponent<TextMeshPro>().text = n.section.ToString() + " - " + n.distance.ToString() + " - E";
                    textD.GetComponent<TextMeshPro>().faceColor = new Color32(0, 105, 255, 255);
                }
                
                if (n.tileType == "tile_straight")
                {
                    textD.GetComponent<TextMeshPro>().text = n.section.ToString() + " - " + n.distance.ToString() + " - S";
                }

                if (n.tileType == "tile_tjunc_0"
                    || n.tileType == "tile_tjunc_1a"
                    || n.tileType == "tile_tjunc_1b"
                    || n.tileType == "tile_tjunc_2")
                {
                    textD.GetComponent<TextMeshPro>().text = n.section.ToString() + " - " + n.distance.ToString() + " - T";
                    textD.GetComponent<TextMeshPro>().faceColor = new Color32(254, 224, 0, 255);
                }
                
                if (n.tileType == "tile_crossroads_0"
                    || n.tileType == "tile_crossroads_1"
                    || n.tileType == "tile_crossroads_2a"
                    || n.tileType == "tile_crossroads_2b"
                    || n.tileType == "tile_crossroads_3"
                    || n.tileType == "tile_crossroads_4")
                {
                    textD.GetComponent<TextMeshPro>().text = n.section.ToString() + " - " + n.distance.ToString() + " - X";
                    textD.GetComponent<TextMeshPro>().faceColor = new Color32(0, 254, 111, 255);
                }
                
                if (n.tileType == "tile_corner_0"
                    || n.tileType == "tile_corner_1")
                {
                    textD.GetComponent<TextMeshPro>().text = n.section.ToString() + " - " + n.distance.ToString() + " - C";
                    textD.GetComponent<TextMeshPro>().faceColor = new Color32(255, 128, 0, 255);
                }
                
                //set finish tile text to red
                if (n.isFinish == true)
                {
                    textD.GetComponent<TextMeshPro>().faceColor = new Color32(250, 63, 63, 255);
                }

                //set start tile text to pink
                if (n.isStart == true)
                {
                    textD.GetComponent<TextMeshPro>().faceColor = new Color32(235, 52, 225, 255);
                }

                //spawn breadcrumbs into scene if required
                if (spawnBreadcrumbs == true)
                {
                    if (n.criticalPath == true)
                    {
                        Instantiate(breadcrumb, (new Vector3((xOffset) + 1.0f, 3.0f, (zOffset) + 1.0f)), Quaternion.Euler(0, 0, 0), Breadcrumbs.transform);
                    }
                }

                //spawn doors if it is a divider with door Add a wall and door prefab
                if (n.tileType == "tile_straight")
                {
                    if (n.hasDivider == true && n.hasObstacle == true)
                    {
                        GameObject newDividerDoor = PrefabUtility.InstantiatePrefab(dividerDoor) as GameObject;
                        newDividerDoor.transform.position = new Vector3(xOffset, 0, zOffset);
                        newDividerDoor.transform.rotation = Quaternion.Euler(0, n.tileRotation, 0);
                        newDividerDoor.transform.parent = MazeExtras.transform;
                    }
                }

                //spawn dividers into scene for loop cuts.
                if (n.isLoop == true && n.hasDivider == true)
                {
                    if (n.tileRotation == 0)
                    {
                        GameObject newDividerArch = PrefabUtility.InstantiatePrefab(dividerArch) as GameObject;
                        newDividerArch.transform.position = new Vector3(xOffset, 0, zOffset);
                        newDividerArch.transform.rotation = Quaternion.Euler(0, n.tileRotation, 0);
                        newDividerArch.transform.parent = MazeExtras.transform;

                    }
                    else
                    {
                        GameObject newDividerWindows = PrefabUtility.InstantiatePrefab(dividerWindows) as GameObject;
                        newDividerWindows.transform.position = new Vector3(xOffset, 0, zOffset);
                        newDividerWindows.transform.rotation = Quaternion.Euler(0, n.tileRotation, 0);
                        newDividerWindows.transform.parent = MazeExtras.transform;
                    }
                }

                //spawn keys into scene
                if (n.hasKeyItem == true)
                {
                    GameObject newKey = PrefabUtility.InstantiatePrefab(doorKey) as GameObject;
                    newKey.transform.position = new Vector3(xOffset, 1f, zOffset);
                    newKey.transform.rotation = Quaternion.Euler(90, 0, 0);
                    newKey.transform.parent = MazeExtras.transform;
                }

                //spawn tile prefabs
                //Blue - (0,105,255,255), yellow (new Color32( 254 , 224 , 0, 255 ), green new Color32( 0 , 254 , 111, 255))

                GameObject newObject = PrefabUtility.InstantiatePrefab(n.tilePrefab) as GameObject;
                newObject.transform.position = new Vector3(xOffset, 0, zOffset);
                newObject.transform.rotation = Quaternion.Euler(0, n.tileRotation, 0);
                newObject.transform.parent = MazeTiles.transform;

                //Instantiate(n.tilePrefab, (new Vector3(xOffset, 0, zOffset)), Quaternion.Euler(0, n.tileRotation, 0), this.transform);
                //write out spawned tile info
                Debug.Log(n.walkable + " " + n.position + " " + n.distance + " " + n.tileRotation + " " + n.tileType + " " + n.isStart + " " + n.isFinish + "" + n.section);

            }

        }
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
        return p.x >= 0 && p.y >= 0 && p.x < xDimension && p.y < yDimension;
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
        //pick a random endcap from the list and make into a path tile, repeat for the number of rooms required
        //ensure you cannot create more rooms than the number of identified end caps, remove a cap from the list once set to path.
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
    void SaveNodeLoopCandidates()
    {

        //walk through the node grid and find loop candidates 
        for (int i = 1; i <= NodeInfoMap.GetUpperBound(0) - 1; i++)
        {
            for (int j = 1; j <= NodeInfoMap.GetUpperBound(1) - 1; j++)
            {
                if (NodeInfoMap[i, j].walkable == false)
                {
                    //A loop path must have a path tile to either the N & S or E & W
                    //loop tile candidates cannot have a connection to another section of the maze and cannot connect to a tile marked as door OR 
                    Vector2 NorthTile = new Vector2(i + 0, j + 1);
                    Vector2 EastTile = new Vector2(i + 1, j + 0);
                    Vector2 SouthTile = new Vector2(i + 0, j - 1);
                    Vector2 WestTile = new Vector2(i - 1, j + 0);

                    if (NodeInfoMap[(int)NorthTile.x, (int)NorthTile.y].walkable == true && NodeInfoMap[(int)NorthTile.x, (int)NorthTile.y].hasObstacle == false && NodeInfoMap[(int)NorthTile.x, (int)NorthTile.y].isFinish == false
                        && NodeInfoMap[(int)SouthTile.x, (int)SouthTile.y].walkable == true && NodeInfoMap[(int)SouthTile.x, (int)SouthTile.y].hasObstacle == false && NodeInfoMap[(int)SouthTile.x, (int)SouthTile.y].isFinish == false
                        && NodeInfoMap[(int)NorthTile.x, (int)NorthTile.y].section == NodeInfoMap[(int)SouthTile.x, (int)SouthTile.y].section
                        && NodeInfoMap[(int)EastTile.x, (int)EastTile.y].walkable == false
                        && NodeInfoMap[(int)WestTile.x, (int)WestTile.y].walkable == false)
                    {
                        loopPaths.Add(new Vector2(i, j));
                    }
                    else if (NodeInfoMap[(int)NorthTile.x, (int)NorthTile.y].walkable == false
                        && NodeInfoMap[(int)SouthTile.x, (int)SouthTile.y].walkable == false
                        && NodeInfoMap[(int)EastTile.x, (int)EastTile.y].walkable == true && NodeInfoMap[(int)EastTile.x, (int)EastTile.y].hasObstacle == false
                        && NodeInfoMap[(int)WestTile.x, (int)WestTile.y].walkable == true && NodeInfoMap[(int)WestTile.x, (int)WestTile.y].hasObstacle == false
                        && NodeInfoMap[(int)EastTile.x, (int)EastTile.y].section == NodeInfoMap[(int)WestTile.x, (int)WestTile.y].section)
                    {
                        loopPaths.Add(new Vector2(i, j));
                    }

                }
            }
        }
        Debug.Log("No of loop paths " + loopPaths.Count);
    }
    void MakeNodeLoops()
    {
        //pick a random loop path from the list and make into path tile, repeat for the number of loops required
        //ensure you cannot cant create more loops than identified, remove a loop from the list once removed.
        if (noLoops > loopPaths.Count)
        {
            noLoops = loopPaths.Count;
        }

        for (int x = 1; x <= noLoops; x++)
        {
            Vector2 loopToPath = loopPaths[UnityEngine.Random.Range(0, loopPaths.Count - 1)];
            NodeInfoMap[(int)loopToPath.x, (int)loopToPath.y].walkable = true;
            NodeInfoMap[(int)loopToPath.x, (int)loopToPath.y].distance = 99;
            NodeInfoMap[(int)loopToPath.x, (int)loopToPath.y].isLoop = true;
            Debug.Log(loopToPath + "make loop");
            loopPaths.Remove(loopToPath);
        }
    }

    void MakeLoopsDividers()
    {
        foreach (Node n in NodeInfoMap)
        {
            if (n.isLoop == true && n.tileType == "tile_straight")
            {
                n.hasDivider = true;
            }
        }
    }
    public int NodeGridSetSection(Vector3 startPos, int section)
    {
        //set the starting node identified by start position.
        startNode = NodeInfoMap[(int)startPos.x, (int)startPos.z];

        Queue<Node> openSet = new Queue<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        //Set start node section, add the start node to the openset queue to begin our walk, also add to closedset so it doesn't get identified as a valid neighbour
        startNode.section = section;
        openSet.Enqueue(startNode);
        closedSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.Dequeue();

            foreach (Node neighbour in GetNodeNeighbours(currentNode))
            {
                //for each neighbour we haven't yet explored
                if (!closedSet.Contains(neighbour) && (neighbour.section == 0))
                {

                    closedSet.Add(neighbour);

                    //walk and set section until 
                    neighbour.section = section;

                    //Add this neighbour to the queue of nodes to examine for next loop
                    openSet.Enqueue(neighbour);
                }
            }
        }
        return section;

    }

    public List<Node> GetNodeNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        //Check all four neighbours around the node and return those that are walkable
        foreach (var offset in offsets)
        {
            //find the neighbor's position
            int checkX = node.gridX + (int)offset.x;
            int checkY = node.gridY + (int)offset.y;

            if (checkX >= 0 && checkX < NodeInfoMap.GetUpperBound(0) + 1 && checkY >= 0 && checkY < NodeInfoMap.GetUpperBound(1) + 1 && node.walkable == true)
            {
                neighbours.Add(NodeInfoMap[checkX, checkY]);
            }
        }

        return neighbours;
    }

    //Read Nodeinfomap and populate the parent path from end to start
    void ParentPath(Node startNode, Node endNode)
    {
        startNode.criticalPath = true;
        Node currentNode = endNode;
        //The critical path is the trail formed by the walk originally, we will block progress using this
        List<Node> criticalPath = new List<Node>();

        //walk back along path from end node to create critical path
        while (currentNode != startNode)
        {
            currentNode.criticalPath = true;
            criticalPath.Add(currentNode);
            currentNode = currentNode.parent;
        }
        criticalPath.Reverse();
        mazePath = criticalPath;
    }

    //The keyholders identifies candidate nodes for holding the key item required to pass the obstacle at the end of the section.
    //One of the candidates is picked and flagged ready to spawn the item into the dungeon
    void MakeKeyHolders()
    {
        int section = 1;
        int maxSection = noSections;
        List<Node> keysNonCritTemp = new List<Node>();
        List<Node> keysCritTemp = new List<Node>();
        Node keyToKeep;

        while (section < maxSection)
        {
            foreach (Node n in NodeInfoMap)
            {
                //prefer not to have key on critical path
                if (n.section == section && n.walkable == true && n.hasObstacle == false && n.criticalPath == false && n.isStart == false)
                {
                    keysNonCritTemp.Add(n);
                    Debug.Log("Key location added for section: " + section + " critical: " + n.criticalPath);
                }
                else if (n.section == section && n.walkable == true && n.hasObstacle == false && n.criticalPath == true)
                {
                    keysCritTemp.Add(n);
                    Debug.Log("Key location added for section: " + section + " critical: " + n.criticalPath);
                }
            }


            if (keysNonCritTemp.Count > 0)
            {
                keyToKeep = keysNonCritTemp[UnityEngine.Random.Range(0, keysNonCritTemp.Count - 1)];
                keyToKeep.hasKeyItem = true;
                Debug.Log("Keeping key Item " + keyToKeep.distance);
            }
            else if (keysCritTemp.Count > 0)
            {
                keyToKeep = keysCritTemp[UnityEngine.Random.Range(0, keysCritTemp.Count - 1)];
                keyToKeep.hasKeyItem = true;
                Debug.Log("Keeping key Item " + keyToKeep.distance);
            }
            else
            {
                Debug.LogError("No key item candidate");
            }

            keysCritTemp.Clear();
            keysNonCritTemp.Clear();
            section++;

        }
    }

}

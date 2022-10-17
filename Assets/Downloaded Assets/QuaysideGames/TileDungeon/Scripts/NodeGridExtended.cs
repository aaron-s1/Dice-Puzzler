using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGridExtended : MonoBehaviour
{
    private int[,] Nodemaze;
    Node[,] nodegrid;
    Node startNode;
    public TileMatchExtended getTiles;
    public TileTypeMatchSimple getPrefab;
    string tempTiletype;
    private List<Vector2> offsets = new List<Vector2> { new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0) };
    public Node[,] MazeDepth(int[,] maze, Vector3 startPos)
    {
        Nodemaze = maze;
        CreateNodeGrid();
        PickTile();
        NodeGridCalcDepth(startPos);
        return nodegrid;
    }

    public bool CallPickTile()
    {
        PickTile();
        return true;
    }

    public bool CallDepthMap(Vector3 startPos)
    {
        NodeGridCalcDepth(startPos);
        return true;
    }

    void CreateNodeGrid()
    {
        nodegrid = new Node[Nodemaze.GetUpperBound(0) + 1, Nodemaze.GetUpperBound(1) + 1];

        for (int x = 0; x < Nodemaze.GetUpperBound(0) + 1; x++)
        {
            for (int y = 0; y < Nodemaze.GetUpperBound(1) + 1; y++)
            {
                bool walkable = (Nodemaze[x, y] == 1) ? true : false;
                Vector3 position = new Vector3(x, 0, y);
                nodegrid[x, y] = new Node(walkable, position, x, y);
            }
        }

    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        //Check all four neighbours around the node and return those that are walkable
        foreach (var offset in offsets)
        {
            //find the neighbor's position
            int checkX = node.gridX + (int)offset.x;
            int checkY = node.gridY + (int)offset.y;

            if (checkX >= 0 && checkX < nodegrid.GetUpperBound(0) + 1 && checkY >= 0 && checkY < nodegrid.GetUpperBound(1) + 1 && node.walkable == true)
            {
                neighbours.Add(nodegrid[checkX, checkY]);
            }
        }

        return neighbours;
    }

    void NodeGridCalcDepth(Vector3 startPos)
    {
        //set the starting node identified by start position.
        startNode = nodegrid[(int)startPos.x, (int)startPos.z];

        Queue<Node> openSet = new Queue<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        //Set start node distance to 0, add the start node to the openset queue to begin our walk, also add to closedset so it doesn't get identified as a valid neighbour
        startNode.distance = 0;
        startNode.isStart = true;
        openSet.Enqueue(startNode);
        closedSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.Dequeue();

            foreach (Node neighbour in GetNeighbours(currentNode))
            {
                //for each neighbour we haven't yet explored
                if (!closedSet.Contains(neighbour))
                {

                    closedSet.Add(neighbour);
                    
                    //increment walk count from start
                    int newCostToNeighbour = currentNode.distance + 1; //GetDistance(currentNode, neighbour);
                    neighbour.distance = newCostToNeighbour;
                    neighbour.parent = currentNode;
                    neighbour.isStart = false;

                    //Add this neighbour to the queue of nodes to examine for next loop
                    openSet.Enqueue(neighbour);
                }
            }
        }

    }

    void PickTile()
    {
        //walk through the nodegrid and for each path tile check its neighbours to the N, S, E, W and then use these to 
        //lookup the correct tile from the scriptable object array and save the prefab model in the node.
        for (int i = 0; i <= nodegrid.GetUpperBound(0); i++)
        {
            for (int j = 0; j <= nodegrid.GetUpperBound(1); j++)
            {
                if (nodegrid[i, j].walkable == true)
                {
                    //create some local variables to hold our lookup key values based on neighbours
                    int NWtile;
                    int Ntile;
                    int NEtile;
                    int Etile;
                    int SWtile;
                    int Stile;
                    int SEtile;
                    int Wtile;

                    //identify the co-ordinates for neighbour tiles in each direction to check
                    Vector2 NorthWestTile = new Vector2(i - 1, j + 1);
                    Vector2 NorthTile = new Vector2(i + 0, j + 1);
                    Vector2 NorthEastTile = new Vector2(i + 1, j + 1);
                    Vector2 EastTile = new Vector2(i + 1, j + 0);
                    Vector2 SouthWestTile = new Vector2(i - 1, j - 1);
                    Vector2 SouthTile = new Vector2(i + 0, j - 1);
                    Vector2 SouthEastTile = new Vector2(i + 1, j - 1);
                    Vector2 WestTile = new Vector2(i - 1, j + 0);

                    //Check the tile to the north west is inside the maze, if it is set to returned value, otherwise set to 0
                    if (NorthWestTile.x >= 0 && NorthWestTile.x < nodegrid.GetUpperBound(0) + 1 && NorthWestTile.y >= 0 && NorthWestTile.y < nodegrid.GetUpperBound(1) + 1 && nodegrid[(int)NorthWestTile.x, (int)NorthWestTile.y].walkable == true)
                    {
                        NWtile = 1;
                    }
                    else
                    {
                        NWtile = 0;
                    }
                    
                    //Check the tile to the north is inside the maze, if it is set to returned value, otherwise set to 0
                    if (NorthTile.x >= 0 && NorthTile.x < nodegrid.GetUpperBound(0) + 1 && NorthTile.y >= 0 && NorthTile.y < nodegrid.GetUpperBound(1) + 1 && nodegrid[(int)NorthTile.x, (int)NorthTile.y].walkable == true)
                    {
                        Ntile = 1;
                    }
                    else
                    {
                        Ntile = 0;
                    }
                    
                    //Check the tile to the north east is inside the maze, if it is set to returned value, otherwise set to 0
                    if (NorthEastTile.x >= 0 && NorthEastTile.x < nodegrid.GetUpperBound(0) + 1 && NorthEastTile.y >= 0 && NorthEastTile.y < nodegrid.GetUpperBound(1) + 1 && nodegrid[(int)NorthEastTile.x, (int)NorthEastTile.y].walkable == true)
                    {
                        NEtile = 1;
                    }
                    else
                    {
                        NEtile = 0;
                    }

                    //Check the tile to the east is inside the maze, if it is set to returned value, otherwise set to 0
                    if (EastTile.x >= 0 && EastTile.x < nodegrid.GetUpperBound(0) + 1 && EastTile.y >= 0 && EastTile.y < nodegrid.GetUpperBound(1) + 1 && nodegrid[(int)EastTile.x, (int)EastTile.y].walkable == true)
                    {
                        Etile = 1;
                    }
                    else
                    {
                        Etile = 0;
                    }

                    //Check the tile to the south west is inside the maze, if it is set to returned value, otherwise set to 0
                    if (SouthWestTile.x >= 0 && SouthWestTile.x < nodegrid.GetUpperBound(0) + 1 && SouthWestTile.y >= 0 && SouthWestTile.y < nodegrid.GetUpperBound(1) + 1 && nodegrid[(int)SouthWestTile.x, (int)SouthWestTile.y].walkable == true)
                    {
                        SWtile = 1;
                    }
                    else
                    {
                        SWtile = 0;
                    }

                    //Check the tile to the south is inside the maze, if it is set to returned value, otherwise set to 0
                    if (SouthTile.x >= 0 && SouthTile.x < nodegrid.GetUpperBound(0) + 1 && SouthTile.y >= 0 && SouthTile.y < nodegrid.GetUpperBound(1) + 1 && nodegrid[(int)SouthTile.x, (int)SouthTile.y].walkable == true)
                    {
                        Stile = 1;
                    }
                    else
                    {
                        Stile = 0;
                    }

                    //Check the tile to the south east is inside the maze, if it is set to returned value, otherwise set to 0
                    if (SouthEastTile.x >= 0 && SouthEastTile.x < nodegrid.GetUpperBound(0) + 1 && SouthEastTile.y >= 0 && SouthEastTile.y < nodegrid.GetUpperBound(1) + 1 && nodegrid[(int)SouthEastTile.x, (int)SouthEastTile.y].walkable == true)
                    {
                        SEtile = 1;
                    }
                    else
                    {
                        SEtile = 0;
                    }

                    //Check the tile to the west is inside the maze, if it is set to returned value, otherwise set to 0
                    if (WestTile.x >= 0 && WestTile.x < nodegrid.GetUpperBound(0) + 1 && WestTile.y >= 0 && WestTile.y < nodegrid.GetUpperBound(1) + 1 && nodegrid[(int)WestTile.x, (int)WestTile.y].walkable == true)
                    {
                        Wtile = 1;
                    }
                    else
                    {
                        Wtile = 0;
                    }

                    //save matching tile
                    for (int z = 0; z < getTiles.Tiles.Length; z++)
                    {

                        if (getTiles.Tiles[z].NorthWestTile == NWtile &&
                            getTiles.Tiles[z].NorthTile == Ntile &&
                            getTiles.Tiles[z].NorthEastTile == NEtile &&
                            getTiles.Tiles[z].EastTile == Etile &&
                            getTiles.Tiles[z].SouthWestTile == SWtile &&
                            getTiles.Tiles[z].SouthTile == Stile &&
                            getTiles.Tiles[z].SouthEastTile == SEtile &&
                            getTiles.Tiles[z].WestTile == Wtile)
                        {
                            tempTiletype = getTiles.Tiles[z].TileType;
                            nodegrid[i, j].tileRotation = getTiles.Tiles[z].Rotation;

                            //get prefab for tile type
                            for (int x = 0; x < getPrefab.Tiles.Length; x++)
                            {
                                if (tempTiletype == getPrefab.Tiles[x].TileType)
                                {
                                    nodegrid[i, j].tilePrefab = getPrefab.Tiles[x].Prefab;
                                    nodegrid[i, j].tileType = tempTiletype;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    //for later use
    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

}

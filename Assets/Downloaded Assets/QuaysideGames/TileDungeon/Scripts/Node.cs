using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{

    public bool walkable;
    public Vector3 position;
    public int gridX;
    public int gridY;
    public int distance;
    public int section = 0;
    public bool criticalPath = false;
    public bool isStart = false;
    public bool isFinish = false;
    public bool hasDivider = false;
    public bool hasObstacle = false;
    public bool hasKeyItem = false;
    public bool isLoop = false;
    public float tileRotation;
    public string tileType;
    public GameObject tilePrefab;
    public Node parent;

    public Node(bool _walkable, Vector3 _position, int _gridX, int _gridY)
    {
        walkable = _walkable;
        position = _position;
        gridX = _gridX;
        gridY = _gridY;
    }

}

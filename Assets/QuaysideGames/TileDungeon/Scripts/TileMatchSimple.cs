using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TileData", menuName = "Tile Data Simple", order = 51)]
public class TileMatchSimple : ScriptableObject
{
    [SerializeField]
    public _tileMatch[] Tiles;
    [System.Serializable]
    public struct _tileMatch
    {
        // make a struct to hold the data information needed to spawn the correct tile
        [SerializeField]
        private int _northTile;
        [SerializeField]
        private int _eastTile;
        [SerializeField]
        private int _southTile;
        [SerializeField]
        private int _westTile;
        [SerializeField]
        private float _rotation;
        [SerializeField]
        private string _tileType;


        public int NorthTile { get { return _northTile; } }
        public int EastTile { get { return _eastTile; } }
        public int SouthTile { get { return _southTile; } }
        public int WestTile { get { return _westTile; } }
        public float Rotation { get { return _rotation; } }
        public string TileType { get { return _tileType; } }
    }
}


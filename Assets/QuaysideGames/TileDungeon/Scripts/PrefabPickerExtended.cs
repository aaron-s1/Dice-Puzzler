using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "New TileData", menuName = "Tile Data Extended", order = 52)]
public class PrefabPickerExtended : ScriptableObject
{
    [SerializeField]
    public _tile[] Tile;
    [System.Serializable]
    public struct _tile
    {
        // make a struct to hold the data information needed to spawn the correct tile
        [SerializeField]
        private int _northWestTile;
        [SerializeField]
        private int _northTile;
        [SerializeField]
        private int _northEastTile;
        [SerializeField]
        private int _westTile;
        [SerializeField]
        private int _eastTile;
        [SerializeField]
        private int _southWestTile;
        [SerializeField]
        private int _southTile;
        [SerializeField]
        private int _southEastTile;
        [SerializeField]
        private float _rotation;
        [SerializeField]
        private GameObject _prefab;


        public int NorthWestTile { get { return _northWestTile; } }
        public int NorthTile { get { return _northTile; } }
        public int NorthEastTile { get { return _northEastTile; } }
        public int WestTile { get { return _westTile; } }
        public int EastTile { get { return _eastTile; } }
        public int SouthWestTile { get { return _southWestTile; } }
        public int SouthTile { get { return _southTile; } }
        public int SouthEastTile { get { return _southEastTile; } }
        public float Rotation { get { return _rotation; } }
        public GameObject Prefab { get { return _prefab; } }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "New TileData", menuName = "Tile Data", order = 51)]
public class PrefabPicker : ScriptableObject
{
    [SerializeField]
    public _tile[] Tile;
    [System.Serializable]
    public struct _tile
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
        private GameObject _prefab;
        [SerializeField]
        private float _rotation;

        public int NorthTile { get { return _northTile; } }
        public int EastTile { get { return _eastTile; } }
        public int SouthTile { get { return _southTile; } }
        public int WestTile { get { return _westTile; } }
        public GameObject Prefab { get { return _prefab; } }
        public float Rotation { get { return _rotation; } }
    }
}

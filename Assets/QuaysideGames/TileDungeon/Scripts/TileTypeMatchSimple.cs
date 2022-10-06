using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TileData", menuName = "Tile Type Match Prefab", order = 53)]
public class TileTypeMatchSimple : ScriptableObject
{
    [SerializeField]
    public _tileTypes[] Tiles;
    [System.Serializable]
    public struct _tileTypes
    {
        // make a struct to hold the data information needed to spawn the correct tile
        [SerializeField]
        private string _tileType;
        [SerializeField]
        private GameObject _prefab;
        public string TileType { get { return _tileType; } }
        public GameObject Prefab { get { return _prefab; } }
    }
}

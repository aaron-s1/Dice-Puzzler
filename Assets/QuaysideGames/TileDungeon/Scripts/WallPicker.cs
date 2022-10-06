using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New WallData", menuName = "Wall Data Picker", order = 53)]
public class WallPicker : ScriptableObject
{
    [SerializeField]
    public _wallMatch[] Walls;
    [System.Serializable]
    public struct _wallMatch
    {
        // make a struct to hold the data information needed to spawn the correct tile
        [SerializeField]
        private int _probability;
        [SerializeField]
        private string _wallType;
        [SerializeField]
        private GameObject _prefab;

        public int Probability { get { return _probability; } }
        public string TileType { get { return _wallType; } }
        public GameObject Prefab { get { return _prefab; } }
    }
}


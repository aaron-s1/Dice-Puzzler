using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float x_offset;
    [SerializeField] float y_offset;
    [SerializeField] float z_offset;

    // void Awake() =>
    //     player = Player.Instance.transform;
    

    
    void FixedUpdate() {
        transform.position = new Vector3
            (player.position.x + x_offset,
            player.position.y + y_offset,
            player.position.z + z_offset);

        
    }
}

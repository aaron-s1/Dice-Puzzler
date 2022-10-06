using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyDoor : MonoBehaviour
{
    Animator anim;
    public GameObject door;
    public string animTrigger;
    private bool isOpen = false;
    [SerializeField] private Key.KeyType keyType;

    private void Awake()
    {
        anim = door.GetComponent<Animator>();
    }

    public Key.KeyType GetKeyType()
    {
        return keyType;
    }

    public void OpenDoor()
    {
        if (isOpen == false)
        {
            anim.SetTrigger(animTrigger);
            isOpen = true;
        }
    }

}

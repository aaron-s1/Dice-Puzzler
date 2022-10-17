using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{

    Animator anim;
    public GameObject door;
    public string animTrigger;
    private bool isOpen = false;


    // Start is called before the first frame update
    void Start()
    {
        anim = door.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isOpen == false)
        {
            anim.SetTrigger(animTrigger);
            isOpen = true;
        }
    }


    //private void OnTriggerExit(Collider other)
    //{
    //    anim.SetTrigger("CloseDoor");
    //}

}

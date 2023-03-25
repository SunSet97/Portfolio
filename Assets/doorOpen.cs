using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorOpen : MonoBehaviour
{
    private GameObject Robot;
    private Animator DoorMove;
    public bool DoorOpen;
    // Start is called before the first frame update
    void Start()
    {
        DoorMove=this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        DoorMove.SetBool("Open",DoorOpen);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Robot"))
        {
            DoorOpen=true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Robot"))
        {
            DoorOpen=false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour
{
    public Animator animator;

    public bool fire = false;
    public bool open = false;
    public bool touched = false;

    void Start()
    {  
        // Do nothing
    }

    public void ChangeState() {
        if (open) {
            // Close door
            animator.Play("Close");
            open = false;
        }
        else {
            // Open door
            animator.Play("Open");
            open = true;
        }
    }

    public void SetHot() {
        this.fire = true;
    }

    public string GetState() {
        touched = true;
        if (this.fire) {
            return "The door handle is hot.";
        } 
        return "The door handle is cold.";
    }
}

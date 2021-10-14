using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Look : MonoBehaviour
{

    public float sens = 120f;

    public Camera MainCam;
    public Camera BetaCam;
    public CharacterController controller;

    public GameObject manager;
    public GameObject gameManager;

    public bool holding = false;
    public bool shooting = false;
    public bool gameover = false;

    float xRot = 0.0f;
    float yRot = 0.0f;
    float maxRot = 40.0f;
    float range = 5.0f;

    Vector2 pos, delta;
    GameObject pickupActive, fireActive;
    Door doorActive;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {

        if (!gameover) {

            LookAround();
            Rays();

            try {
                if (holding && shooting && fireActive) fireActive.GetComponent<Fire>().TakeHit();
            } catch (Exception e) {
                // That's not a fire :P
            }

        }

        
    }

    // void LookAround() {
    //     float mouseX = Input.GetAxis("Mouse X") * sens * Time.deltaTime;
    //     float mouseY = Input.GetAxis("Mouse Y") * sens * Time.deltaTime;

    //     xRot -= mouseY;
    //     yRot += mouseX;

    //     xRot = Mathf.Clamp(xRot, -maxRot, +maxRot);
    //     transform.localRotation = Quaternion.Euler(xRot, yRot, 0.0f);
    // }


    void LookAround() {
        
        if (Input.touchCount > 0) {
            if (Input.GetTouch(0).phase == TouchPhase.Began) {
                pos = Input.GetTouch(0).position;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled) {
                pos = Vector2.zero;
                delta = Vector2.zero;
            }
        }

        DetectSwipes();

        // xRot -= mouseY;
        // yRot += mouseX;

        xRot = Mathf.Clamp(xRot, -maxRot, +maxRot);
        transform.localRotation = Quaternion.Euler(xRot, yRot, 0.0f);
    }


    void DetectSwipes() {

        delta = Vector2.zero;
        if (pos != Vector2.zero && Input.touchCount > 0) {
            delta = Input.GetTouch(0).position - pos; 
        }

        if (delta.magnitude > sens) {
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y)) {
                yRot += delta.x * 0.005f;
            } else {
                xRot -= delta.y * 0.005f;
            }
        }
    }

    


    void Rays() {
        RaycastHit hit;
        bool info = Physics.Raycast(transform.position, transform.forward, out hit, range);

        if (info) {

            if (hit.transform.name.Contains("Exit")) {
                Victory();
            }

            else if (hit.transform.name.Contains("Extinguisher")) {
                // Handle the Extinguisher
                doorActive = null;
                pickupActive = hit.transform.gameObject.transform.parent.gameObject;
                manager.GetComponent<UI>().ShowPickup();

            }

            else if (hit.transform.name.Contains("Fire")) {
                // Handle the Fire
                doorActive = null;
                fireActive = hit.transform.gameObject;
                if (!holding) manager.GetComponent<UI>().HidePickup();
            }

            else {
                fireActive = null;
                if (!holding) manager.GetComponent<UI>().HidePickup();

                // Handle the Door
                try {
                    doorActive = hit.transform.parent.GetComponent<Door>();
                    if (doorActive) {
                        manager.GetComponent<UI>().ChangeDoorMessage(doorActive.open);
                        manager.GetComponent<UI>().ShowDoor();
                    } else {
                        doorActive = null;
                        manager.GetComponent<UI>().HideDoor();
                    }
                } catch (Exception e) {
                    // That's not a door :O
                    doorActive = null;
                    manager.GetComponent<UI>().HideDoor();
                }
            }
        } else {
            doorActive = null;
            fireActive = null;
            manager.GetComponent<UI>().HideDoor();
            if (!holding) manager.GetComponent<UI>().HidePickup();
        }
    }

    void PickUp() {

        MainCam.cullingMask &= ~(1 << LayerMask.NameToLayer("Extinguisher"));
        BetaCam.cullingMask |= (1 << LayerMask.NameToLayer("Extinguisher"));

        StopExtinguisher();

        pickupActive.transform.SetParent(gameObject.transform);

        pickupActive.transform.localPosition = new Vector3(-2f, gameObject.transform.GetChild(0).transform.position.y - 2.3f, 2f);
        pickupActive.transform.localRotation = Quaternion.Euler(0.0f, -120.0f, 0.0f);

        holding = true;

        manager.GetComponent<UI>().ShowPickup();
        manager.GetComponent<UI>().ShowUse();
        manager.GetComponent<UI>().ChangeExtinguisherMessage(holding);
        
    }

    void Drop() {

        MainCam.cullingMask |= (1 << LayerMask.NameToLayer("Extinguisher"));
        BetaCam.cullingMask &= ~(1 << LayerMask.NameToLayer("Extinguisher"));

        StopExtinguisher();

        pickupActive.transform.SetParent(null);
        pickupActive.transform.position = new Vector3(gameObject.transform.position.x + 2.5f, 0.0f, gameObject.transform.position.z + 2.5f);
        
        pickupActive.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        pickupActive = null;

        holding = false;

        manager.GetComponent<UI>().HidePickup();
        manager.GetComponent<UI>().HideUse();
        manager.GetComponent<UI>().ChangeExtinguisherMessage(holding);

    }

    public void StartExtinguisher() {
        if (holding) pickupActive.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>().Play(true);
        shooting = true;
        manager.GetComponent<UI>().EnablePopup("Remember PASS: Pull, Aim, Squeeze, Sweep.");
    }

    public void StopExtinguisher() {
        if (holding) pickupActive.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
        shooting = false;
    }

    public void DoorHandle() {
        manager.GetComponent<UI>().EnablePopup(doorActive.GetState());
    }

    public void ToggleDoor() {
        if (!doorActive.touched) 
            manager.GetComponent<UI>().EnablePopup("Warning: Touch handle with back of palm.");
        manager.GetComponent<UI>().ChangeDoorMessage(doorActive.open);
        doorActive.ChangeState();
    }

    public void ToggleExtinguisher() {
        if (!holding) PickUp();
        else Drop();
    }


    void Victory() {

        int level = PlayerPrefs.GetInt("level", 1);
        PlayerPrefs.SetInt("level", level+1);
        PlayerPrefs.Save();

        gameover = true;
        gameManager.GetComponent<Manager>().End(false, "Successfully cleared Level " + level);
        
    }



}

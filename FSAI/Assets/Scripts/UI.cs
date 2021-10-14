using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{

    public GameObject crouchUI;
    public GameObject doorUI;
    public GameObject popupUI;
    public GameObject extinguisherUI;
    public GameObject timerUI;

    public GameObject manager;
    public bool gameover = false;

    public float levelTime = 300.0f;
    float startTime;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        float delta = Time.time - startTime;
        UpdateTimer(levelTime - delta);
    }

    public void ShowDoor() {
        doorUI.SetActive(true);
    }

    public void HideDoor() {
        doorUI.SetActive(false);
    }

    public void ChangeDoorMessage(bool open) {
        if (!open) {
            doorUI.transform.GetChild(0).GetComponent<Text>().text = "> Open Door";
        } else {
            doorUI.transform.GetChild(0).GetComponent<Text>().text = "> Close Door";
        }
    }

    public void ChangeCrouchMessage(bool crouching) {
        if (!crouching) {
            crouchUI.GetComponent<Text>().text = "> Crouch Low";
        } else {
            crouchUI.GetComponent<Text>().text = "> Stand Up";
        }
    }

    public void EnablePopup(string message) {
        popupUI.GetComponent<Text>().text = message;
        popupUI.SetActive(true);
    }

    public void DisablePopup() {
        popupUI.SetActive(false);
    }

    void UpdateTimer(float time) {

        if (time <= 0 && !gameover) {
            GameOver();
        }
        else {
            string minutes = ((int) (time / 60)).ToString();
            string seconds = ((int) (time % 60)).ToString();
            string millis =  ((int) ((time % 1) * 100)).ToString();

            if (minutes.Length < 2) minutes = "0" + minutes;
            if (seconds.Length < 2) seconds = "0" + seconds;
            if (millis.Length < 2) millis = "0" + millis;

            timerUI.GetComponent<Text>().text = minutes + " : " + seconds + " : " + millis;
        }
    }


    public void ShowPickup() {
        extinguisherUI.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void HidePickup() {
        extinguisherUI.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void ShowUse() {
        extinguisherUI.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void HideUse() {
        extinguisherUI.transform.GetChild(1).gameObject.SetActive(false);
    }

    public void ChangeExtinguisherMessage(bool holding) {
        if (!holding) {
            extinguisherUI.transform.GetChild(0).GetComponent<Text>().text = "> Grab Extinguisher";
        } else {
            extinguisherUI.transform.GetChild(0).GetComponent<Text>().text = "> Drop Extinguisher";
        }
    }

    void GameOver() {
        gameover = true;
        manager.GetComponent<Manager>().End(true, "Ran out of time");
    }


}

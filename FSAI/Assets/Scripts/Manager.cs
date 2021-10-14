using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{

    public GameObject overUI;
    public GameObject startUI;
    public GameObject gameUI;
    public GameObject instructionsUI;
    public GameObject player;

    int level;

    // Start is called before the first frame update
    void Start()
    {
        level = PlayerPrefs.GetInt("level", 1);
        startUI.transform.GetChild(3).GetComponent<Text>().text = "Play Level " + level;
    }


    public void Play() {

        Time.timeScale = 1.0f;

        overUI.SetActive(false);
        startUI.SetActive(false);
        gameUI.SetActive(true);

    }

    public void End(bool over, string sub) {

        string main = "You Won";
        if (over) main = "Game Over";

        overUI.transform.GetChild(1).GetComponent<Text>().text = main; 
        overUI.transform.GetChild(2).GetComponent<Text>().text = sub; 

        overUI.SetActive(true);
        startUI.SetActive(false);
        gameUI.SetActive(false);

        Time.timeScale = 0.0f;

    }

    public void Begin() {
        
        overUI.SetActive(false);
        startUI.SetActive(true);
        gameUI.SetActive(false);

        // player.SetActive(false);
        Time.timeScale = 0.0f;

    }

    public void Restart() {
        SceneManager.LoadScene("Office", LoadSceneMode.Single);
    }

    public void Instructions() {
        instructionsUI.SetActive(true);
        startUI.SetActive(false);
    }

    public void Close() {
        instructionsUI.SetActive(false);
        startUI.SetActive(true);
    }


}

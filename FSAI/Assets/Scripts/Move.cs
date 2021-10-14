using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Move : MonoBehaviour
{

    public GameObject generator;
    public Camera mainCam;
    public float speed = 10.0f;
    // public bool jump = false;
    public float oxygen = 100f;
    public CharacterController controller;
    public Slider oxygenBar;

    public bool gameover = false;
    public GameObject manager;
    public GameObject gameManager;

    // float g = 9.81f;
    // float y = 0.0f;
    // float h = 2.0f;

    int[,] field;
    int xPos;
    int zPos;

    bool crouching = false;


    // Start is called before the first frame update
    void Start() {
        field = generator.GetComponent<Level>().field;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameover) {

            Smoke();
            oxygenBar.value = oxygen;

        }

        // float x = Input.GetAxis("Horizontal");
        // float z = Input.GetAxis("Vertical");

        // if (controller.isGrounded) {
        //     if (Input.GetKeyDown(KeyCode.Space) && jump) {
        //         y = Mathf.Sqrt(2 * h * g);
        //     } else {
        //         y = 0;
        //     }
        // } 

        
        // y -= g * Time.deltaTime;
    
        // Vector3 step = (x * Vector3.Scale(transform.right, new Vector3(1, 0, 1)) + z * Vector3.Scale(transform.forward, new Vector3(1, 0, 1))) * speed + y * new Vector3(0, 1, 0);
        // controller.Move(step * Time.deltaTime);
        
    }

    public void Forward() {
        Vector3 step = (speed * Vector3.Scale(transform.forward, new Vector3(1, 0, 1)));
        controller.Move(step * Time.deltaTime);
    }

    public void Backward() {
        Vector3 step = (speed * Vector3.Scale(transform.forward, new Vector3(-1, 0, -1)));
        controller.Move(step * Time.deltaTime);
    }

    void OnControllerColliderHit(ControllerColliderHit hit) {
        if (hit.gameObject.layer == 8) GameOver("Burned by fire");
    }

    void Smoke() {

        xPos = (int) Mathf.Floor((this.gameObject.transform.position.x + 4.0f) / 4.0f);
        zPos = (int) Mathf.Floor((this.gameObject.transform.position.z + 4.0f) / 4.0f);

        if (field[xPos, zPos] == 3) {
            if (!crouching && oxygen >= 0.0f)
                oxygen -= 3.0f * Time.deltaTime;
        } else if (oxygen < 100f) {
            oxygen += 1.5f * Time.deltaTime;
        }

        if (oxygen <= 0.0f) {
            GameOver("Low oxygen levels");
            oxygen = 100.0f;
        }
    }

    public void Crouch() {

        if (crouching) {
            crouching = false;
            speed = 10.0f;
            mainCam.transform.Translate(0, 0.8f, 0);
            
            if (gameObject.transform.childCount > 1) {
                gameObject.transform.GetChild(1).Translate(0, 0.8f, 0);
            }

        } else {
            crouching = true;
            speed = 4.0f;
            mainCam.transform.Translate(0, -0.8f, 0);

            if (gameObject.transform.childCount > 1) {
                gameObject.transform.GetChild(1).Translate(0, -0.8f, 0);
            }
        }
        manager.GetComponent<UI>().ChangeCrouchMessage(crouching);
    }

    void GameOver(string message) {
        gameover = true;
        gameManager.GetComponent<Manager>().End(true, message);
    }

    
}

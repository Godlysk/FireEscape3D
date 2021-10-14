using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Extinguisher : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GameObject player;
    Look script;

    bool shooting = false;
    bool pressed = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pressed = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        script = player.GetComponent<Look>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pressed) {
            if (shooting == false) {
                script.StartExtinguisher();
                shooting = true;
            }
        } else {
            if (shooting == true) {
                script.StopExtinguisher();
                shooting = false;
            }
        }
        
    }
}

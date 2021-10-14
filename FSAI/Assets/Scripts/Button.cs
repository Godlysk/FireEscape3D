using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Button : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GameObject player;
    public bool forward = true;

    Move script;
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
        script = player.GetComponent<Move>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pressed) {
            if (forward) script.Forward();
            else script.Backward();
        }
        
    }
}

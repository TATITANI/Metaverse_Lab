using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class carController : MonoBehaviour
{
    float speed = 0;
    private Vector2 mouseDownPos;
    private Vector2 direction;
    private Vector3 startPos;
    private Vector2 movement = Vector2.zero;

    private void Start()
    {
        startPos = this.transform.position;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            this.speed = 0;
            mouseDownPos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            movement = mouseDownPos - (Vector2)Input.mousePosition;
            speed = 0.002f;
            GetComponent<AudioSource>().Play();
        }

        transform.Translate(movement * speed);
        this.speed *= 0.98f;
    }

    public void Reset()
    {
        transform.position = startPos;
        speed = 0;
    }
}
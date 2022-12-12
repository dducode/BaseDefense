using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    Vector3 startPos;
    Vector3 movement;

    void Start()
    {
        startPos = transform.position;
        movement = startPos;
    }

    void Update()
    {
        movement = Game.Player.transform.position + startPos;
        transform.position = movement;
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    movement playerMovement;
    float mouseSpeed = 3;
    private void Start()
    {
        playerMovement = GetComponent<movement>();
    }

    private void Update()
    {
        float x = Input.GetAxisRaw("Vertical");
        float z = Input.GetAxisRaw("Horizontal");
        playerMovement.Move(x, z);

        if(Input.GetKeyDown(KeyCode.Space))
        {
            playerMovement.Jump();
        }
    }
}

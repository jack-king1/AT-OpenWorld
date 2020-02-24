using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    movement playerMovement;
    float mouseSpeed = 3;
    PlayerData pd;
    TorchControl tc;

    private void Start()
    {
        playerMovement = GetComponent<movement>();
        pd = new PlayerData();
        tc = GetComponent<TorchControl>();
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

        if (Input.GetKeyDown(KeyCode.E))
        {
            tc.spotLight.enabled = !tc.spotLight.enabled;
            tc.pointLight.enabled = !tc.pointLight.enabled;
        }
    }
}

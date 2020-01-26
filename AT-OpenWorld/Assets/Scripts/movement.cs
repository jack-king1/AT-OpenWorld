using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{

    [SerializeField] private float speed = 10;
    [SerializeField] private float rotateSpeed = 50;
    [SerializeField] private Animator anim;
    [SerializeField] private float gravity = 7.0f;
    [SerializeField] private float jumpForce = 2.0f;

    CharacterController controller;
    bool isGrounded;
    Vector3 move = new Vector3(0, 0, 0);


    private void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent <CharacterController>();
    }

    private void Update()
    {
        if(!controller.isGrounded)
        {
            move.y -= gravity * Time.deltaTime;
        }
    }

    public void Move(float x, float z)
    {
        Rotate(z);
        move.x = 0.0f;

        move.z = 0.0f;
        if(x > 0)
        {
            move = transform.TransformDirection(Vector3.forward);
            anim.SetBool("Moving", true);
        }
        else if( x < 0)
        {
            move = transform.TransformDirection(-Vector3.forward);
            anim.SetBool("Moving", true);
        }
        else
        {
            anim.SetBool("Moving", false);
        }
        controller.Move(move * Time.deltaTime * speed);
    }

    public void Rotate(float y)
    {
        transform.Rotate(Vector3.up * y * Time.deltaTime * rotateSpeed);
    }

    public void Jump()
    {
        if(controller.isGrounded)
        {
            move.y = jumpForce;
        }
    }

    void Gravity()
    {

    }
}

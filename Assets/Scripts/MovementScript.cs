using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    public float speed;
    public float friction;

    float horizontal;
    float vertical;

    public Transform orientation;

    Vector3 movementDirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        Inputs();

        rb.drag = friction;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Inputs()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
    }

    public void Move()
    {
        movementDirection = orientation.forward * vertical + orientation.right * horizontal;


        rb.AddForce(movementDirection * speed * 10, ForceMode.Force);
    }

    private void NormalizeSpeed()
    {
        Vector3 curVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        Vector3 newVel = Vector3.Normalize(curVel);

    rb.velocity = newVel;
    }
}

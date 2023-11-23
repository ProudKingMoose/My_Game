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
    Vector3 currentPos, lastPos;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        transform.position = GameManager.instance.nextHeroPosition;
    }


    void Update()
    {
        Inputs();

        rb.drag = friction;
    }

    void FixedUpdate()
    {
        Move();

        currentPos = transform.position;

        if (currentPos == lastPos)
            GameManager.instance.isWalking = false;
        else
            GameManager.instance.isWalking = true;
        lastPos = currentPos;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "EncounterZone")
        {
            RegionData region = other.GetComponent<RegionData>();
            GameManager.instance.currentRegion = region;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "EncounterZone")
        {
            
            GameManager.instance.encounterPosible = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "EncounterZone")
        {
            GameManager.instance.encounterPosible = false;
        }
    }
}

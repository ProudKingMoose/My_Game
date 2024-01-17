using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    public float speed;
    public float vSpeed;
    public float gravity = 9.8f;

    float horizontal;
    float vertical;

    public Transform orientation;

    private bool sprinting;

    Vector3 movementDirection;
    Vector3 currentPos, lastPos;

    CharacterController CharacterController;

    private void Start()
    {
        CharacterController = GetComponent<CharacterController>();

        transform.position = GameManager.instance.lastHeroPosition;
        sprinting = false;
    }


    void Update()
    {
        Inputs();
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
        if (Input.GetKeyDown(KeyCode.LeftControl) && !sprinting)
        {
            speed *= 2f;
            sprinting = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl) && sprinting)
        {
            speed /= 2f;
            sprinting = false;
        }

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
    }

    public void Move()
    {
        movementDirection = orientation.forward * vertical + orientation.right * horizontal;

        CharacterController.Move(movementDirection.normalized * speed * Time.deltaTime);

        if (CharacterController.isGrounded)
        {
            vSpeed = 0;
        }
        else
        {
            vSpeed -= gravity * Time.deltaTime;
            movementDirection.y = vSpeed;
            CharacterController.Move(movementDirection * Time.deltaTime);
        }

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

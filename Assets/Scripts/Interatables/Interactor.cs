using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

interface IInteractable
{
    public void Interact();
}

public class Interactor : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractRange;


    // Update is called once per frame
    void Update()
    {
        Vector3 point = new Vector3(InteractorSource.position.x, InteractorSource.position.y - 0.5f, InteractorSource.position.z);
        Debug.DrawRay(point, InteractorSource.forward, Color.green);
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray r = new Ray(point, InteractorSource.forward);
            if (Physics.Raycast(r, out RaycastHit hitinfo, InteractRange))
            {
                if (hitinfo.collider.gameObject.TryGetComponent(out IInteractable interactableObject))
                {
                    interactableObject.Interact();
                }
            }
        }
    }
}

using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    public static CameraSystem instance;
    [SerializeField] CinemachineVirtualCamera cinemachineVirtualCamera;

    private Vector3 startingPosition;
    private Quaternion startingRotation;
    private float targetFieldOfView = 30;
    private float StartingFieldOfView = 60;
    private float rotationlength = 180;
    private bool Zoom = false;
    private bool rotationF;
    private bool action = false;
    private char type;
    private Quaternion lookRotation;
    private Vector3 targetedDirection;
    GameObject currentUser;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        startingPosition = transform.position;
        startingRotation = transform.rotation;
    }
    public void FirstCameraFix(GameObject user, char Type)
    {
        currentUser = user;
        type = Type;
        action = true;
        Debug.Log("this code is runned");
        Debug.Log(user);
        transform.position = user.transform.position;
        rotationF = true;

        Zoom = true;
    }
    public void FollowAttackerStep(GameObject user)
    {
        rotationF = false;

        transform.SetParent(user.transform);
    }

    public void CameraOnTargetTacker(GameObject Attacktarget, GameObject Bufftarget)
    {
        action = false;
        if (Attacktarget != null)
        {
            transform.SetParent(null);
            transform.position = Attacktarget.transform.position;
        }
        else if (Bufftarget != null)
            transform.position = Bufftarget.transform.position;
    }

    public void ReturnCamera()
    {
        currentUser = null;
        transform.position = startingPosition;
        transform.rotation = startingRotation;
        Zoom = false;
        rotationF = default;
        type = default;

    }
    private void Update()
    {
        Zoomies();
        if (action)
            Rotations();
    }
    private void Zoomies()
    {
        if (Zoom && cinemachineVirtualCamera.m_Lens.FieldOfView != targetFieldOfView)
            cinemachineVirtualCamera.m_Lens.FieldOfView = Mathf.Lerp(cinemachineVirtualCamera.m_Lens.FieldOfView, targetFieldOfView, Time.deltaTime * 10);
        else if (!Zoom && cinemachineVirtualCamera.m_Lens.FieldOfView != StartingFieldOfView)
            cinemachineVirtualCamera.m_Lens.FieldOfView = Mathf.Lerp(cinemachineVirtualCamera.m_Lens.FieldOfView, StartingFieldOfView, Time.deltaTime * 10);
    }
    private void Rotations()//It s better now but polish it if you have time
    {
        Debug.Log(rotationF);
        if (rotationF)
        {
            if (type == 'E')
            {
                targetedDirection = -(new Vector3(currentUser.transform.position.x - 1, currentUser.transform.position.y, currentUser.transform.position.z) - transform.position).normalized;

                lookRotation = Quaternion.LookRotation(targetedDirection);

                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * 360);
            }
            else if (type == 'H' && transform.rotation.y != currentUser.transform.eulerAngles.y + rotationlength)
            {
                targetedDirection = (new Vector3(currentUser.transform.position.x + 1, currentUser.transform.position.y, currentUser.transform.position.z) - transform.position).normalized;

                lookRotation = Quaternion.LookRotation(targetedDirection);

                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * 360);
            }
        }
        else if (!rotationF)
        {
            targetedDirection = (new Vector3(currentUser.transform.position.x - 1, currentUser.transform.position.y, currentUser.transform.position.z) - transform.position).normalized;

            lookRotation = Quaternion.LookRotation(targetedDirection);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * 360);
        }
    }
}

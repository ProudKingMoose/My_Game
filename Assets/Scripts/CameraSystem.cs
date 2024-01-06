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
        action = true;
        type = Type;
        currentUser = user;
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
        action = false;
        currentUser = null;
        transform.position = startingPosition;
        transform.rotation = startingRotation;
        Zoom = false;
        rotationF = default;
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
    private void Rotations()
    {
        Debug.Log(rotationF);
        if (rotationF)
        {
            if (type == 'E' && transform.eulerAngles.y != currentUser.transform.eulerAngles.y - rotationlength)
            {
                Vector3 eulerRotation = new Vector3(transform.eulerAngles.x, Mathf.Lerp(transform.eulerAngles.y, currentUser.transform.eulerAngles.y + rotationlength, Time.deltaTime * 5), transform.eulerAngles.z);
                transform.rotation = Quaternion.Euler(eulerRotation);
            }
            else if (type == 'H' && transform.eulerAngles.y != currentUser.transform.eulerAngles.y + rotationlength)
            {
                Vector3 eulerRotation = new Vector3(transform.eulerAngles.x, Mathf.Lerp(transform.eulerAngles.y, currentUser.transform.eulerAngles.y - rotationlength, Time.deltaTime * 5), transform.eulerAngles.z);
                transform.rotation = Quaternion.Euler(eulerRotation);
            }
        }
        else if (!rotationF && transform.eulerAngles.y != currentUser.transform.eulerAngles.y)
        {
            Vector3 eulerRotation = new Vector3(transform.eulerAngles.x, Mathf.Lerp(transform.eulerAngles.y, currentUser.transform.eulerAngles.y, Time.deltaTime * 5), transform.eulerAngles.z);
            transform.rotation = Quaternion.Euler(eulerRotation);
        }
    }
}

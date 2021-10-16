using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCam : MonoBehaviour
{
    private Transform cam;

    private void Start()
    {
        cam = Camera.main.transform;
    }

    private void LateUpdate()
    {
        Vector3 dir = transform.position - cam.position;
        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = rot;
    }

}

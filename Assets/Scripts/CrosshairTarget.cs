using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairTarget : MonoBehaviour
{
    private Camera mainCamera;
    private Ray ray;
    private RaycastHit hitInfo;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        ray.origin = mainCamera.transform.position;
        ray.direction = mainCamera.transform.forward;

        Physics.Raycast(ray, out hitInfo);
        if (hitInfo.collider == null)
        {
            transform.position = mainCamera.transform.forward * 300f;
        }
        else if (!hitInfo.collider.CompareTag("ignoreShooting"))
        {
            transform.position = hitInfo.point;
        }
    }
}

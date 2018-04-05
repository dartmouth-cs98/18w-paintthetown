﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// based on the example code from http://wiki.unity3d.com/index.php?title=MouseOrbitImproved 

public class orbitCamera : MonoBehaviour {

    public Transform target;
    public float distance = 5.0f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    public float distanceMin = .5f;
    public float distanceMax = 15f;

    private Rigidbody rigidb;

    private Touch t;

    float x = 0.0f;
    float y = 0.0f;

    // Use this for initialization
    void Start()
    {
        GameObject buildingMesh = Instantiate((GameObject)Resources.Load("dartmouth_hall"));

        target = buildingMesh.GetComponent<Transform>();

        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        rigidb = GetComponent<Rigidbody>();

        // Make the rigid body not change rotation
        if (rigidb != null)
        {
            rigidb.freezeRotation = true;
        }
    }

    void LateUpdate()
    {
        if (target && Input.touchCount > 0)
        {
            //x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
            //y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
            t = Input.GetTouch(0);

            x += t.deltaPosition.x * xSpeed * distance * 0.002f;
            y += t.deltaPosition.y * ySpeed * 0.002f;

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);

            // distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);

            RaycastHit hit;
            if (Physics.Linecast(target.position, transform.position, out hit))
            {
                //distance -= hit.distance;
            }
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position;

            transform.rotation = rotation;
            transform.position = position;
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}

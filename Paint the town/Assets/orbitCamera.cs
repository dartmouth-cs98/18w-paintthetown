using System.Collections;
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

    public float zoomSpeed = .5f; // speed to zoom in or out at

    // Use this for initialization
    void Start()
    {
        string bID = PlayerPrefs.GetString("bid");
       
        // this will load in the desired building on scene strt, but it depends on the bID being one of the imported files
        GameObject buildingMesh = Instantiate((GameObject)Resources.Load(bID));

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
            if (Input.touchCount == 2)
            {
                // store them 
                Touch touch0 = Input.GetTouch(0);
                Touch touch1 = Input.GetTouch(1);

                // find the positions of those touches in the previous frame
                Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
                Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

                // find magnitude of the distance between the touches, both current and in the previous frame
                float touchDistanceMag = (touch0.position - touch1.position).magnitude;
                float prevTouchDistanceMag = (touch0PrevPos - touch1PrevPos).magnitude;

                // find the difference in magnitude between the two distances
                float distMagnitudeDiff = prevTouchDistanceMag - touchDistanceMag;

                distance += distMagnitudeDiff * zoomSpeed;
            }

            else
            {
                t = Input.GetTouch(0);

                x += t.deltaPosition.x * xSpeed * distance * 0.002f;
                y += t.deltaPosition.y * ySpeed * 0.002f;

                y = ClampAngle(y, yMinLimit, yMaxLimit);
            }

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

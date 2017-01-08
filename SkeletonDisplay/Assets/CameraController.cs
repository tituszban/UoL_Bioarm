using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    float angle = 0;
    float distance = 7.5f;

    public float rotateSpeed = 0.1f;
    public float distanceSpeed = 0.1f;

    public Transform cam;

	// Use this for initialization
	void Start () {
		
	}

    bool clickedInside = true;

	// Update is called once per frame
	void Update () {
        transform.rotation = Quaternion.Euler(0, angle, 0);
        cam.localPosition = new Vector3(0, 0, -distance);

        if (Input.GetMouseButtonDown(0))
        {

            clickedInside = Input.mousePosition.y / Screen.height > 0.1f;
        }

        if (Input.GetMouseButton(0) && clickedInside)
        {
            Cursor.lockState = CursorLockMode.Confined;
            angle = (angle + Input.GetAxis("Mouse X") * rotateSpeed) % 360;
            distance = Mathf.Clamp(distance - (Input.GetAxis("Mouse Y") + Input.GetAxis("Mouse ScrollWheel")) * distanceSpeed, 1, 7.5f);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }

        //Debug.Log(Input.mousePosition.y / Screen.height);


        //angle = (angle - Input.GetAxis("Horizontal") * rotateSpeed) % 360;
        //distance = Mathf.Clamp(distance - Input.GetAxis("Vertical") * distanceSpeed, 1, 7.5f);
	}
}

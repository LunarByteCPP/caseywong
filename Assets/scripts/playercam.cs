using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playercam : MonoBehaviour
{
    //https://www.youtube.com/watch?v=f473C43s8nE

    public float sensX;
    public float sensY;

    public Transform orientation;
    float xRotation;
    float yRotation;

    WorldGravity worldGravity;

    public Transform rigidBodyTransform;


    void Start()
    {
        //lock cursor and make it invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        worldGravity = rigidBodyTransform.GetComponent<WorldGravity>();
    }

    void Update()
    {
        //get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;

        xRotation -= mouseY;
        //make sure the camera doesn't go out of bounds
        // xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.rotation = rigidBodyTransform.rotation * Quaternion.Euler(xRotation, yRotation, 0);
        // orientation.rotation = Quaternion.AngleAxis(xRotation, worldGravity.GetWorldUp());
        // transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        // orientation = Quaternion.LookRotation(yRotation, orientation.up);
        // orientation.rotation *= Quaternion.Euler(0, yRotation, 0);
        orientation.rotation = transform.rotation;
    }
}

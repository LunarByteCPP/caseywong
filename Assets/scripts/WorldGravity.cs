/***************************************************************
file: WorldGravity.cs
author: Alex Mariano
class: CS 4700 – Game Development
assignment: Final Project
date last modified: 11/30/2024

purpose: This file provides functions and support for planet
gravity.
Will include tangents and normal directions.

****************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGravity : MonoBehaviour
{

    public GameObject center;

    public float gravityConstant;
    public GameObject debugCube;

    Rigidbody rb;

    public bool keepSelfNormal; 

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        ApplyGravity();
        
    }

    void FixedUpdate()
    {
        SelfNormalize();
    }
    void ApplyGravity()
    {
        rb.AddForceAtPosition(gravityConstant * rb.mass * -GetWorldUp(), rb.centerOfMass);
    }


    public Vector3 GetWorldUp()
    {
        Vector3 directionVector = transform.position - center.transform.position;
        directionVector.Normalize();
        return directionVector;
    }

    void SelfNormalize()
    {
        Debug.Log(GetWorldUp());
        // Quaternion rotation = Quaternion.FromToRotation(transform.up, GetWorldUp());
        Vector3 forwardDirection = Vector3.ProjectOnPlane(transform.forward, GetWorldUp()).normalized;
        Quaternion rotation = Quaternion.LookRotation(forwardDirection, GetWorldUp());

        // Quaternion rotation = Quaternion.LookRotation(GetWorldUp(), Vector3.up);
        // rotation *= Quaternion.Euler(Vector3.right * 90);
        // debugCube.transform.rotation = rotation;
        if (keepSelfNormal)
        {
            
            transform.rotation = rotation;
        } 
    }

    // public Quaternion GetForwardQuaternion()
    // {
    //     Quaternion rotation = Quaternion.LookRotation(GetWorldUp(), transform.up);
    //     rotation *= Quaternion.Euler(Vector3.right * 90);
    //     return rotation;
    // }
}

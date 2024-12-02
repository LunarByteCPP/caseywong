/***************************************************************
file: WorldGravity.cs
author: Alex Mariano
class: CS 4700 – Game Development
assignment: Final Project
date last modified: 12/01/2024

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

    float gravityMultiplier = 1f;

    Rigidbody rb;

    public bool keepSelfNormal;

    public const float NEWTON_CONSTANT = 6.6743E-11f; // N * m^2 / kg^2

    public const float SPECIAL_SAUCE = 1E5f; // This is to scale planet distances.
    //Ex. Earth's Radius is 6378km, and represented at 637.8m in the scnee.
    //An object at the surface of the new scaled surface shoudl experience
    //About 9.81 Newtons of force.

    GalaxyInformation galaxyInformation;

    public const float MICROGRAVITY_THRESHOLD = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        galaxyInformation = GameObject.Find("Galaxy").GetComponent<GalaxyInformation>();
    }

    // Update is called once per frame
    void Update()
    {
        GetNearestPlanet();
        CheckMicroGravity();
        ApplyGravity();
    }

    void FixedUpdate()
    {
        SelfNormalize();
    }
    void ApplyGravity()
    {
        if (center != null)
        {
            rb.AddForceAtPosition(GetGravityForce() * -GetWorldUp(), rb.centerOfMass);
        }

    }

    float GetGravityForce()
    {
        if (center != null)
        {
            float distance = Vector3.Distance(transform.position, center.transform.position) * SPECIAL_SAUCE;
            float massOne = rb.mass;
            float massTwo = center.GetComponent<Rigidbody>().mass * 1E20f;

            float force = NEWTON_CONSTANT * massOne * massTwo / Mathf.Pow(distance, 2);
            return force;
        }
        return 0;
    }


    public Vector3 GetWorldUp()
    {
        if (center != null)
        {
            Vector3 directionVector = transform.position - center.transform.position;
            directionVector.Normalize();
            return directionVector;
        }
        return new Vector3(0, 0, 0);
    }

    void SelfNormalize()
    {
        if (center != null)
        {
            // Debug.Log(GetWorldUp());
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

    }

    public void GetNearestPlanet()
    {
        List<Transform> planets = galaxyInformation.GetAllPlanets();
        float distance = float.PositiveInfinity;
        foreach (Transform planet in planets)
        {
            float testDistance = Mathf.Abs(Vector3.Distance(transform.position, planet.position));
            if (center == null || testDistance <= distance)
            {
                center = planet.gameObject;
                distance = testDistance;
            }
        }
    }

    public void CheckMicroGravity()
    {
        if (center != null)
        {
            if (GetGravityForce() <= MICROGRAVITY_THRESHOLD)
            {
                center = null;
            }
        }
    }

    // public Quaternion GetForwardQuaternion()
    // {
    //     Quaternion rotation = Quaternion.LookRotation(GetWorldUp(), transform.up);
    //     rotation *= Quaternion.Euler(Vector3.right * 90);
    //     return rotation;
    // }
}

/***************************************************************
file: GalaxyInformation.cs
author: Alex Mariano
class: CS 4700 – Game Development
assignment: Final Project
date last modified: 12/01/2024

purpose: This file holds Get and Set methods for finding all
available planets in a scene.
This file should be placed in an object called "GalaxyInformation"

****************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalaxyInformation : MonoBehaviour
{

    private List<Transform> planets;

    // Start is called before the first frame update
    void Start()
    {
        planets = new List<Transform>();
        RegisterAllPlanets(transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void RegisterAllPlanets(Transform parent)
    {
        foreach(Transform child in parent)
        {
            if (child.gameObject.tag == "Planet")
            {
                planets.Add(child);
            }
            RegisterAllPlanets(child);
        }
    }


    public List<Transform> GetAllPlanets()
    {
        return planets;
    }
}

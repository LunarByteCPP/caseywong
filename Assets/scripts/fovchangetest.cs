using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fovchangetest : MonoBehaviour
{
    public Camera thiscamera;
    public float fovchange;
    public KeyCode camerafov = KeyCode.R;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(camerafov))
        {
            thiscamera.fieldOfView = thiscamera.fieldOfView + fovchange;
        }
        if (Input.GetKeyUp(camerafov))
        {
            thiscamera.fieldOfView = thiscamera.fieldOfView - fovchange;
        }
    }
}

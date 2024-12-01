using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movecamera : MonoBehaviour
{
    public Transform cameraposition;
    public Transform rigidBodyTransform;

    void Update()
    {
        transform.position = cameraposition.position;
    }
}

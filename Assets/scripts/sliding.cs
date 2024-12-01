using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerObj;
    private Rigidbody rb;
    private playermovement pm;

    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    private float slideTimer;

    public float slideYScale;
    private float startYScale;

    [Header("Input")]
    public KeyCode slideKey = KeyCode.LeftControl;
    private float horizontalInput;
    private float verticalInput;

    public bool sliding;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<playermovement>();
        startYScale = playerObj.localScale.y;

    }
    private void FixedUpdate()
    {
        sliding = pm.sliding;
        if (pm.sliding)
        {
            SlidingMovement();
        }
    }
    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(slideKey) && (horizontalInput != 0 || verticalInput != 0))
        {
            StartSlide();
        }

        if (Input.GetKeyUp(slideKey) && pm.sliding)
        {
            StopSlide();
        }
    }

    private void StartSlide()
    {
        pm.sliding = true;
        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        rb.AddForce(-transform.up * 5f, ForceMode.Impulse);
        slideTimer = maxSlideTime;
    }
    private void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (slideTimer <= 0)
        {
            StopSlide();
        }
        if (!pm.onslope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

            slideTimer -= Time.deltaTime;
        }
        else
        {   
            rb.AddForce(pm.getslopemovedirection(inputDirection) * slideForce, ForceMode.Force);
        }

    }
    private void StopSlide()
    {
        pm.sliding = false;
        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
    }
}

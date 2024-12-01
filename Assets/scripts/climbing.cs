using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbing : MonoBehaviour
{
    [Header("Refrences")]
    //public GameObject gameobj;
    public Transform orientation;
    public Rigidbody rb;
    public playermovement pm;
    public LayerMask whatIsWall;
    [Header("Climbing")]
    public float climbSpeed;
    public float maxClimbTime;
    public float climbTimer;
    public bool climbing;
    [Header("Climb jumping")]
    public float climbJumpUpForce;
    public float climbJumpBackForce;

    public KeyCode jumpKey = KeyCode.Space;
    public int climbJumps;
    public int climbJumpsLeft;

    [Header("Detection")]
    public float detectionLength;
    public float sphereCastRadius;
    public float maxwallLookAngle;
    private float wallLookAngle;

    private RaycastHit frontWallHit;
    private bool wallFront;

    private Transform lastWall;
    private Vector3 lastWallNormal;
    public float minWallNormalAngleChange;

    [Header("Exiting")]
    public bool exitingWall;
    public float exitWallTime;
    private float exitWallTimer;


    /*private void Start()
    {

        pm = gameobj.GetComponent<playermovement>();
        rb = pm.GetComponent<Rigidbody>();

    }*/
    private void Update()
    {
        wallCheck();
        StateMachiene();

        if (climbing && !exitingWall)
        {
            ClimbingMovement();
        }
    }

    private void StateMachiene()
    {
        //sate 1 climbing
        if (wallFront && Input.GetKey(KeyCode.W) && wallLookAngle < maxwallLookAngle && !exitingWall)
        {
            if (!climbing && climbTimer > 0)
            {
                StartClimbing();
            }
            //timer
            if (climbTimer > 0)
            {
                climbTimer -= Time.deltaTime;
            }
            if (climbTimer < 0)
            {
                StopClimbing();
            }
            if (wallFront && Input.GetKeyDown(jumpKey) && climbJumpsLeft > 0)
            {
                ClimbJump();
            }
        }
        //state2 exit
        else if (exitingWall)
        {
            if (climbing)
            {
                StopClimbing();
            }
            if (exitWallTimer > 0)
            {
                exitWallTimer -= Time.deltaTime;
            }
            if (exitWallTimer < 0)
            {
                exitingWall = false;
            }
        }
        // State3 None
        else
        {
            if (climbing)
            {
                StopClimbing();
            }
        }


    }



    private void wallCheck()
    {
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, detectionLength, whatIsWall);
        wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);

        bool newWall = frontWallHit.transform != lastWall || Mathf.Abs(Vector3.Angle(lastWallNormal, frontWallHit.normal)) > minWallNormalAngleChange;

        if (pm.grounded || (wallFront && newWall))
        {
            climbTimer = maxClimbTime;
            climbJumpsLeft = climbJumps;
        }
    }
    private void StartClimbing()
    {
        climbing = true;
        pm.climbing = true;

        lastWall = frontWallHit.transform;
        lastWallNormal = frontWallHit.normal;

        //function for FOV change maybe
    }
    private void ClimbingMovement()
    {
        rb.velocity = new Vector3(rb.velocity.x, climbSpeed, rb.velocity.z);
        //clmbing movement sound
    }
    private void StopClimbing()
    {
        climbing = false;
        pm.climbing = false;
    }
    private void ClimbJump()
    {
        exitingWall = true;
        exitWallTimer = exitWallTime;
        Vector3 forceToApply = transform.up * climbJumpUpForce + frontWallHit.normal * climbJumpBackForce;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);

        climbJumpsLeft--;
    }
}



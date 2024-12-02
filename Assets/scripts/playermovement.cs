using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://www.youtube.com/watch?v=xCxSjgYTw9c
//https://www.youtube.com/watch?v=tAJLiOEfbQg

public class playermovement : MonoBehaviour
{

    [Header("movement")]


    public float walkspeed;
    public float sprintspeed;
    protected float movespeed;
    public float slidespeed;

    private float lastdesiredmovespeed;
    private float desiredmovespeed;

    public float speedincreasemultiplier;
    public float slopeincreasemultiplier;

    public float grounddrag;
    public float climbspeed;

    [Header("jumping")]
    public float jumpforce;
    public float jumpcooldown;
    public float airmultiplier;
    public bool readytojump;

    [Header("crouching")]
    public float crouchspeed;
    public float crouchyscale;
    private float startyscale;


    [Header("keybinds")]
    public KeyCode jumpkey = KeyCode.Space;
    public KeyCode sprintkey = KeyCode.Mouse1;
    public KeyCode crouchkey = KeyCode.C;




    [Header("Ground Check")]

    public Transform groundcheck;
    public float groundDistance = 0.4f;
    public LayerMask whatisground;
    public bool grounded;

    [Header("slope handling")]
    public float maxslopeangle;
    private RaycastHit slopehit;
    public bool exitingslope;
    public bool slopetouch;

    [Header("Refrences")]
    public Climbing climbingscript;
    public FovChangeTest1 fc;
    public Transform orientation;
    float horizontalinput;
    float verticalinput;
    Vector3 movedirection;

    public GameObject gameobj;
    public Rigidbody rb;


    [Header("movementstate")]
    public movementstate state;

    [Header("displayvariables")]
    public float speed;
    public float fov;
    



    public enum movementstate
    {
        walking,
        sprinting,
        crouching,
        sliding,
        climbing,
        air
    }
    public bool climbing;
    public bool sliding;



    private void Start()
    {
        rb = gameobj.GetComponent<Rigidbody>();
        // rb.freezeRotation = true;

        readytojump = true;
        slopetouch = false;

        startyscale = gameobj.transform.localScale.y;
    }



    private void Update()
    {
        speed = rb.velocity.magnitude;
        fov = fc.currentfov;
        //ground check
        grounded = Physics.CheckSphere(groundcheck.position, groundDistance, whatisground);
        myinput();
        SpeedControl();
        statehandler();
        
        if (onslope())
        {
            slopetouch = true;
        }
        else
        {
            slopetouch = false;
        }



        //handle drag
        if (grounded | onslope())
        {
            rb.drag = grounddrag;
        }
        else
        {
            rb.drag = 0;
        }
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void myinput()
    {
        horizontalinput = Input.GetAxis("Horizontal");
        verticalinput = Input.GetAxis("Vertical");

        //whentojump
        if (Input.GetKey(jumpkey) && readytojump && (grounded || onslope()))
        {
            readytojump = false;

            jump();

            Invoke(nameof(resetjump), jumpcooldown);
        }

        //startcrouch
        if (Input.GetKeyDown(crouchkey) && !sliding&& (grounded || onslope()))
        {
            gameobj.transform.localScale = new Vector3(gameobj.transform.localScale.x, crouchyscale, gameobj.transform.localScale.z);
            rb.AddForce(-transform.up* 5f, ForceMode.Impulse);
        }
        //stop crouch
        if (Input.GetKeyUp(crouchkey) && !sliding )
        {
            gameobj.transform.localScale = new Vector3(gameobj.transform.localScale.x, startyscale, gameobj.transform.localScale.z);
            //rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
        }

    }

    private void statehandler()
    {
        //mode sliding
        if (sliding)
        {
            state = movementstate.sliding;
            if (onslope() && rb.velocity.y < 0.1f)
            {
                desiredmovespeed = slidespeed;
            }
            else
            {
                desiredmovespeed = sprintspeed;
            }
        }

        //mode crouching
        else if ( Input.GetKey(crouchkey) && !sliding && (grounded || onslope()))
        {
            state = movementstate.crouching;
            desiredmovespeed = crouchspeed;
        }
        //mode sprinting
        else if (grounded && Input.GetKey(sprintkey))
        {
            state = movementstate.sprinting;
            desiredmovespeed = sprintspeed;
        }
        //mode walking
        else if (grounded)
        {
            state = movementstate.walking;
            desiredmovespeed = walkspeed;
        }
        //mode air
        else if (!grounded && !climbing)
        {
            state = movementstate.air;
        }
        else if (climbing)
        {
            state = movementstate.climbing;
            desiredmovespeed = climbspeed;

        }

        // check if desiredmovespeed has changed drastically
        if (Mathf.Abs(desiredmovespeed - lastdesiredmovespeed) > 4f && movespeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpmovespeed());
        }
        else
        {
            movespeed = desiredmovespeed;
        }
        lastdesiredmovespeed = desiredmovespeed;
    }

    private IEnumerator SmoothlyLerpmovespeed()
    {
        // smoothly lerp movementSpeed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredmovespeed - movespeed);
        float startValue = movespeed;

        while (time < difference)
        {
            movespeed = Mathf.Lerp(startValue, desiredmovespeed, time / difference);

            if (onslope())
            {
                float slopeAngle = Vector3.Angle(transform.up, slopehit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedincreasemultiplier * slopeincreasemultiplier * slopeAngleIncrease;
            }
            else
            {
                time += Time.deltaTime * speedincreasemultiplier;
            }

            yield return null;
        }

        movespeed = desiredmovespeed;
    }

    private void MovePlayer()
    {
        //calculate movement direction
        movedirection = orientation.forward * verticalinput + orientation.right * horizontalinput;
        if (climbingscript.exitingWall)
        {
            return;
        }


        // on slope
        // if (onslope() && !exitingslope)
        // {
        //     grounded = true;
        //     rb.AddForce(getslopemovedirection(movedirection) * movespeed * 20f, ForceMode.Force);
        //     if (rb.velocity.y > 0)
        //     {
        //         rb.AddForce(-transform.up * 80f, ForceMode.Force);
        //     }
        // }

        // on ground
        else if (grounded)
        {
            rb.AddForce(getslopemovedirection(movedirection) * movespeed * 10f, ForceMode.Force);
        }
        else if (!grounded) // not grounded
        {
            rb.AddForce(movedirection.normalized * movespeed * 10f * airmultiplier, ForceMode.Force);
        }


    }
    private void SpeedControl()
    {
        //limiting speed on slope
        if (onslope() && !exitingslope)
        {
            if (rb.velocity.magnitude > movespeed)
            {
                rb.velocity = rb.velocity.normalized * movespeed;
            }
        }
        //limiting speed on ground or air
        else
        {
            Vector3 flatvel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            //limit velocity if needed
            if (flatvel.magnitude > movespeed)
            {
                Vector3 limitedvel = flatvel.normalized * movespeed;
                rb.velocity = new Vector3(limitedvel.x, rb.velocity.y, limitedvel.z);
            }
        }
    }
    private void jump()
    {
        exitingslope = true;
        //reset y velocity
        // rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(gameobj.transform.up * jumpforce, ForceMode.Impulse);
    }
    private void resetjump()
    {
        readytojump = true;

        exitingslope = false;
    }

    public bool onslope()
    {
        if (Physics.Raycast(gameobj.transform.position, -transform.up, out slopehit, groundDistance + 0.1f))
        {
            grounded = true;
            float angle = Vector3.Angle(transform.up, slopehit.normal);
            return angle < maxslopeangle && angle != 0;

        }

        return false;
    }

    public Vector3 getslopemovedirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopehit.normal).normalized;
    }
}

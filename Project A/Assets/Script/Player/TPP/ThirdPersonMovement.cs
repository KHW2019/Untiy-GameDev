using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    [Header("Componenet")]
    public CharacterController controller;
    public Transform cam;
    public AudioSource audioS;
    public Rigidbody rb;
    public LayerMask whatIsWall;
    public Transform orientation;

    [Header("Movement")]
    public float speed;
    public float gravity;
    public float jumpHeight;

    [Header("Camera")]
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    [Header("Ground Checking")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public bool isGrounded;
    Vector3 velocity;

    [Header("Climbing variables")]
    public float ClimbSpeed;
    public float maxClimbTime;
    private float climbTimer;

    private bool climbing;

    [Header("Climbing Detection")]
    public float DetectionLength;
    public float sphereCastRadius;
    public float maxWallLookAngle;
    private float wallLookAngle;

    private RaycastHit frontWallHit;
    private bool wallFront;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        controller = GetComponent<CharacterController>();
        audioS = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();

        climbing = false;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        FootStep();
        GroundCheck();
        WallCheck();
        StateMachine();

        if (climbing) ClimbingMovement();
    }

    void Movement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);


            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir * speed * Time.deltaTime);
        }
    }

    void FootStep()
    {
        if (controller.isGrounded == true && controller.velocity.magnitude > 2f && audioS.isPlaying == false)
        {
            audioS.Play();
        }

        if (controller.isGrounded == false || controller.velocity.magnitude < 2f || audioS.isPlaying == true)
        {
            audioS.Stop();
        }
    }


    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // applying downforce
        if (isGrounded && velocity.y < 0)
        {
            //Down force
            velocity.y = -1.8f;
            //Debug.Log(isGrounded);
        }

        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            jumpHeight = Random.Range(3f, 7f);
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    private void WallCheck()
    {
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, DetectionLength, whatIsWall);
        wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);

        if (isGrounded)
        {
            climbTimer = maxClimbTime;
        }
    }

    private void StateMachine()
    {
        //State 1 - Climbing
        if (wallFront && Input.GetKey(KeyCode.W) && wallLookAngle < maxWallLookAngle)
        {
            if (!climbing && climbTimer > 0) StartClibming(); Debug.Log("I am going up");

            //State 2 - Timer
            if (climbTimer > 0) climbTimer -= Time.deltaTime;
            if (climbTimer < 0) StopClimbing();
        }

        // State 3 - None
        else
        {
            if (climbing) StopClimbing();
        }
    }

    private void StartClibming()
    {
        climbing = true;
        Debug.Log("climbing");
        //camera fov change

    }

    private void ClimbingMovement()
    {
        rb.velocity = new Vector3(rb.velocity.x, ClimbSpeed, rb.velocity.z);

        //sound effect

    }

    private void StopClimbing()
    {
        climbing = false;

        //particle effect 

    }

}

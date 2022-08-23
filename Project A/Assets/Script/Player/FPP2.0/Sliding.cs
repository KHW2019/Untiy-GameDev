using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{

    [Header("References")]
    public Transform orinentation;
    public Transform playerObj;
    private Rigidbody rb;
    private PlayerMovmentX pm;

    [Header("Sliding")]
    public float maxSlidingTimer;
    public float slideForce;
    private float slideTimer;

    public float slideYscale;
    private float startYscale;

    [Header("Input")]
    public KeyCode Slidekey = KeyCode.LeftControl;
    private float horizontalInput;
    private float verticalInput;

    //private bool sliding;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovmentX>();

        startYscale = playerObj.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKeyDown(Slidekey) && (horizontalInput != 0 || verticalInput != 0) && pm.grounded)
        {
            startSlide();
        }

        if(Input.GetKeyUp(Slidekey) && pm.sliding)
        {
            stopSlide();
        }
    }

    private void FixedUpdate()
    {
        if (pm.sliding)
            slidingMovement();
    }

    void startSlide()
    {
        pm.sliding = true;

        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYscale, playerObj.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        slideTimer = maxSlidingTimer;

    }

    private void slidingMovement()
    {
        Vector3 inputDirection = orinentation.forward * verticalInput + orinentation.right * horizontalInput;

        //sliding normal 
        if(!pm.OnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

            slideTimer -= Time.deltaTime;
        }

        //sliding down a slope
        else
        {
            rb.AddForce(pm.getSlopMoveDirection(inputDirection) * slideForce, ForceMode.Force);
        }

        //Debug.Log(slideTimer);

        if (slideTimer <= 0) 
            stopSlide();
    }

    private void stopSlide()
    {
        pm.sliding = false;

        playerObj.localScale = new Vector3(playerObj.localScale.x, startYscale, playerObj.localScale.z);
    }
}

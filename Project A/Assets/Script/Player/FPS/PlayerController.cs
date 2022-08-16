using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public CharacterController cc;
    public Rigidbody Rb;

    public float speed;
    public float gravity;
    public float jumpHeight;
    

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public AudioSource audioS;
    Vector3 velocity;
    bool isGrounded;


    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        audioS = GetComponent<AudioSource>();
        Rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        Footstep();
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

        //if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        //{

        //}

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        cc.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpHeight = Random.Range(3f,7f);
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        cc.Move(velocity * Time.deltaTime);

        //applying footstep by using velocity
        if (isGrounded && Rb.velocity.magnitude > 2 && audioS.isPlaying == false)
        { 
            audioS.Play();

            audioS.volume = Random.Range(0.8f, 1);
            audioS.pitch = Random.Range(0.8f, 1.1f);

            Debug.Log(velocity.magnitude);
        }
        else
        {
            audioS.Stop();
        }
    }

    void Footstep()
    {
        //if (Input.GetKeyDown(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        //{
        //    audioS.Play();
        //    //if (Input.GetKey(KeyCode.LeftShift))
        //    //{
        //    //    footstepsSound.enabled = false;
        //    //    sprintSound.enabled = true;
        //    //}
        //    //else
        //    //{
        //    //    footstepsSound.enabled = true;
        //    //    sprintSound.enabled = false;
        //    //}
        //}
        //else
        //{
        //    audioS.Stop();
        //    //footstepsSound.enabled = false;
        //    //sprintSound.enabled = false;
        //}
    }
}

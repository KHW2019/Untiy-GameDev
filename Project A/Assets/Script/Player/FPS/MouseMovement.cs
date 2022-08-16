using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public float mouseSensitivity = 100f;

    public Transform playerBody;

    //float mouseX;
    //float mouseY;

    float xRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //check player x and y movement by Time.delta time
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //decrase x roatiation base on mouseY
        xRotation -= mouseY;

        //clamping
        xRotation = Mathf.Clamp(xRotation, -90f, 50f);

        //apply y rotation
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        //picking up x movment
        playerBody.Rotate(Vector3.up * mouseX);
    }
}

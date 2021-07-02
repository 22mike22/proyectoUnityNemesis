using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playermovent : MonoBehaviour
{
    public float RotationSpeed = 1.0f;
    private bool moverCamara = true;
    private bool menu = false;

    private Rigidbody Physics;
    //public Camera FPSCamera;
    


    // Start is called before the first frame update
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = true;
        Physics = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) menu = !menu;
        if (menu)
        {
            float rotationY = Input.GetAxis("Mouse X") * RotationSpeed;
            if (Input.GetKeyDown(KeyCode.Q)) moverCamara = !moverCamara;
            if (moverCamara)
                transform.Rotate(0, rotationY, 0);

            if (Input.GetKeyDown(KeyCode.W))
                transform.Translate(new Vector3(0, 0, 1));

            if (Input.GetKeyDown(KeyCode.S))
                transform.Translate(new Vector3(0, 0, -1));

            if (Input.GetKeyDown(KeyCode.A))
                transform.Translate(new Vector3(-1, 0, 0));

            if (Input.GetKeyDown(KeyCode.D))
                transform.Translate(new Vector3(1, 0, 0));

            if (Input.GetKeyDown(KeyCode.Space))
                transform.Translate(new Vector3(0, 1, 0));

            if (Input.GetKeyDown(KeyCode.LeftShift))
                transform.Translate(new Vector3(0, -1, 0));
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour
{
    float velocidad = 1f;
    public static bool moverCamara = true;
    private bool menu = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float rotar = Input.GetAxis("Mouse Y")*velocidad;
        if (Input.GetKeyDown(KeyCode.Q)) moverCamara = !moverCamara;
        if (Input.GetKeyDown(KeyCode.M)) menu = !menu;
        if (moverCamara && menu)
            transform.Rotate(-rotar, 0, 0); 
    }
}

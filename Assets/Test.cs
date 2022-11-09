using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    int[] vel;
    Rigidbody rb;
    void Start()
    {
        int[] vel = { 0, 0, 0 };
        rb= GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        int fscale = 250;
        if (Input.GetKeyDown(KeyCode.W))
        {
            //rb.velocity = new Vector3(x,y,z);
            rb.AddForce(transform.forward*fscale);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            //rb.velocity = new Vector3(x,y,z);
            rb.AddForce(transform.forward * -1*fscale);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            //rb.velocity = new Vector3(x,y,z);
            rb.AddForce(transform.right* -1*fscale);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            //rb.velocity = new Vector3(x,y,z);
            rb.AddForce(transform.right * fscale);
        }
    }
}

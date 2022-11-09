using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net;
using System.IO.Pipes;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEditor.PackageManager;

public class controlller : MonoBehaviour
{
    private TcpClient socketConnection;

    // Start is called before the first frame update
    public float speed = 10;
    Rigidbody rb;
    TcpListener server = null;
    Byte[] bytes = new Byte[2];
    public string fin;
    String data = null;
    NetworkStream stream;
    Thread clientReceiveThread;

  
    private void ListenForData()
    {
        try
        {
            socketConnection = new TcpClient("127.0.0.1", 8021);
            Byte[] bytes = new Byte[256];
            while (true)
            {
                // Get a stream object for reading 				
                using (NetworkStream stream = socketConnection.GetStream())
                {
                    int length;
                    // Read incomming stream into byte arrary. 					
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var incommingData = new byte[length];
                        Array.Copy(bytes, 0, incommingData, 0, length);
                        // Convert byte array to string message. 						
                        string serverMessage = Encoding.UTF8.GetString(incommingData);
                        
                        fin = serverMessage;
                        //Debug.Log("server message received as: " + serverMessage);
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }
        void Start()
        {
            rb = GetComponent<Rigidbody>();
        try
        {
            clientReceiveThread = new Thread(new ThreadStart(ListenForData));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);
        }
        //makeserv();
    }

        // Update is called once per frame
        void Update()
        {
            
            speed = 20;
            GetComponent<Renderer>().material.color = Color.red;
            Vector3 sv = Vector3.zero;
            //float hInput = Input.GetAxis("Horizontal");
            //float vInput = Input.GetAxis("Vertical");
            if (Input.GetKey(KeyCode.LeftControl))
            {
                speed = 50;
            }
            if (Input.GetKey(KeyCode.Space)||fin=="1")
            {
                rb.AddForce(new Vector3(0, speed, 0));
            }
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                rb.AddForce(new Vector3(0, -1 * speed, 0));
            }
            if (Physics.Raycast(transform.position, Vector3.down, 1.2f))
            {
                GetComponent<Renderer>().material.color = Color.white;
                
            }
            else
            {
                speed /= 5;
            }
            if (Input.GetKey(KeyCode.W) || fin=="2")
            {
                sv.x = speed;
            }
            else if (Input.GetKey(KeyCode.S)||fin=="3")
            {
                sv.x = -1 * speed;
            }
            if (Input.GetKey(KeyCode.D)||fin=="4")
            {
                sv.z = -1 * speed;
            }
            else if (Input.GetKey(KeyCode.A) || fin == "5")
            {
                sv.z = speed;
            }
            rb.velocity = sv;
        //fin = 0;
        }
        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag("ground")) {
                Vector3 _ = rb.velocity;
                _.y = 0;
                rb.velocity = _;
            }
            //else if(collision.gameObject.)
        }
    } 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camscript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
    Vector3 mypos,myrot;
    //private int nof;
    void Start()
    {
        myrot = new Vector3(0, 90, 0);
        transform.eulerAngles = new Vector3(0,90,0);
    }
    void Update()
    {
        float deltaangle = 0.5f;
        Vector3 _ = transform.eulerAngles;
        
        //return to normal position
        if (_.y!=90.0f) 
        {
            _.y = (_.y > 90.0f) ? (_.y - 0.1f) : (_.y + 0.1f);
        }
        if (_.x != 0.0f)
        {
            if (_.x > 180)
            {
                _.x += 0.1f;
            }
            else {
                _.x -= 0.1f;
            }
        }
        //Change camera angle from user
        if (Input.GetKey(KeyCode.E)|| Input.GetKey(KeyCode.RightArrow))
        {
            _.y += deltaangle;
            
        }
        else if(Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow))
        {
            _.y -= deltaangle;
            
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            _.x += deltaangle;
            
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            _.x -= deltaangle;
            
        }
        

        //Stutter correction
        if ((_.x > 0.000001f && _.x < 0.1f)|| (_.x > 359.000001f && _.x < 359.1f)) { _.x = 0.0f;}
        if ((_.y > 90.0001f && _.y < 90.1) || (_.y > 89.0001f && _.y < 89.01)) { _.y = 90f; }
        //Stutter fixed


        mypos = player.transform.position;
        mypos.x -= 10;
        mypos.y += 2;
        transform.position = mypos;
        transform.eulerAngles = _;
    }
    
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMovement : MonoBehaviour , IGroundMovementSpeed
{
    public float GroundSpeed;
    private Transform Ground;

    Rigidbody2D[] GroundComponent;

    // Start is called before the first frame update
    void Start()
    {
       // GroundComponent = GetComponentsInChildren<Rigidbody2D>();
        Ground = GetComponent<Transform>();
      
    }

    // Update is called once per frame
    void Update()
    {

        Ground.transform.position += new Vector3(GroundSpeed * Time.deltaTime, 0, 0);     

    /*  foreach(Rigidbody2D i in GroundComponent)
        i.velocity = new Vector2(GroundSpeed, 0);
    */
    }

    public void SetMovementSpeed(float MS){
        GroundSpeed = MS;
    }

}

    

    

   

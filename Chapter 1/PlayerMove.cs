using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

public float speed;    
public ColyseusClient myClient; 
Rigidbody2D player; 

void Start(){
    //Get the rigidbody2d of the gameObject this script is assigned to.
    player = GetComponent<Rigidbody2D>();
}

void FixedUpdate() {
    //Determine the direction of the movement based on user input.
    float moveX = Input.GetAxis("Horizontal");
    float moveY = Input.GetAxis("Vertical");

    //Calculate the velocity of the gameObject.
    player.velocity = new Vector2(moveX * speed, moveY * speed);
    if (player.velocity != Vector2.zero) {
        myClient.OnPlayerMove();
    }
}
}

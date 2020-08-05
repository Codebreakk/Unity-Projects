using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** This was all done with the help of the following guide:
* https://craftgames.co/unity-2d-platformer-movement/
*/

public class p1_script : MonoBehaviour
{
  Rigidbody2D rb; // reference for the Rigidbody of the playable character
  public float speed; // the speed at which the character shall move
  public float jumpForce; // the jump height
  bool isGrounded = false; // true if the player is grounded, false otherwise
  // Transform of an empty object placed bellow player to check if it's feet are close to the ground
  public Transform isGroundedChecker;
  public float checkGroundRadius; // Radius of the GroundChecker
  public LayerMask groundLayer; // the layer for the ground
  public float fallMultiplier = 2.5f;
  public float lowJumpMultiplier = 2f;
  public float rememberGroundedFor;
  float lastTimeGrounded;
  public int defaultAdditionalJumps = 1; // the maximum number of additional jumps allowed per character.
  int additionalJumps;

  // Start is called before the first frame update
  void Start()
  {
    rb = GetComponent<Rigidbody2D>();
  }

  // Update is called once per frame
  void Update()
  {
    Move();
    Jump();
    BetterJump();
    CheckIfGrounded();
  }

  // Move is responsible for calculating the movement of the character
  void Move(){
    // Get the movement input in the x axis
    float x = Input.GetAxisRaw("Horizontal");
    // Calculate how much the character must move
    float moveBy = x * speed;
    // Set the velocity of the character's Rigidbody
    rb.velocity = new Vector2(moveBy, rb.velocity.y);
  }

  // Jump is responsible for executing the "jump" action when the Space bar is pressed
  void Jump(){
    // if grounded then we jump, but if not grounded we check if the time since we were grounded is less or equal to "rememberGroundedFor"
    if(Input.GetKeyDown(KeyCode.Space) && (isGrounded || Time.time - lastTimeGrounded <= rememberGroundedFor || additionalJumps > 0)){
      rb.velocity = new Vector2(rb.velocity.x, jumpForce);
      additionalJumps--;
    }
  }

  // This is a better jump, similar to the one in Super Mario games. We still need the "Jump" method
  void BetterJump(){
    if(rb.velocity.y < 0){ // the character is falling after jumping
      rb.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
    }else if(rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space)){
      // The player is still pressing the jump button
      rb.velocity += Vector2.up * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
    }
  }

  // Checks if the character is on the ground
  void CheckIfGrounded(){
    // collider has the value returned by the OverlapCircle
    Collider2D collider = Physics2D.OverlapCircle(isGroundedChecker.position, checkGroundRadius, groundLayer);
    // Physics2D.OverlapCircle(position, radius, layer);

    // If collider is null, the character is on the air, otherwise it's on the ground
    if(collider != null){
      isGrounded = true;
      additionalJumps = defaultAdditionalJumps;
    }else{
      if(isGrounded){
        // Saves the last time the character was on the ground
        lastTimeGrounded = Time.time;
      }
      isGrounded = false;
    }
  }
}

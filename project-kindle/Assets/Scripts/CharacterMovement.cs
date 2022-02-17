using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    //constants for movement
    private float maxSpeed = 8.0f;
    private float jumpHeight = 2.0f;
    private float moveAccel = 16.0f;
    
    private LayerMask PlatformLayer = 64;
    //bools for jumping
    //bool to check if the jump button is being held and if we are in a jump
    bool jumpIsHeld = false;
    bool inJump = false;
    bool isGrounded = false;

    //variable to hold the components in case we need to manipulate it
    private Rigidbody2D rigBod;
    private BoxCollider2D hitbox;

    //variable to hold the current movement speed
    private float moveSpeed = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        rigBod = gameObject.GetComponent<Rigidbody2D>();
        hitbox = gameObject.GetComponent<BoxCollider2D>();
        Debug.Log("Tatu wuz here!");
        rigBod.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = checkGround();
        //calls movementHandler every frame
        movementHandler();

    }

    //Handles movement by check if left or right are being pressed or if the jump key is being held down
    void movementHandler(){
        Vector3 moveVect = new Vector3(moveSpeed, 0.0f, 0.0f) * Time.deltaTime;
        
        //*****MOVING********
        //when right arrow is held down and the speed is less than the maxspeed, accelerate in the positive or negative x direction
        //respectively
        if(Input.GetKey("right") && moveSpeed > -maxSpeed){
            moveSpeed = moveSpeed + (moveAccel * Time.deltaTime);
        }

        else if(Input.GetKey("left") && moveSpeed < maxSpeed){
            moveSpeed = moveSpeed - (moveAccel * Time.deltaTime);
        }

        //if neither button is pressed, slow down to zero if we're still movin'
        else{
            if(moveSpeed > (moveAccel*Time.deltaTime)){
                moveSpeed = moveSpeed - (moveAccel * Time.deltaTime);
            }
            else if(moveSpeed < (-moveAccel*Time.deltaTime)){
                moveSpeed = moveSpeed + (moveAccel * Time.deltaTime);
            }
            //set movespeed to zero in any other case to prevent weirdness.
            else{
                moveSpeed = 0;
            }
        }

        //apply the speed
        moveVect = new Vector3(moveSpeed * Time.deltaTime, 0.0f, 0.0f);
        transform.position += moveVect;

        //****JUMPING*****
        //calculate the jump force
        float jumpForce = Mathf.Sqrt(2 * Physics2D.gravity.magnitude * jumpHeight);

        //counter jumpForce to push down on object
        Vector2 counterJumpForce = new Vector2(0.0f, -jumpForce);

        if(Input.GetKeyDown("z")){
            jumpIsHeld = true;

            if(isGrounded){
                inJump = true;
                rigBod.AddForce(Vector2.up * jumpForce * rigBod.mass, ForceMode2D.Impulse);
            }
        }

        else if(Input.GetKeyUp("z")){
            jumpIsHeld = false;
        }

        //apply the counter force while sprite is in the air
        if(inJump){
            if(!jumpIsHeld && Vector2.Dot(rigBod.velocity, Vector2.up) > 0){
                rigBod.AddForce(counterJumpForce * rigBod.mass);
            }
        }
    }

    public bool checkGround(){
        //basically sees if the box around the sprite is in contact with the floor. Return whatever the results are.
        float extraHeight = 1.0f;

        //boxsize is shortened a bit to avoid detecting walls
        Vector3 boxSize = hitbox.bounds.size - new Vector3(0.1f, 0f, 0f);
        RaycastHit2D hit = Physics2D.BoxCast(hitbox.bounds.center, boxSize, 0f, Vector2.down, extraHeight, PlatformLayer);

        return hit.collider != null;
    }
}

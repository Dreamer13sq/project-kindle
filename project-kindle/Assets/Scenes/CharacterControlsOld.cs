//Basically your imports from C++, usually will just be unityengine, but refer to 
//unity docs if you need to import something else.
using UnityEngine;
using UnityEngine.SceneManagement;

//Make sure the name of the class is the same as the name of the file!!!
public class CharacterControlsOld : MonoBehaviour
{
    //Constants
    private float maxSpeed = 8;
    private float jumpForce = 4.0f;
    //Setting up the RigidBody2D object so that we can mess with it in code
    private Rigidbody2D rigBod;

    //called when the object first enters the scene, only ever called once.
    private void Start() {
        //grabbing the sprite's (or objects) rigidbody2d component so that we can mess with it
        rigBod = gameObject.GetComponent<Rigidbody2D>();
        //basically your cout for unity, will spit out a message in the console
        Debug.Log("Tatu wuz here!");
        rigBod.freezeRotation = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //call the movement handler every frame
        movementHandler();
    }

    //movement handler checks if the player is holding down any movement keys or the jump buttons, and makes
    //kindle move accordingly
    void movementHandler() {
        //********MOVEMENT************
        //Input.GetAxis() gets the value of whatever is being pressed down. "Horizontal" checks for the left and right arrows,
        //A and D, and the left and right stick of a controller. for keyboard controls, slowly builds up to -1 and 1.
        float leftRite = Input.GetAxis("Horizontal"); //left = -1, right = 1
 
        //Vectors are basically arrays of floats. access them using moveVect.x, y, z
        //timedelta is the time it takes for 1 frame to process. we use it here to smooth out
        //the movement
        Vector3 moveVect = new Vector3(leftRite, 0.0f, 0.0f) * Time.deltaTime * maxSpeed;
        Vector2 jumpStr = new Vector2(0, jumpForce);

        //transform.position gets the current position of the sprite, you can mess with it however you like
        transform.position += moveVect;

        //*********JUMPING*************

        //if the z key is being pushed down, and while we are not already in the air, we jump.
        //the second part of this condition is so the player can't double jump.
        if(Input.GetKeyDown("z") && Mathf.Abs(rigBod.velocity.y) < 0.001f){
            //adds a force that causes the sprite to move with a certain strength.
            rigBod.AddForce(jumpStr, ForceMode2D.Impulse);
        }
    }
}
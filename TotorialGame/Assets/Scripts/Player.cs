using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float moveForce = 10f;
    [SerializeField]
    private float jumpForce = 11f;

    private float movementX;
    
    private SpriteRenderer sr;
    private Rigidbody2D myBody;
    private Animator anim;
    private const string WALK_ANIMATION = "Walk";

    private bool isGrounded = true;
    private string GROUND_TAG = "Ground";

    private void Awake() {
        myBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        PlayerMoveKeyboard();
        AnimatePlayer();
        
    }

    private void FixedUpdate() {
        PlayerJump();
    }

    void PlayerMoveKeyboard() {
        movementX = Input.GetAxisRaw("Horizontal");
        // Time.deltaTime is used to smooth the movement of the player
        transform.position += new Vector3(movementX, 0f, 0f) * Time.deltaTime * moveForce;
    }

    void AnimatePlayer(){
        
        if(movementX > 0){
            // moving right
            anim.SetBool(WALK_ANIMATION, true);
            sr.flipX = false;
        } else if (movementX < 0) {
            // moving left
            anim.SetBool(WALK_ANIMATION, true);
            sr.flipX = true;
        } else {
            // not moving
            anim.SetBool(WALK_ANIMATION, false);

        }
    }

    private void PlayerJump() {
        if(Input.GetButtonDown("Jump") && isGrounded ) {
            isGrounded = false;
            myBody.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision){
        if (collision.gameObject.CompareTag(GROUND_TAG)) 
            isGrounded = true;
    }

} // class
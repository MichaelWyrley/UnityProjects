using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove2D : MonoBehaviour
{
    public float speedx = 12f;
    public float speedz = 6f;
    public float jumpHeight = 10f;
    public CharacterController controller;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public Vector3 Position {
        get {
            return transform.position;
        }
    }

    private bool isGrounded;

    Vector3 velocity;
    public float gravity = -9.81f;

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0){
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x * speedx + transform.forward * z * speedz;
        controller.Move(move * Time.deltaTime);

        if((Input.GetAxisRaw("Jump") > 0) && isGrounded){
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime * 1/2);

    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Acid") {
            Destroy(this.gameObject);
        } else if (other.tag == "Spike") {
            Destroy(this.gameObject);
        }
    }

    public void toggleEnable(bool toggle){
        gameObject.SetActive(toggle);
    }

    void OnControllerColliderHit(ControllerColliderHit hit){
        if (hit.collider.CompareTag("Movable")){
            Rigidbody body = hit.collider.attachedRigidbody;
            if (body == null) 
                return;
            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
            body.velocity = pushDir * speedx;
        }
    }


}

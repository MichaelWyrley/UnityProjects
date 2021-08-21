using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove3D : MonoBehaviour
{   

    [SerializeField]
    private float speedx = 12f;
    [SerializeField]
    private float speedy = 6f;
    private CharacterController controller;
    private PlayerMove2D player2D;

    public Vector3 Position {
        get {
            return transform.position;
        }
    }
    void Awake() {
        controller = gameObject.GetComponent(typeof(CharacterController)) as CharacterController;
        player2D = FindObjectOfType<PlayerMove2D>();
    }

    void OnEnable() {
        transform.position = player2D.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // print(1f / Time.unscaledDeltaTime);

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Jump");


        Vector3 move = transform.right * x * speedx + transform.up * y * speedy;
        controller.Move(move * Time.deltaTime);

    }

    public void toggleEnable(bool toggle){
        gameObject.SetActive(toggle);
    }
}
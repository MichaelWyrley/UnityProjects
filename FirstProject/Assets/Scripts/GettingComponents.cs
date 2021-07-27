using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GettingComponents : MonoBehaviour
{
    // Start is called before the first frame update

    private Rigidbody2D myBody;
    private BoxCollider2D myCollider;
    private AudioSource audioSource;
    private Animator anim;
    private Transform myTransform;

    private Player p = new Player();

    void Start()
    {
        myBody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        myTransform = transform;
        myTransform.position = new Vector3(10,20,30);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Bobing")]
    [SerializeField]
    [Range(0,100)]
    private float xSpeed;

    [SerializeField]
    [Range(0,100)]
    private float ySpeed;

    [SerializeField]
    [Range(0,100)]
    private float zSpeed;
    
    [SerializeField]
    [Range(0,1)]
    private float xHeight;

    [SerializeField]
    [Range(0,1)]
    private float yHeight;

    [SerializeField]
    [Range(0,1)]
    private float zHeight;

    [Header("Rotation")]
    [SerializeField]
    [Range(0,100)]
    private float xRot;

    [SerializeField]
    [Range(0,100)]
    private float yRot;

    [SerializeField]
    [Range(0,100)]
    private float zRot;



    // Update is called once per frame
    void Update()
    {
        // make the object spin
        transform.Rotate (Time.deltaTime * xRot,Time.deltaTime * yRot, Time.deltaTime * zRot);

        // make the object bob up and down
        Vector3 pos = transform.position;
        pos.x += Mathf.Sin(Time.time * xSpeed) * xHeight;
        pos.y += Mathf.Sin(Time.time * ySpeed) * yHeight;
        pos.z += Mathf.Sin(Time.time * zSpeed) * zHeight;

    
        transform.position = pos;
    }
}

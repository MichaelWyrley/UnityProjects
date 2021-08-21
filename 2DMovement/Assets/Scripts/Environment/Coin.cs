using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [Range (0,100)]
    public float rotSpeed;
    [Range (0,1)]
    public float bobSpeed;
    [Range (0,1)]
    public float bobHeight;

    private DoorScript door;

    void Start(){
        door = FindObjectOfType<DoorScript> ();
    }


    // Update is called once per frame
    void Update()
    {
        // make the object spin
        transform.Rotate (0,Time.deltaTime * rotSpeed, 0);

        // make the object bob up and down
        Vector3 pos = transform.position;
        pos.y += Mathf.Sin(Time.time * bobSpeed) * bobHeight;

    
        transform.position = pos;
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            door.coinCollect(this);
            Destroy(this.gameObject);
            door.LastCoin();
        }
    }

    public Color getColour() {
        return GetComponent<Shape>().colour;
    }
}

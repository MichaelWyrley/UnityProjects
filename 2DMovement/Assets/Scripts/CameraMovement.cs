using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
    
    [SerializeField]
    private Transform player;
    private Vector3 tempPos;
    public float height;
    public float distance;

    

    void LateUpdate() {
        if(!player)
            return;
            
        tempPos = transform.position;
        tempPos.x = player.position.x;
        tempPos.y = player.position.y+height;
        tempPos.z = player.position.z-distance;

        transform.position = tempPos;
    }

}

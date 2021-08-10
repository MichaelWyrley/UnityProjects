using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
    
    [SerializeField]
    private Transform player;
    private Vector3 tempPos;

    

    void LateUpdate() {
        if(!player)
            return;
            
        tempPos = transform.position;
        tempPos.x = player.position.x;
        tempPos.y = player.position.y+3.5f;
        tempPos.z = player.position.z-15f;

        transform.position = tempPos;
    }

}

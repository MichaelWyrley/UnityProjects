using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutMovement : MonoBehaviour
{
    
    [SerializeField]
    private Transform player;
    private Vector3 tempPos;

    void LateUpdate() {
        if(!player)
            return;
            
        tempPos = transform.position;
        tempPos.x = player.position.x;
        tempPos.y = player.position.y;
        tempPos.z = player.position.z-0.1f;

        transform.position = tempPos;
    }

    public void toggleEnable(bool toggle){
        gameObject.SetActive(toggle);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
    
    private bool is2D = true;
    private Vector3 tempPos;
    public float height2D;
    public float distance2D;
    public float height3D;
    public float distance3D;

    private PlayerMove2D player2D;
    private CutMovement player2DCut;
    private PlayerMove3D player3D;

    void Start() {
        player2D = FindObjectOfType<PlayerMove2D>();
        player2DCut = FindObjectOfType<CutMovement>();
        player3D = FindObjectOfType<PlayerMove3D>();
    }

    void Update() {
        if(Input.GetButtonDown("Perspective")){
            is2D = !is2D;
            changePlayer();
        } 
    }

    void LateUpdate() {
        tempPos = transform.position;
        if(is2D){
            if(!player2D) return;
            tempPos.x = player2D.Position.x;
            tempPos.y = player2D.Position.y+height2D;
            tempPos.z = player2D.Position.z-distance2D;
        } else {
            if(!player3D) return;
            tempPos.x = player3D.Position.x;
            tempPos.y = player3D.Position.y+height3D;
            tempPos.z = player3D.Position.z-distance3D;
        }
        transform.position = tempPos;
    }

    void changePlayer(){
        if(player2D != null)
            player2D.toggleEnable(is2D);
        if(player2DCut != null)
            player2DCut.toggleEnable(is2D);
        if(player3D != null)
            player3D.toggleEnable(!is2D);
    }

}

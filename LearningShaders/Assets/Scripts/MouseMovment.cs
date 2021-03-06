using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovment : MonoBehaviour
{
    Camera camera;
    RaycastHit hit;
    Ray ray;
    Vector3 mousePos, smoothPoint;
    public float radius, softness, smoothSpeed, scaleFactor;

    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.UpArrow)){
            radius += scaleFactor * Time.deltaTime;
        } else if(Input.GetKey(KeyCode.DownArrow)){
            radius -= scaleFactor * Time.deltaTime;
        } else if(Input.GetKey(KeyCode.LeftArrow)){
            softness -= scaleFactor * Time.deltaTime;
        } else if(Input.GetKey(KeyCode.RightArrow)){
            softness += scaleFactor * Time.deltaTime;
        }
        Mathf.Clamp(radius, 0, 100);
        Mathf.Clamp(softness, 0, 100);

        mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y,0);
        ray = camera.ScreenPointToRay(mousePos);

        if(Physics.Raycast(ray, out hit)){
            smoothPoint = Vector3.MoveTowards(smoothPoint, hit.point, smoothSpeed*Time.deltaTime);
            Vector4 pos = new Vector4(smoothPoint.x, smoothPoint.y, smoothPoint.z, 0);
            Shader.SetGlobalVector("GLOBALmask_Position", pos);
        }
        Shader.SetGlobalFloat("GLOBALmask_Radius", radius);
        Shader.SetGlobalFloat("GLOBALmask_Softness", softness);
    }
}

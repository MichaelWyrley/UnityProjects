using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class RaymarchCamera : SceneViewFilter
{
    [SerializeField]
    private Shader shader;

    public Material raymarchMaterial {
        get { 
            if (!raymarchMat && shader){
                raymarchMat = new Material(shader);
                raymarchMat.hideFlags = HideFlags.HideAndDontSave;
            }
            return raymarchMat; 
        }
    }

    private Material raymarchMat;

    public Camera _camera {
        get {
            if(!cam) {
                cam = GetComponent<Camera>();
            }
            return cam;
        }
    }
    private Camera cam;

    public Transform _directionalLight;

    public float _maxDistance;
    public Vector4 _sphere1;
    public Vector4 _box1;
    public float _transparency;
    public Color _mainColour;
    public Vector3 _modInterval;
    List<ComputeBuffer> buffersToDispose;


    private void OnRenderImage(RenderTexture source, RenderTexture destination){
        if(!raymarchMaterial){
            Graphics.Blit(source, destination);
            return;
        }
        buffersToDispose = new List<ComputeBuffer> ();
        SendShapes();

        raymarchMaterial.SetMatrix("_CamFrustum", CamFrustum(_camera));
        raymarchMaterial.SetMatrix("_CamToWorldMatrix", _camera.cameraToWorldMatrix);
        raymarchMaterial.SetFloat("_maxDistance", _maxDistance);
        raymarchMaterial.SetVector("_sphere1", _sphere1);
        raymarchMaterial.SetVector("_box1", _box1);
        raymarchMaterial.SetVector("_lightDirection", _directionalLight ? _directionalLight.forward : Vector3.down);
        raymarchMaterial.SetFloat("_transparency", _transparency);
        raymarchMaterial.SetColor("_mainColour", _mainColour);
        raymarchMaterial.SetVector("_modInterval", _modInterval);

        RenderTexture.active = destination;
        raymarchMaterial.SetTexture("_MainTex", source);

        GL.PushMatrix();
        GL.LoadOrtho();
        raymarchMaterial.SetPass(0);
        GL.Begin(GL.QUADS);

        // bottem left of quad
        GL.MultiTexCoord2(0, 0.0f, 0.0f);
        GL.Vertex3(0.0f, 0.0f, 3.0f); // last one corresponds to row in the matrix of frustum
        // bottem right of quad
        GL.MultiTexCoord2(0, 1.0f, 0.0f);
        GL.Vertex3(1.0f, 0.0f, 2.0f);
        // top right of quad
        GL.MultiTexCoord2(0, 1.0f, 1.0f);
        GL.Vertex3(1.0f, 1.0f, 1.0f);
        // top left of quad
        GL.MultiTexCoord2(0, 0.0f, 1.0f);
        GL.Vertex3(0.0f, 1.0f, 0.0f);

        GL.End();
        GL.PopMatrix();

        foreach (var buffer in buffersToDispose) {
            buffer.Dispose ();
        }
        
    }
    // get the positions of the trapese that is projected from the camera
    private Matrix4x4 CamFrustum(Camera cam){
        Matrix4x4 frustum = Matrix4x4.identity;
        float fov = Mathf.Tan((cam.fieldOfView * 0.5f) * Mathf.Deg2Rad);

        Vector3 goUp = Vector3.up * fov;
        Vector3 goRight = Vector3.right * fov * cam.aspect;

        Vector3 topLeft = (-Vector3.forward - goRight + goUp); // go back to camera then move left (-right) and up
        Vector3 topRight = (-Vector3.forward + goRight + goUp);
        Vector3 bottemLeft = (-Vector3.forward - goRight - goUp);
        Vector3 bottemRight = (-Vector3.forward + goRight - goUp);

        frustum.SetRow(0, topLeft);
        frustum.SetRow(1, topRight);
        frustum.SetRow(2, bottemRight);
        frustum.SetRow(3, bottemLeft);

        return frustum;
    } 

    private void SendShapes() {
        List<Shape> allShapes = new List<Shape> (FindObjectsOfType<Shape> ());

        ShapeData[] shapeData = new ShapeData[allShapes.Count];
        for (int i = 0; i < allShapes.Count; i++) {
            var s = allShapes[i];
            shapeData[i] = new ShapeData () {
                position = s.Position,
                scale = s.Scale, 
                colour = s.colour,
                shapeType = (int) s.shapeType,
                operation = (int) s.operation,
                blendStrength = s.blendStrength*3,
            };
        }

        ComputeBuffer shapeBuffer = new ComputeBuffer (shapeData.Length, ShapeData.GetSize ());
        shapeBuffer.SetData (shapeData);
        raymarchMaterial.SetBuffer ("shapes", shapeBuffer);
        raymarchMaterial.SetInt ("_noShapes", shapeData.Length);

        buffersToDispose.Add (shapeBuffer);
        
    }

    struct ShapeData {
        public Vector3 position;
        public Vector3 scale;
        public Vector4 colour;
        public int shapeType;
        public int operation;
        public float blendStrength;

        public static int GetSize () {
            return sizeof (float) * 11 + sizeof (int) * 2;
        }
    }

}

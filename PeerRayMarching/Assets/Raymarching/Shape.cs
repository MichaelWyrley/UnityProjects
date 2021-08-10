using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour
{
    public enum ShapeType {Ellipsoid,Cube, Torus};
    public enum Operation {None, Blend, Cut,Mask};

    public ShapeType shapeType;
    public Operation operation;
    public Color colour = Color.white;
    [Range(0.0f,1.0f)]
    public float blendStrength;

    public Vector3 Position {
        get {
            return transform.position;
        }
    }

    public Vector3 Scale {
        get {
            return transform.localScale;
        }
    }

    public Quaternion Rotation {
        get {
            return transform.rotation.normalized;
        }
    }
}

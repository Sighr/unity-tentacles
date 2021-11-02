using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleDegreeJoint : MonoBehaviour
{
    public RotationDegree degreeOfFreedom;

    public void SetValue(float value)
    {
        var t = transform;
        t.localEulerAngles = degreeOfFreedom switch
        {
            RotationDegree.RotateX => new Vector3(value, 0, 0),
            RotationDegree.RotateY => new Vector3(0, value, 0),
            RotationDegree.RotateZ => new Vector3(0, 0, value),
            _ => t.localEulerAngles
        };
    }

    public float GetValue()
    {
        return transform.localEulerAngles[(int) degreeOfFreedom];
    }
    
}


using System;
using UnityEngine;

public class CoordinateSystem
{
	public enum Axis
	{
		Up,
		Forward,
		Right
	}
	
	public CoordinateSystem(Transform t, Vector3 shift)
	{
		Up = t.up;
		Forward = t.forward;
		Pos = t.position + shift;
	}
	
	public CoordinateSystem(Vector3 position, Vector3 up, Vector3 forward)
	{
		Up = up;
		Forward = forward;
		Pos = position;
	}
	
	public CoordinateSystem(CoordinateSystem system)
	{
		Up = system.Up;
		Forward = system.Forward;
		Pos = system.Pos;
	}

	public Vector3 Up { get; private set; }
	public Vector3 Forward { get; private set; }
	public Vector3 Right => Vector3.Cross(Up, Forward);	// z
	public Vector3 Pos { get; private set; }

	public void Rotate(RotationDegree degree, float value)
	{
		switch(degree)
		{
			case RotationDegree.RotateY:
				Forward = Quaternion.AngleAxis(value, Up) * Forward;
				break;
			case RotationDegree.RotateZ:
				Up = Quaternion.AngleAxis(value, Forward) * Up;
				break;
			case RotationDegree.RotateX:
				var r = Right;
				Forward = Quaternion.AngleAxis(value, r) * Forward;
				Up = Quaternion.AngleAxis(value, r) * Up;
				break;
		}
	}

	public void TranslateUp(float value)
	{
		Pos += value * Up;
	}
}